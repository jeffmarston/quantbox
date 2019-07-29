using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eze.Quantbox
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        public AlgoMaster AlgoMaster { get; private set; }

        public ConfigurationController(AlgoMaster algoMaster)
        {
            AlgoMaster = algoMaster;
        }

        // GET api/configuration/algo/{algoName}
        [HttpGet("algos")]
        public ActionResult<List<AlgoMetadata>> Get()
        {
            var configs = from algo in AlgoMaster.Algos select algo.Metadata;
            return configs.ToList();
        }

        [HttpGet("algo/{algoName}")]
        public ActionResult<AlgoMetadata> Get(string algoName)
        {
            algoName = algoName.ToLower().Trim();
            var foundAlgo = AlgoMaster.Algos.Find(o => o.Name.ToLower().Trim() == algoName);
            if (foundAlgo == null)
            {
                return NotFound();
            }
            else
            {
                return foundAlgo.Metadata;
            }
        }

        // POST api/configuration/algo/{algoName}
        [HttpPost("algo/{algoName}")]
        public ActionResult<string> PostAlgoConfig(string algoName, [FromBody] AlgoMetadata config)
        {
            algoName = algoName.ToLower().Trim();
            var foundAlgo = AlgoMaster.Algos.Find(o => o.Name.ToLower().Trim() == algoName);
            if (foundAlgo == null)
            {
                if (string.IsNullOrWhiteSpace(config.Name))
                {
                    return BadRequest("Name not specified");
                }

                // Create a new one
                // config.Name = algoName;
                var newAlgo = AlgoMaster.CreateAlgo(config);
                newAlgo.PublishState();

                AlgoMaster.Save();
                return Accepted();
            }
            else
            {
                foundAlgo.Metadata = config;
                foundAlgo.Metadata.Enabled = foundAlgo.Enabled;
                AlgoMaster.Save();
                return Accepted();
            }
        }

        // DELETE api/configuration/algo/{algoName}
        [HttpDelete("algo/{algoName}")]
        public ActionResult<string> DeleteAlgoConfig(string algoName)
        {
            algoName = algoName.ToLower().Trim();
            var foundAlgo = AlgoMaster.Algos.Find(o => o.Name.ToLower().Trim() == algoName);
            if (foundAlgo == null)
            {
                return NotFound();
            }
            else
            {
                AlgoMaster.Algos.Remove(foundAlgo);
                AlgoMaster.Save();
                foundAlgo.PublishDelete();
                foundAlgo.Dispose();
                return Accepted();
            }
        }

        // GET api/configuration/ems
        [HttpGet("ems")]
        public ActionResult<EmsSettings> GetEmsConfig()
        {
            return AlgoMaster.EmsSettings;
        }

        // POST api/configuration/ems
        [HttpPost("ems")]
        public ActionResult<string> PostEmsConfig([FromBody] EmsSettings config)
        {
            try
            {
                AlgoMaster.EmsSettings = config;
                AlgoMaster.Save();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return Accepted();
        }


        // POST api/configuration/ems
        [HttpPost("adapter")]
        public ActionResult<string> PostAdapter([FromBody] string adapterToUse)
        {
            Console.WriteLine("Use " + adapterToUse);
            return Accepted();
        }
    }
}