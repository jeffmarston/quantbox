using RealTick.Api.Communication;
using RealTick.Api.Data;
using RealTick.Api.Talipc;
using System;
using System.Collections.Generic;

namespace Eze.Quantbox
{
    public class EmsAdapter : ITradingSystemAdapter, IDisposable
    {
        private TalipcToolkitApp _app;
        private AsyncQuery _query;

        public EmsSettings Settings { get; }

        public EmsAdapter(EmsSettings settings)
        {
            Settings = settings;
            _app = new TalipcToolkitApp();

            Service = "ACCOUNT_GATEWAY";
            Topic = "ORDER";

            // THESE ARE SETTINGS
            GatewayMachine = settings.Gateway;
            Bank = settings.Bank;
            Branch = settings.Branch;
            Customer = settings.Customer;
            Deposit = settings.Deposit;

            _query = _app.GetAsyncQuery(GatewayMachine, Service, Topic);
            _query.OnTerminate += OnTerminate;
            _query.OnOtherAck += OnOtherAck;
            _query.OnAdviseData += OnAdviseData;
            _query.OnExecute += OnExecute;
            _query.OnExecuteAck += OnExecuteAck;
            _query.OnRequestData += OnRequestData;

            if (!_query.Connect())
            {
                Console.WriteLine("No dice on connection");
            }
        }

        #region Callbacks
        private void OnRequestData(object sender, DataEventArgs e)
        {
            Console.WriteLine("OnRequestData");
        }

        private void OnExecuteAck(object sender, AckEventArgs e)
        {
            Console.WriteLine("OnExecuteAck");
        }

        private void OnExecute(object sender, ExecuteEventArgs e)
        {
            Console.WriteLine("OnExecute");
        }

        private void OnAdviseData(object sender, DataEventArgs e)
        {
            Console.WriteLine("OnAdviseData");
        }

        private void OnOtherAck(object sender, AckEventArgs e)
        {
            Console.WriteLine("OnOtherAck");
        }

        private void OnTerminate(object sender, EventArgs e)
        {
            Console.WriteLine("OnTerminate");
        }
        #endregion

        public string GatewayMachine { get; private set; }
        public string Service { get; private set; }
        public string Topic { get; private set; }
        public string Bank { get; private set; }
        public string Branch { get; private set; }
        public string Customer { get; private set; }
        public string Deposit { get; private set; }

        public bool CreateTrades(IList<Trade> trades)
        {
            if (trades.Count == 0)
            {
                return false;
            }

            var data = new DataBlock();
            foreach (var trade in trades)
            {
                var row = new Row();

                row.Add(new Field("TYPE", FieldType.StringScalar, "UserSubmitStagedOrder"));
                row.Add(new Field("BANK", FieldType.StringScalar, Bank));
                row.Add(new Field("BRANCH", FieldType.StringScalar, Branch));
                row.Add(new Field("CUSTOMER", FieldType.StringScalar, Customer));
                row.Add(new Field("DEPOSIT", FieldType.StringScalar, Deposit));
                row.Add(new Field("DISP_NAME", FieldType.StringScalar, trade.Symbol));
                row.Add(new Field("VOLUME", FieldType.IntScalar, trade.Amount));
                row.Add(new Field("VOLUME_TYPE", FieldType.StringScalar, "AsEntered"));
                row.Add(new Field("PRICE_TYPE", FieldType.StringScalar, "Market"));
                row.Add(new Field("PRICE", FieldType.PriceScalar, new Price("0.0")));
                row.Add(new Field("GOOD_UNTIL", FieldType.StringScalar, "DAY"));
                row.Add(new Field("BUYORSELL", FieldType.StringScalar, "BUY"));
                row.Add(new Field("EXIT_VEHICLE", FieldType.StringScalar, "NONE"));
                row.Add(new Field("CURRENCY", FieldType.StringScalar, "USD"));
                row.Add(new Field("ACCT_TYPE", FieldType.IntScalar, 119));
                row.Add(new Field("STYP", FieldType.IntScalar, 1)); // STOCK

                data.Add(row);
            }

            _query.Poke("ORDERS;*;", data.ConvertToBinary());

            return true;
        }

        #region IDisposable
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_query != null)
                    {
                        _query.OnTerminate -= OnTerminate;
                        _query.OnOtherAck -= OnOtherAck;
                        _query.OnAdviseData -= OnAdviseData;
                        _query.OnExecute -= OnExecute;
                        _query.OnExecuteAck -= OnExecuteAck;
                        _query.OnRequestData -= OnRequestData;
                        _query.Dispose();
                        _query = null;
                    }

                    if (_app != null)
                    {
                        _app.Dispose();
                        _app = null;
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}