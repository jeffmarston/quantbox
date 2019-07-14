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
                return NotFound();
            }
            else
            {
                foundAlgo.Metadata = config;
                // Don't overwrite enabled
                foundAlgo.Metadata.Enabled = foundAlgo.Enabled;

                AlgoMaster.Save();
                return Accepted();
            }
        }
    }
}