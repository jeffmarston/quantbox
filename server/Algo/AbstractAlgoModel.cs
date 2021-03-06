﻿using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Eze.Quantbox
{

    public abstract class AbstractAlgoModel : IDisposable
    {
        public AlgoMetadata Metadata { get; set; }
        public OrderStats Stats { get; protected set; }
        
        public IClientProxy Publisher { protected get; set; }
        public ITradingSystemAdapter Adapter { get; set; }
        public string Name { get; protected set; }
        public bool Enabled
        {
            get { return Metadata.Enabled; }
            set
            {
                Metadata.Enabled = value;
                PublishState();
            }
        }
        public int Violations { get; set; }
        public void StampHistory()
        {
            // clear out anything older than 2 minutes
            (History as List<AlgoHistory>).RemoveAll(o => { return o.Date.AddMinutes(2) < DateTime.Now; });

            var date = DateTime.Now;
            date = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Kind);
            History.Add(new AlgoHistory()
            {
                Date = date,
                Value = (int)Stats.Total
            });
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
                Debug.WriteLine("Sending state update for " + this.Name);
            }
        }

        //public void PublishStats()
        //{
        //    if (Publisher != null)
        //    {
        //        Publisher.SendAsync("algo-stats", this.Name, this.Stats);
        //        Debug.WriteLine("Sending state update for " + Name);
        //    }
        //}
        public void PublishStats(OrderStats stats)
        {
            if (Publisher != null)
            {
                Publisher.SendAsync("algo-stats", Name, stats);
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

        public void PublishDelete()
        {
            if (Publisher != null)
            {
                Publisher.SendAsync("delete-algo", Name);
                Debug.WriteLine("Delete algo " + Name);
            }
        }

        public abstract void Dispose();
    }
}