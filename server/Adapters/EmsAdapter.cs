using RealTick.Api.Communication;
using RealTick.Api.Data;
using RealTick.Api.Talipc;
using System;
using System.Collections.Generic;

namespace Eze.Quantbox
{

    public class OrderRecord
    {
        public string OrderID;
        public string Portfolio;
        public string OrderTag;
        public string Type;
        public string Side;
        public string Symbol;
        public string Status;
        public long lQty;
        public long lQtyTraded;
        public double dPrice;
        public long lWorking;

        public string GetDetails()
        {
            string sDetails = "";
            sDetails = Side + " " + lQty + " " + Symbol + ": " + Status;
            if (lQtyTraded > 0)
                sDetails += "(" + lQtyTraded + " traded @ " + dPrice.ToString("F2") + ")";
            return sDetails;
        }
    }

    public class OrderStats
    {
        public long Total;
        public long Pending;
        public long Staged;     //  live but not working
        public long Working;    //  at least one order out in the market
        public long Completed;  // no longer live, and at least partially completed
        public long Deleted;    // no longer live, nothing filled

        public void Reset()
        {
            Total = Pending = Staged = Working = Completed = Deleted = 0;
        }

        public void AddOrder(OrderRecord order)
        {
            UpdateOrderStats(order, true);
        }

        // either add an order to stats, or remove it.
        public void UpdateOrderStats(OrderRecord order, bool bAdd=true)
        {
            int inc = bAdd ? 1 : -1;

            Total++;
            switch (order.Status)
            {
                case "PENDING":
                    Pending += inc;
                    break;
                case "LIVE":
                    {
                        if (order.lWorking > 0)
                            Working += inc;
                        else
                            Staged += inc;
                        break;
                    }
                case "COMPLETED":
                    Completed += inc;
                    break;
                case "DELETED":
                    Deleted += inc;
                    break;
            }
        }

        public void ReplaceOrder(OrderRecord newOrder, OrderRecord oldOrder)
        {
            if (oldOrder != null)
                UpdateOrderStats(oldOrder, false);  // remove old order's contribution to stats
            UpdateOrderStats(newOrder, true);
        }
    }

    public class EmsAdapter : ITradingSystemAdapter, IDisposable
    {
        static TalipcToolkitApp _app;
        private AsyncQuery _query;

        public EmsSettings Settings { get; }
        public Dictionary<string, OrderStats> Stats = new Dictionary<string, OrderStats>();
        public OrderStats GetStats(string AlgoName)
        {
            if (Stats.ContainsKey(AlgoName))
                return Stats[AlgoName];
            else
                return null;
        }

        private Dictionary<string, OrderRecord> _book = new Dictionary<string, OrderRecord>();

        public EmsAdapter(EmsSettings settings)
        {
            if ( settings != null )
                Settings = settings;
            if (_app == null)
            {
                _app = new TalipcToolkitApp();
            }

            Stats = new Dictionary<string, OrderStats>();

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
            else
            {
                _query.Advise("ORDERS;*;", "TAL4");
                _query.Request("ORDERS;*;", "TAL4");
            }
        }

        #region Callbacks
        private void OnRequestData(object sender, DataEventArgs e)
        {
            string sItem = e.Item;
            Console.WriteLine("OnRequestData: " + sItem);

            if (sItem.StartsWith("ORDERS;"))
            {
                IDataBlock block = e.Data.GetDataAsBlock();
                ProcessDataBlock(block, true);
            }

        }

        private void OnExecuteAck(object sender, AckEventArgs e)
        {
            Console.WriteLine("OnExecuteAck (" + e.Status + ")");
        }

        private void OnExecute(object sender, ExecuteEventArgs e)
        {
            Console.WriteLine("OnExecute");
        }

        private void OnAdviseData(object sender, DataEventArgs e)
        {
            //Console.WriteLine("OnAdviseData");
            string sItem = e.Item;
            if (sItem.StartsWith("ORDERS;"))
            {
                IDataBlock block = e.Data.GetDataAsBlock();
                ProcessDataBlock(block, false);
            }
        }

