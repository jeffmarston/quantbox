using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;

namespace Eze.Quantbox
{
    public class CsvAdapter: ITradingSystemAdapter
    {
        private Random _rand = new Random();
        private Timer _timer = new Timer();
        public EmsSettings Settings { get; }
        public OrderStats GetStats(string AlgoName) { return null; }

        private const string csvFolder = @"csv\";
        public CsvAdapter(EmsSettings emsSettings)
        {
            Settings = emsSettings;
            FilenameRoot = "GeneratedTrades";
            if (!Directory.Exists(csvFolder))
            {
                Directory.CreateDirectory(csvFolder);
            }

            _timer.Interval = 1000;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Enabled = true;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // algo name "*" has no real meaning for the CsvAdapter. But he name is required for the EmsAdapater so we have to pass something.
            StatsChanged?.Invoke("*", new OrderStats() {
                Total = _rand.Next(1, 9),
                Deleted = _rand.Next(1, 9),
                Staged = _rand.Next(1, 9)
            });
        }

        public string FilenameRoot { get; internal set; }
        public Action PublishStats { get; set; }

        private static object _lockObj = new object();

        public event StatsEventHandler StatsChanged;

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
                var filename  = FilenameRoot + "_" + string.Concat(trades[0].Algo.Split(Path.GetInvalidFileNameChars())) + ".csv";
                File.AppendAllText(csvFolder + filename, sb.ToString());
            }
            return true;
        } 
    }
}