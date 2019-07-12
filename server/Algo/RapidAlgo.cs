using System;
using System.Collections.Generic;
using System.Timers;
using Microsoft.AspNetCore.SignalR;

namespace Eze.Quantbox
{
    public class RapidAlgo : AbstractAlgoModel
    {
        private Random _rand = new Random();
        private Timer _timer = new Timer();

        public RapidAlgo()
        {
            _timer.Interval = 1000;
            _timer.Elapsed += Timer_Elapsed;
            _timer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.Enabled && _rand.Next(2)==0)
            {
                var numTrades = _rand.Next(1, 4);
                var tradesToCreate = new List<Trade>();
                for (int i = 0; i < numTrades; i++)
                {
                    var trade = new Trade()
                    {
                        Symbol = Symbols[_rand.Next(0, Symbols.Count)],
                        Side = "Buy",
                        Amount = _rand.Next(1, 40) * 100,
                        Trader = "JEFF",
                        Manager = "FO",
                        Algo = Name
                    };
                    tradesToCreate.Add(trade);
                    PublishToConsole(Name + " created trade: " + trade.ToString());
                }
                Adapter.CreateTrades(tradesToCreate);
                TradesCreated += numTrades;
                PublishState();
            }
            else
            {
                // do nothing, just record history;
                TradesCreated = TradesCreated;
            }
        }
    }
}