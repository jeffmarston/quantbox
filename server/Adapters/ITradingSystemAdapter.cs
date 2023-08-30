using System;
using System.Collections.Generic;

namespace Eze.Quantbox
{
    public delegate void StatsEventHandler(string name, OrderStats stats);

    public interface ITradingSystemAdapter
    {
        OrderStats GetStats(string algoName);

        void StatsRecalcNeeded(string algoName);

        bool CreateTrades(IList<Trade> trades);

        event StatsEventHandler StatsChanged;

        void CancelAllOrders(string algoName);

        void SendOffWave(string algoName);
    }
}