using System;
using System.Diagnostics;

namespace Eze.Quantbox
{
    [DebuggerDisplay("{Date}  =  {Value}")]
    public class AlgoHistory
    {
        public DateTime Date { get; set; }
        public int Value { get; set; }
    }
}