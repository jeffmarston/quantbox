using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// POST /schema/*2019.4
//  POST /schema/Viking

//body = { version: '2019.4', objects: [...] } 

//GET versions
//GET schema/Viking
//GET schema/*2019.4

//GET objects/Viking/pr_GetSomething
//GET objects/*2019.4/pr_GetSomething

/// </summary>
namespace Eze.SchemaCompare
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchemaController : ControllerBase
    {
        public DataAccessor DataAccess { get; private set; }

        public SchemaController(DataAccessor dataAccess)
        {
            DataAccess = dataAccess;
        }

        [HttpGet("{client}")]
        public ActionResult<SchemaData> Get(string client)
        {
            client = client.ToLower().Trim();
            if (DataAccess.SchemaDictionary.ContainsKey(client))
            {
                var entry = DataAccess.SchemaDictionary[client];
                var result = new SchemaData()
                {
                    Client = entry.Client,
                    Version = entry.Version,
                    Objects = new List<DbObject>()
                };
                foreach (var obj in entry.Objects)
                {
                    result.Objects.Add(new DbObject()
                    {
                        Name = obj.Name,
                        Type = obj.Type,
                        CheckSum = obj.CheckSum
                    });
                }
                return result;
            }
            return NotFound();
        }

        [HttpGet("{client}/{objectName}")]
        public ActionResult<string> Get(string client, string objectName)
        {
            client = client.ToLower().Trim();
            objectName = objectName.ToLower().Trim();
            string result = null;
            if (DataAccess.SchemaDictionary.ContainsKey(client))
            {
                foreach (var obj in DataAccess.SchemaDictionary[client].Objects)
                {
                    if (obj.Name.ToLower() == objectName)
                    {
                        result = obj.Definition;
                        return result;
                    }
                }
            }
            return NotFound();
        }

        [HttpGet("{client}/dbobjects")]
        public ActionResult<string[]> GetDbObjects(string client)
        {
            client = client.ToLower().Trim();
            string result = null;
            if (DataAccess.SchemaDictionary.ContainsKey(client))
            {
                var objects = DataAccess.SchemaDictionary[client].Objects.Select(o => o.Name).ToList();
                objects.Sort();
                return objects.ToArray();
            }
            return NotFound();
        }

        [HttpGet("{client}/overview")]
        public ActionResult<object[]> GetDiff(string client)
        {
            client = client.ToLower().Trim();
            var result = new List<object>();
            if (DataAccess.SchemaDictionary.ContainsKey(client))
            {
                var clientSchema = DataAccess.SchemaDictionary[client];
                var version = clientSchema.Version;

                List<DbObject> standardObjects = null;
                if (DataAccess.SchemaDictionary.ContainsKey("*" + version))
                {
                    standardObjects = DataAccess.SchemaDictionary["*" + version].Objects.ToList();
                }
                if (standardObjects == null)
                {
                    return result.ToArray();
                }

                foreach (var clientObj in clientSchema.Objects)
                {
                    var match = standardObjects.Where(o => o.Name == clientObj.Name).FirstOrDefault();
                    if (match != null && match.CheckSum != clientObj.CheckSum)
                    {
                        dynamic item = new {
                            DbObject = clientObj.Name,
                            DbType = clientObj.Type,
                            Message = clientObj.Name + " differs from the standard. Lengths: " + clientObj.Definition.Length + " != " + match.Definition.Length
                        };
                        result.Add(item);
                    }
                }

                return result.ToArray();
            }
            return NotFound();
        }

        [HttpPost("{client}")]
        public ActionResult<string> PostClientData(string client, [FromBody] SchemaData schema)
        {
            client = client.ToLower().Trim();
            if (string.IsNullOrEmpty(client))
            {
                return BadRequest("Client not specified");
            }
            else
            {
                DataAccess.SchemaDictionary[client] = schema;
                DataAccess.Save();
                return Accepted();
            }
        }
    }
}
