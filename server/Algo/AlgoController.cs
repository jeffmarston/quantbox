using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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