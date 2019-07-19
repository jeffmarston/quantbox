using System;

namespace Eze.Quantbox
{
    public class AlgoStats
    {
        public string Name { get; set; }
        public int Routed { get; set; }
        public int Exceptions { get; set; }
        public int Created { get; set; }

        public AlgoStats(string name)
        {
            Name = name;
            Created = 0;
            Exceptions = 0;
            Routed = 0;
        }
    }
}