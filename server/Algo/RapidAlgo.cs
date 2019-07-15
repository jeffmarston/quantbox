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
            State = new AlgoState();

            _timer.Interval = 1000;
            _timer.Elapsed += Timer_Elapsed;
            _timer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.Enabled && _rand.Next(Metadata.FrequencySec) == 0)
            {
                // Prevent crazy huge number of trades.
                if (TradesCreated > 10000) {
                    TradesCreated = 0;
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
                TradesCreated += numTrades;
                PublishState();
                PublishToConsole("Sending to Adapter: " + Adapter.Settings.Bank);
            }
            else
            {
                // do nothing, just record history;
                TradesCreated = TradesCreated;
            }
        }
    }
}