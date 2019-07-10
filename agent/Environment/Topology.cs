using System.IO;
using Newtonsoft.Json;

namespace Eze.AdminConsole.Environment
{
    public class Topology
    {
        public MachineSpec[] servers { get; set; }
        public MachineSpec[] clients { get; set; }
        public MachineSpec[] database { get; set; }
        public Topology()
        {
            servers = new MachineSpec[] {
                new MachineSpec("localhost")
            };
            clients = new MachineSpec[] {
                new MachineSpec("localhost")
            };
            database = new MachineSpec[] {
                new MachineSpec("localhost"),
                new MachineSpec("marston9020b")
            };
        }
    }

    public class MachineSpec
    {
        public string name { get; set; }

        public MachineSpec(string name)
        {
            this.name = name;
        }
    }
}