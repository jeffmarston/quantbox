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
            Stats = new OrderStats();
            Adapter.StatsChanged += Adapter_StatsChanged;


            _timer.Interval = 1000;
            _timer.Elapsed += Timer_Elapsed;
            _timer.Enabled = true;
        }

        private void Adapter_StatsChanged(string name, OrderStats stats)
        {
            if (name == "*" || Name == name)
            {
                Stats = stats;
                PublishToConsole(Name + " Stats: " + stats);
                PublishStats(stats);
            }
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
                if (Stats.Total > 10000)
                {
                    Enabled = false;
                }
                var numTrades = _rand.Next(Metadata.MinBatchSize, Metadata.MaxBatchSize);
                var tradesToCreate = new List<Trade>();
                for (int i = 0; i < numTrades; i++)
                {
                    var trade = new Trade()
                    {
                        Symbol = Metadata.Symbols[_rand.Next(0, Metadata.Symbols.Count)],
                        Side = (_rand.Next(100) < Metadata.BuyShortRatio) ? "Buy" : "Short",
                        Amount = _rand.Next(1, 60) * 100,
                        Trader = "JEFF",
                        Manager = "FO",
                        Algo = Name
                    };
                    tradesToCreate.Add(trade);
                    PublishToConsole(Name + " created trade: " + trade.ToString());
                }

                Adapter.CreateTrades(tradesToCreate);

                OrderStats EmsStats = Adapter.GetStats(Name);
                StampHistory();
            }
            else
            {
                // do nothing, just record history;
                StampHistory();
            }
        }
    }
}