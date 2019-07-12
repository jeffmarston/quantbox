using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Eze.Quantbox
{

    public abstract class AbstractAlgoModel
    {
        protected List<string> Symbols = new List<string>() { "AAPL", "BBY", "COKE", "DELL", "ENR", "F", "GOOG", "HD" };
        private int _tradesCreated;

        public IClientProxy Publisher { protected get; set; }
        public ITradingSystemAdapter Adapter { protected get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public int Violations { get; set; }
        public int TradesCreated
        {
            get => _tradesCreated;
            set
            {
                _tradesCreated = value;
                // clear out anything older than 2 minutes
                (History as List<AlgoHistory>).RemoveAll(o => { return o.Date.AddMinutes(2) < DateTime.Now; });

                var date = DateTime.Now;
                date = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Kind);
                History.Add(new AlgoHistory()
                {
                    Date = date,
                    Value = _tradesCreated
                });
            }
        }
        public IList<AlgoHistory> History { get; private set; }

        public AbstractAlgoModel()
        {
            History = new List<AlgoHistory>();
        }

        public void PublishState()
        {
            if (Publisher != null)
            {
                Publisher.SendAsync("algos", this);
                Debug.WriteLine("Sending state update for " + Name);
            }
        }
        public void PublishToConsole(string msg)
        {
            if (Publisher != null)
            {
                Publisher.SendAsync("console", msg, Name);
                Debug.WriteLine(msg);
            }
        }
    }
}