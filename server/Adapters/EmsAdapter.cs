﻿using RealTick.Api.Communication;
using RealTick.Api.Data;
using RealTick.Api.Talipc;
using System;
using System.Collections.Generic;
using System.Text;

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
        public Price ArrivalPrice;
        public ulong ConversionRuleFlags;
        public string TicketID; // only exists on child orders

        public string GetDetails()
        {
            string sDetails = "";
            sDetails = Side + " " + lQty + " " + Symbol + ": " + Status;
            if (lQtyTraded > 0)
                sDetails += "(" + lQtyTraded + " traded @ " + dPrice.ToString("F2") + ")";
            return sDetails;
        }

        public bool IsLive()
        {
            return Status == "LIVE" || Status == "PENDING";
        }
        // If buy, then 1.  If sell, than -1.  Otherwise, 0
        public int SideMult()
        {
            if (Side.Equals("Buy", StringComparison.OrdinalIgnoreCase))
                return 1;
            else if (Side.Equals("Sell", StringComparison.OrdinalIgnoreCase) || Side.Equals("SellShort", StringComparison.OrdinalIgnoreCase))
                return -1;
            // not a buy or a sell?
            return 0;
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
        public long TotalQty;
        public long CompletedQty;
        public double TotalValue;   // this is really Target Value:  Value of completed portion, plus estimate of residual portion.
        public double CompletedValue;
        public double CompletedPct;
        public double BenchmarkValue;
        public double BenchmarkPL;
        public long Manual;
        public long AutoRouted;

        public bool _statsRecalcNeeded = false;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            double dValueCompletion = GetValueCompletionRate() * 100;
            sb.AppendFormat("{0:F2}% Completed", dValueCompletion);
            sb.AppendFormat(", Total Orders: {0}", Total);
            sb.AppendFormat(" (Working: {0}, Staged: {1}, Completed: {2})", Working, Staged, Completed);
            sb.AppendFormat(", P&L: {0:C2}", BenchmarkPL);
            sb.AppendLine();
            return sb.ToString();
        }

        public double GetQtyCompletionRate()
        {
            if (TotalQty == 0)
                return 0.0;
            return CompletedQty / (double)TotalQty;
        }

        public double GetValueCompletionRate()
        {
            if (TotalValue == 0)
                return 0.0;
            return CompletedValue / (double)TotalValue;
        }

        public void AddOrder(OrderRecord order)
        {
            UpdateOrderStats(order, true);
        }

        // either add an order to stats, or remove it.
        public void UpdateOrderStats(OrderRecord order, bool bAdd=true)
        {
            int inc = bAdd ? 1 : -1;

            Total += inc;
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

            long OrderTargetQty = order.IsLive() ? order.lQty : order.lQtyTraded;
            long OrderResidual = OrderTargetQty - order.lQtyTraded;
            if (OrderResidual < 0)
                OrderResidual = 0;
            double OrderCompletedValue = (double)(order.lQtyTraded * order.dPrice);
            double OrderResidualValue = (double)(OrderResidual * order.ArrivalPrice.DecimalValue);
            double OrderTargetValue = OrderCompletedValue + OrderResidualValue;

            if ( OrderTargetValue < OrderCompletedValue )
            {
                // this shouldn't happen, but it seems like it does occasionally...
                OrderTargetValue = OrderCompletedValue;
            }

            double OrderBenchmarkValue = (double)(OrderTargetQty * order.ArrivalPrice.DecimalValue);
            // Ex:  bought at 5, benchmark 7 -> P&L = 2.  Flip the sign for sells (SideMult)
            double OrderPL = (OrderBenchmarkValue - OrderTargetValue) * order.SideMult();

            TotalQty += (OrderTargetQty * inc);
            CompletedQty += (order.lQtyTraded * inc);
            CompletedValue += (OrderCompletedValue * inc);
            TotalValue += (OrderTargetValue * inc);
            BenchmarkValue += (OrderBenchmarkValue * inc);
            BenchmarkPL += (OrderPL * inc);
            CompletedPct = 1000 * GetValueCompletionRate();
            CompletedPct = Math.Truncate(CompletedPct) / 10;

            if (order.Status != "DELETED" && order.Status != "PENDING")
            {
                const ulong CRF_SUBMITTED_BY_RULES = 0x00000001; // sorry for this magic

                if ((order.ConversionRuleFlags & CRF_SUBMITTED_BY_RULES) != 0)
                    AutoRouted += inc;
                else if (order.lWorking > 0 || order.lQtyTraded > 0)
                    Manual += inc;
            }
        }

    // Called when there is a realtime update to an order
    public bool ReplaceOrder(OrderRecord newOrder, OrderRecord oldOrder)
        {
            bool AnythingInterestingChanged = false;
            if ( oldOrder == null )
                AnythingInterestingChanged = true;
            else
            {
                if (newOrder.Status != oldOrder.Status)
                    AnythingInterestingChanged = true;
                else if ( newOrder.lWorking != oldOrder.lWorking )
                    AnythingInterestingChanged = true;
                else if ( newOrder.lQtyTraded != oldOrder.lQtyTraded )
                    AnythingInterestingChanged = true;
                else if ( newOrder.lQty != oldOrder.lQty )
                    AnythingInterestingChanged = true;
            }
            if (AnythingInterestingChanged)
            {
                if (oldOrder != null)
                    UpdateOrderStats(oldOrder, false);  // remove old order's contribution to stats
                UpdateOrderStats(newOrder, true);
            }
            return AnythingInterestingChanged;
        }
    }

    public class EmsAdapter : ITradingSystemAdapter, IDisposable
    {
        static TalipcToolkitApp _app;
        private AsyncQuery _query;
        private Random _rand = new Random();

        public EmsSettings Settings { get; set; }
        public Dictionary<string, OrderStats> Stats = new Dictionary<string, OrderStats>();
        public OrderStats GetStats(string AlgoName)
        {
            if (Stats.ContainsKey(AlgoName))
                return Stats[AlgoName];
            else
                return null;
        }

        private Dictionary<string, OrderRecord> _book = new Dictionary<string, OrderRecord>();
        private Dictionary<string, OrderRecord> _child_book = new Dictionary<string, OrderRecord>();


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
                    else if (f.FieldInfo.Name == "TRDPRC_1")
                        order.ArrivalPrice = f.PriceValue;
                    else if (f.FieldInfo.Name == "TS3_CONVERSION_RULE_FLAGS")
                        order.ConversionRuleFlags = (ulong)f.LongValue;
                    else if (f.FieldInfo.Name == "TICKET_ID")
                        order.TicketID = f.StringValue;
                }
                ProcessOrder(order, bRequest);
            }

            if ( bRequest ) // when we get a data refesh, re-publish all stats
            {
                foreach (KeyValuePair<string, OrderStats> s in Stats)
                {
                    OrderStats stats = s.Value;
                    // Fire event to publish the result
                    StatsChanged?.Invoke(s.Key, stats);
                }
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

                    // I don't like this...
                    if ( stats._statsRecalcNeeded )
                    {
                        RecalculateStats(order.Portfolio);
                    }

                    // Fire event to publish the result
                    if (!bRequest)    // only do this here for realtime updates.  Otherwise, do it outside of loop.
                        StatsChanged?.Invoke(order.Portfolio, stats);
                    if ( !bRequest )
                    {
                        if (_rand.Next(10) == 0)
                        {
                            var IDs = new List<string>();
                            IDs.Add(order.OrderID);
                            AppendMagicDataToOrders(IDs);
                        }

                    }
                }
                else if (order.Type == "UserSubmitOrder")
                {
                    _child_book[order.OrderID] = order;
                }
            }
        }

        private List<string> BuildListOfChildOrderIDs(string algoName, bool bLiveOnly=true)
        {
            List<string> IDs = new List<string>();

            foreach (KeyValuePair<string, OrderRecord> entry in _child_book)
            {
                string TicketID = entry.Value.TicketID;
                if (TicketID != null && TicketID.Length > 0)
                {
                    // look up the parent and see if it is associated with the desired Algo
                    OrderRecord parent = _book[entry.Value.TicketID];
                    if (parent != null && parent.Portfolio == algoName)
                    {
                        if ( !bLiveOnly || entry.Value.IsLive() )
                            IDs.Add(entry.Value.OrderID);
                    }
                }
            }


            return IDs;
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

        public void StatsRecalcNeeded(string name)
        {
            if (Stats.ContainsKey(name))
            {
                OrderStats stats = Stats[name];
                if ( stats != null )
                    stats._statsRecalcNeeded = true;
            }
        }

        // Call this to walk the book and recalculate all stats.  May not be necessary anymore, now that we support 
        // delta updates for stats.
        public void RecalculateStats(string algoName)
        {
            if ( algoName != null && algoName != "" )
            {
                if (Stats.ContainsKey(algoName))
                    Stats[algoName] = null;
            }
            else // clear them all
                Stats.Clear();

            foreach ( KeyValuePair<string, OrderRecord> entry in _book )
            {
                if (algoName == null || algoName.Length == 0 || entry.Value.Portfolio == algoName)
                {
                    OrderStats stats = GetOrCreateStats(entry.Value.Portfolio);
                    stats.AddOrder(entry.Value);
                }
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

        public bool CancelOneOrder(string orderID, string msg)
        {
            var data = new DataBlock();
            var row = new Row();

            row.Add(new Field("TYPE", FieldType.StringScalar, "UserSubmitCancel"));
            row.Add(new Field("REFERS_TO_ID", FieldType.StringScalar, orderID));
            if ( msg != null && msg.Length > 0 )
                row.Add(new Field("REASON", FieldType.StringScalar, msg));

            data.Add(row);

            _query.Poke("ORDERS;*;", data.ConvertToBinary());

            return true;
        }

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
                if ( side == "SellShort" )
                    row.Add(new Field("SHORT_LOCATE_ID", FieldType.StringScalar, "GSCO"));

                row.Add(new Field("EXIT_VEHICLE", FieldType.StringScalar, "NONE"));
                row.Add(new Field("CURRENCY", FieldType.StringScalar, "USD"));
                row.Add(new Field("ACCT_TYPE", FieldType.IntScalar, 119));
                if (trade.Symbol != "BAD") {
                    row.Add(new Field("STYP", FieldType.IntScalar, 1)); // STOCK
                }
                row.Add(new Field("PORTFOLIO_NAME", FieldType.StringScalar, trade.Algo));
                row.Add(new Field("ORDER_TAG", FieldType.StringScalar, trade.Algo)); // STOCK

                data.Add(row);
            }

            _query.Poke("ORDERS;*;", data.ConvertToBinary());

            /*
            if (_rand.Next(5) == 0)
            {
                var IDs = new List<string>();

                foreach (KeyValuePair<string, OrderRecord> entry in _book)
                {
                    if (entry.Value.Symbol == "MSFT")
                        IDs.Add(entry.Value.OrderID);
                }
                AppendMagicDataToOrders(IDs);
            }
            */

            return true;
        }

        public bool AppendMagicDataToOrders(IList<string> OrderIDs)
        {
            var data = new DataBlock();
            foreach (var ID in OrderIDs)
            {
                var row = new Row();

                row.Add(new Field("TYPE", FieldType.StringScalar, "AppendEventData"));
                row.Add(new Field("REFERS_TO_ID", FieldType.StringScalar, ID));
                // populate the FIX_MSG field just to demonstrate the concept   

                string sRandomValue;

                int r = _rand.Next(100);
                sRandomValue = r.ToString() + "%";
                row.Add(new Field("FIX_MSG", FieldType.StringScalar, sRandomValue));
                const int UPDATE_OVERWRITE_EXISTING = 2;
                row.Add(new Field("UPDATE_TYPE", FieldType.IntScalar, UPDATE_OVERWRITE_EXISTING));
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

        public void CancelAllOrders(string algoName)
        {
            List<string> orderIDs = BuildListOfChildOrderIDs(algoName, true);
            if (orderIDs != null)
            {
                foreach (string ID in orderIDs)
                {
                    CancelOneOrder(ID, "Cancel All");
                }
            }
        }
        #endregion

    }
}