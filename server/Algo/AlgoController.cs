using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Eze.Quantbox.Environment
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
            // hack to publish shortly after it's requested, since the client doesn't want to update with the results returned from the REST call.
            var timer = new Timer();
            timer.Interval = 2000;
            timer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                foreach (var algo in AlgoMaster.Algos)
                {
                    algo.PublishState();
                }
                timer.Enabled = false;
            };
            timer.Enabled = true;

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
                foundAlgo.Enabled = enabled;
                foundAlgo.PublishState();
                foundAlgo.PublishToConsole((enabled ? "Enabled " : "Disabled ") + foundAlgo.Name);
                return Accepted();
            }
        }

    }
}