using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Eze.Quantbox
{
    public class SlowBatchAlgo : AbstractAlgoModel
    {
        private Random _rand = new Random();
        private Timer _timer = new Timer();

        public SlowBatchAlgo()
        {
            _timer.Interval = 15000;
            _timer.Elapsed += Timer_Elapsed;
            _timer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.Enabled && _rand.Next(6) == 0)
            {
                var numTrades = _rand.Next(4, 8)*20;
                var tradesToCreate = new List<Trade>();
                for (int i = 0; i < numTrades; i++)
                {
                    var trade = new Trade()
                    {
                        Symbol = Symbols[_rand.Next(0, Symbols.Count)],
                        Side = "Buy",
                        Amount = _rand.Next(6, 12) * 100,
                        Trader = "ROB",
                        Manager = "CS",
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