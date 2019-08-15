using System.Collections.Generic;

namespace Eze.SchemaCompare
{
    public class SchemaData
    {
        public string Version { get; set; }
        public List<DbObject> Objects { get; set; }
        public string Client { get; set; }

        public SchemaData()
        {
            Objects = new List<DbObject>();
        }
    }

    public class DbObject
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int CheckSum { get; set; }
        public string Definition { get; set; }
    }
}