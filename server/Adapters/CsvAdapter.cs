using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace Eze.Quantbox
{
    public class CsvAdapter: ITradingSystemAdapter
    {
        private Random _rand = new Random();
        private Timer _timer = new Timer();
        private Dictionary<string, OrderStats> _algoStats = new Dictionary<string, OrderStats>();
        public EmsSettings Settings { get; set; }
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
            foreach (var algoStat in _algoStats)
            {
                var stats = algoStat.Value;

                if (_rand.Next(0, 10) < 1) { stats.Staged = Math.Min(stats.Total, stats.Staged + _rand.Next(1, 9)); }
                if (_rand.Next(0, 10) < 1) { stats.Manual = Math.Min(stats.Total - stats.AutoRouted, stats.Manual + _rand.Next(1, 5)); }
                if (_rand.Next(0, 2) < 1) { stats.AutoRouted = Math.Min(stats.Total - stats.Manual, stats.AutoRouted + _rand.Next(1, 10)); }
                // if (_rand.Next(0, 10) < 1) { stats.Deleted = Math.Min(stats.Total - stats.Completed, stats.Deleted + _rand.Next(1, 10)); }
                stats.Completed = stats.Manual + stats.AutoRouted;
                stats.Working = stats.Total - stats.Completed; 

                stats.CompletedPct = (stats.Total == 0) ? 0 : Math.Truncate(stats.Completed * 10d / (stats.Total)) * 10;
                stats.CompletedValue = stats.Completed * 2000 *  22.55; //arbitrary amount and price

                StatsChanged?.Invoke(algoStat.Key, stats);
            }
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

            if (!_algoStats.ContainsKey(trades[0].Algo)) {
                _algoStats.Add(trades[0].Algo, new OrderStats());
            }

            _algoStats[trades[0].Algo].Total += trades.Count;

            StringBuilder sb = new StringBuilder();
            foreach (var trade in trades)
            {
                _algoStats[trades[0].Algo].TotalQty += trade.Amount;
                _algoStats[trades[0].Algo].TotalValue += trade.Amount * 22.55; // Assumes every stock is worth $22.55

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