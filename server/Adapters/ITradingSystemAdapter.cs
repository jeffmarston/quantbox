using System.Collections.Generic;

namespace Eze.Quantbox
{
    public interface ITradingSystemAdapter
    {
        bool CreateTrades(IList<Trade> trades);
    }
}