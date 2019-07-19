using System;
using System.Collections.Generic;
using System.Timers;

namespace Eze.Quantbox
{
    public class RapidAlgo : AbstractAlgoModel
    {
        private Random _rand = new Random();
        private Timer _timer = new Timer();

        public RapidAlgo(AlgoMetadata metadata, ITradingSystemAdapter adapter)
        {
            Name = metadata.Name;
            Adapter = adapter;
            Metadata = metadata;
            Stats = new AlgoStats(Name);

            _timer.Interval = 1000;
            _timer.Elapsed += Timer_Elapsed;
            _timer.Enabled = true;
        }

        public override void Dispose()
        {
            _timer.Enabled = false;
            Metadata = null;
            Stats = null;
            Adapter = null;
            _timer.Dispose();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.Enabled && _rand.Next(Metadata.FrequencySec) == 0)
            {
                // Prevent crazy huge number of trades.
                if (Stats.Created > 10000) {
                    Stats.Created = 0;
                }
                var numTrades = _rand.Next(Metadata.MinBatchSize, Metadata.MaxBatchSize);
                var tradesToCreate = new List<Trade>();
                for (int i = 0; i < numTrades; i++)
                {
                    var trade = new Trade()
                    {
                        Symbol = Metadata.Symbols[_rand.Next(0, Metadata.Symbols.Count)],
                        Side = (_rand.Next(100) < Metadata.BuyShortRatio) ? "Buy" : "Short",
                        Amount = _rand.Next(1, 40) * 100,
                        Trader = "JEFF",
                        Manager = "FO",
                        Algo = Name
                    };
                    tradesToCreate.Add(trade);
                    PublishToConsole(Name + " created trade: " + trade.ToString());
                }

                Adapter.CreateTrades(tradesToCreate);

                OrderStats EmsStats = Adapter.GetStats(Name);
                Stats.Created += numTrades;
                if ( EmsStats != null )
                    Stats.Routed = (int)(EmsStats.Completed + EmsStats.Working);
                else
                    Stats.Routed += Stats.Created / 2;
                Stats.Exceptions += Stats.Created / 5;
                StampHistory();

                PublishStats();
            }
            else
            {
                // do nothing, just record history;
                StampHistory();
            }

            OrderStats stats = Adapter.GetStats(Name);
            if ( stats != null )
                PublishToConsole(Name + ": Total: " + stats.Total + "  (Working: " + stats.Working + ", Staged =" + stats.Staged + ", Completed =" + stats.Completed + ")");
        }
    }
}