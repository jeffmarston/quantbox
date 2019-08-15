using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eze.schema.extract
{
    public static class SqlConstants
    {
        public static readonly string SqlQuery = @"select name, type,
CHECKSUM(OBJECT_DEFINITION(object_id)) as [checksum], 
OBJECT_DEFINITION(object_id) as defintion
from sys.objects
where type in ('P', 'TR', 'U')";
    }
}
