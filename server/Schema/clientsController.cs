using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

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
    public class clientsController : ControllerBase
    {
        public DataAccessor DataAccess { get; private set; }

        public clientsController(DataAccessor dataAccess)
        {
            DataAccess = dataAccess;
        }

        [HttpGet()]
        public ActionResult<string[]> GetAll()
        {
            var clients = DataAccess.SchemaDictionary.Keys.ToList();
            clients.Sort();
            return clients.ToArray();
        }

    }
}
