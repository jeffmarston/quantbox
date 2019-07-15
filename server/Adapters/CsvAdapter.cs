using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Eze.Quantbox
{

    public class CsvAdapter: ITradingSystemAdapter
    {
        private const string csvFolder = @"csv\";
        public CsvAdapter(EmsSettings emsSettings)
        {
            Filename = "GeneratedTrades";
            if (!Directory.Exists(csvFolder))
            {
                Directory.CreateDirectory(csvFolder);
            }
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
                File.AppendAllText(csvFolder + Filename + "_" + trades[0].Algo+".csv", sb.ToString());
            }
            return true;
        }
    }
}