using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Eze.AdminConsole.Environment
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnvironmentController : ControllerBase
    {
        // GET api/environment
        [HttpGet]
        public ActionResult<Topology> Get()
        {
            // var envTopology = new Topology();
            var envTopology = Load();
            return envTopology;
        }

        // PUT api/environment
        [HttpPut()]
        public void Put([FromBody] Topology topology)
        {
            Save(topology);
        }

        public Topology Load()
        {
            string folderbase = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            string filePath = System.IO.Path.Combine(folderbase, "Eze", "Admin", "environment.json");
            if (System.IO.File.Exists(filePath)) {
                string txt = System.IO.File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<Topology>(txt);
            }
            return new Topology();
        }

        private void Save(Topology topology)
        {
            string json = JsonConvert.SerializeObject(topology);
            string folderbase = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            string folderPath = System.IO.Path.Combine(folderbase, "Eze", "Admin");
            // If directory doesn't exist, create it
            System.IO.Directory.CreateDirectory(folderPath);
            System.IO.File.WriteAllText(System.IO.Path.Combine(folderPath, "environment.json"), json);
        }
    }
}