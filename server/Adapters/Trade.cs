using System;

namespace Eze.Quantbox
{
    public class Trade
    {
        public string Symbol { get; set; }
        public string Side { get; set; }
        public int Amount { get; set; }
        public string Trader { get; set; }
        public string Manager { get; set; }
        public string Algo { get; internal set; }

        public override string ToString()
        {
            return $"(Symbol={Symbol}, Symbol={Side}, Symbol={Amount})";
        }
    }
}