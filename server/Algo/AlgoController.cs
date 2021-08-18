using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Eze.Quantbox
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlgoController : ControllerBase
    {
        public AlgoMaster AlgoMaster { get; private set; }

        public AlgoController(AlgoMaster algoMaster)
        {
            AlgoMaster = algoMaster;
        }


        // GET api/algo
        [HttpGet]
        public ActionResult<List<AbstractAlgoModel>> Get()
        {
            return AlgoMaster.Algos;
        }

        // POST api/algo/{algoName}/enabled
        [HttpPost("{algoName}/enabled")]
        public ActionResult<string> PostEnabled(string algoName, [FromBody] Boolean enabled)
        {
            algoName = algoName.ToLower().Trim();
            var foundAlgo = AlgoMaster.Algos.Find(o => o.Name.ToLower().Trim() == algoName);
            if (foundAlgo == null)
            {
                return NotFound();
            }
            else
            {
                foundAlgo.Metadata.Enabled = enabled;
                AlgoMaster.Save();

                foundAlgo.Enabled = enabled;
                foundAlgo.PublishState();
                foundAlgo.PublishToConsole((enabled ? "Enabled " : "Disabled ") + foundAlgo.Name);
                return Accepted();
            }
        }

        // POST api/algo/{algoName}/enabled
        [HttpPost("{algoName}/cancel")]
        public ActionResult<string> PostCancelOrders(string algoName)
        {
            algoName = algoName.ToLower().Trim();
            var foundAlgo = AlgoMaster.Algos.Find(o => o.Name.ToLower().Trim() == algoName);
            if (foundAlgo == null)
            {
                return NotFound();
            }
            else
            {
                foundAlgo.Adapter.CancelAllOrders(foundAlgo.Name);
                return Accepted();
            }
        }

        // POST api/algo/{algoName}/enabled
        [HttpPost("{algoName}/recalculate")]
        public ActionResult<string> PostRecalculate(string algoName, [FromBody] Boolean enabled)
        {
            algoName = algoName.ToLower().Trim();
            var foundAlgo = AlgoMaster.Algos.Find(o => o.Name.ToLower().Trim() == algoName);
            if (foundAlgo == null)
            {
                return NotFound();
            }
            else
            {
                foundAlgo.Adapter.StatsRecalcNeeded(foundAlgo.Name);
                return Accepted();
            }
        }
    }
}