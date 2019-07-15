using System.Collections.Generic;

namespace Eze.Quantbox
{
    public interface ITradingSystemAdapter
    {
        EmsSettings Settings { get; }

        bool CreateTrades(IList<Trade> trades);
    }
}