using System.Collections.Generic;

namespace Eze.Quantbox
{
    public class AlgoMetadata
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public List<string> Symbols { get; set; }
        public int BuyShortRatio { get; set; }
        public int MinBatchSize { get; set; }
        public int MaxBatchSize { get; set; }
        public int FrequencySec { get; set; }

        public AlgoMetadata(string name)
        {
            Name = name;
            Enabled = false;
            Symbols = new List<string>() { "AAPL", "BBY", "COKE", "DELL", "ENR", "F", "GOOG", "HD" };
            BuyShortRatio = 50;
            MinBatchSize = 1;
            MaxBatchSize = 20;
            FrequencySec = 2;
        }
    }
}