using System;
using System.Collections.Generic;

namespace Eze.Quantbox
{
    public delegate void StatsEventHandler(string name, OrderStats stats);

    public interface ITradingSystemAdapter
    {
        EmsSettings Settings { get; }

        OrderStats GetStats(string AlgoName);

        bool CreateTrades(IList<Trade> trades);


        event StatsEventHandler StatsChanged;
    }
}