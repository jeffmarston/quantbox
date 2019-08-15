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
        [HttpGet()]
        public ActionResult<QuantBoxConfig> GetConfig()
        {
            return new QuantBoxConfig()
            {
                ActiveAdapter = AlgoMaster.ActiveAdapter,
                EmsSettings = AlgoMaster.EmsSettings
            };
        }

        // POST api/configuration/ems
        [HttpPost()]
        public ActionResult<string> PostConfig([FromBody] QuantBoxConfig config)
        {
            try
            {
                AlgoMaster.ActiveAdapter = config.ActiveAdapter;
                AlgoMaster.EmsSettings = config.EmsSettings;
                AlgoMaster.Save();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return Accepted();
        }

        // POST api/configuration/adapter
        //[HttpPost("adapter/{adapter}")]
        //public ActionResult<string> PostActiveAdapter(string adapter)
        //{
        //    if (adapter != "EMS" && adapter != "CSV")
        //    {
        //        return NotFound();
        //    }

        //    try
        //    {
        //        AlgoMaster.SetActiveAdapter(adapter);
        //        AlgoMaster.Save();
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //    return Accepted();
        //}

        //// GET api/configuration/ems
        //[HttpGet("adapter")]
        //public ActionResult<string> GetAdapter()
        //{
        //    return AlgoMaster.GetActiveAdapter();
        //}
    }
}