        private void ProcessDataBlock(IDataBlock block, bool bRequest)
        {
            foreach (Row row in block)
            {
                OrderRecord order = new OrderRecord();
                foreach (Field f in row)
                {
                    if (f.FieldInfo.Name == "TYPE")
                        order.Type = f.StringValue;
                    else if (f.FieldInfo.Name == "ORDER_ID")
                        order.OrderID = f.StringValue;
                    else if (f.FieldInfo.Name == "CURRENT_STATUS")
                        order.Status = f.StringValue;
                    else if (f.FieldInfo.Name == "DISP_NAME")
                        order.Symbol = f.StringValue;
                    else if (f.FieldInfo.Name == "VOLUME")
                        order.lQty = f.IntValue;
                    else if (f.FieldInfo.Name == "VOLUME_TRADED")
                        order.lQtyTraded = f.IntValue;
                    else if (f.FieldInfo.Name == "BUYORSELL")
                        order.Side = f.StringValue;
                    else if (f.FieldInfo.Name == "AVG_PRICE")
                        order.dPrice = f.DoubleValue;
                    else if (f.FieldInfo.Name == "WORKING_QTY")
                        order.lWorking = f.LongValue;
                    else if (f.FieldInfo.Name == "ORDER_TAG")
                        order.OrderTag = f.StringValue;
                    else if (f.FieldInfo.Name == "PORTFOLIO_NAME")
                        order.Portfolio = f.StringValue;
                }
                ProcessOrder(order, bRequest);
            }
        }

        private void ProcessOrder(OrderRecord order, bool bRequest)
        {
            if (order != null)
            {
                if (order.Type == "UserSubmitStagedOrder")
                {
                    string sDetails = order.GetDetails();
                    if (!bRequest)    // don't want to spam the console at startup...
                        Console.WriteLine("Received Order Update: " + sDetails);
                    OrderRecord oldOrder = null;
                    if (_book.ContainsKey(order.OrderID))
                        oldOrder = _book[order.OrderID];

                    OrderStats stats = GetOrCreateStats(order.Portfolio);
                    stats.ReplaceOrder(order, oldOrder); // it's ok if oldOrder is null
                    _book[order.OrderID] = order;
                }
            }
        }

        private OrderStats GetOrCreateStats(string name)
        {
            if (name == null || name == "")
                name = "<none>";
            OrderStats stats = null;
            if (Stats.ContainsKey(name))
                stats = Stats[name];
            if (stats == null)
            {
                stats = new OrderStats();
                Stats[name] = stats;
            }
            return stats;
        }

        // Call this to walk the book and recalculate all stats.  May not be necessary anymore, now that we support 
        // delta updates for stats.
        private void RecalculateStats()
        {
            foreach (KeyValuePair<string, OrderStats> s in Stats)
                s.Value.Reset();

            foreach ( KeyValuePair<string, OrderRecord> entry in _book )
            {
                OrderStats stats = GetOrCreateStats(entry.Value.Portfolio);
                stats.AddOrder(entry.Value);
            }
        }

        private void OnOtherAck(object sender, AckEventArgs e)
        {
            //Console.WriteLine("OnOtherAck");
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
        public Action PublishStats { get; set; }

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
                string side = trade.Side;
                if (side == "Short" || side == "SHORT")
                    side = "SellShort";
                row.Add(new Field("BUYORSELL", FieldType.StringScalar, side));
                row.Add(new Field("EXIT_VEHICLE", FieldType.StringScalar, "NONE"));
                row.Add(new Field("CURRENCY", FieldType.StringScalar, "USD"));
                row.Add(new Field("ACCT_TYPE", FieldType.IntScalar, 119));
                row.Add(new Field("STYP", FieldType.IntScalar, 1)); // STOCK
                row.Add(new Field("PORTFOLIO_NAME", FieldType.StringScalar, trade.Algo));
                row.Add(new Field("ORDER_TAG", FieldType.StringScalar, trade.Algo)); // STOCK

                data.Add(row);
            }

            _query.Poke("ORDERS;*;", data.ConvertToBinary());

            return true;
        }

        #region IDisposable
        private bool disposedValue = false;

        public event StatsEventHandler StatsChanged;

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