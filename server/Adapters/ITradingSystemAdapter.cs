using System.Collections.Generic;

namespace Eze.Quantbox
{
    public interface ITradingSystemAdapter
    {
        EmsSettings Settings { get; }
        OrderStats GetStats(string AlgoName);

        bool CreateTrades(IList<Trade> trades);
    }
}