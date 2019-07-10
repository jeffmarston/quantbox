using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Eze.Quantbox
{

    public class CsvAdapter: ITradingSystemAdapter
    {
        public CsvAdapter()
        {
            Filename = "GeneratedTrades.csv";
        }
        public string Filename { get; internal set; }
        private static object _lockObj = new object();

        public bool CreateTrades(IList<Trade> trades)
        {
            if (trades.Count == 0)
            {
                return false;
            }
            StringBuilder sb = new StringBuilder();
            foreach (var trade in trades)
            {
                sb.Append(trade.Symbol).Append(",");
                sb.Append(trade.Side).Append(",");
                sb.Append(trade.Amount).Append(",");
                sb.Append(trade.Trader).Append(",");
                sb.Append(trade.Manager).Append(",");
                sb.Append(trade.Algo).AppendLine();
            }
            lock (_lockObj)
            {
                File.WriteAllText("csv\\" + Filename + "_" + trades[0].Algo, sb.ToString());
            }
            return true;
        }
    }
}