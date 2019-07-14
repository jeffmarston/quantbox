using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eze.Quantbox
{
    public class QuantBoxConfig
    {
        public IEnumerable<AlgoMetadata> Metadata { get; set; }
        public string EmsSettings { get; set; }

    }
}
