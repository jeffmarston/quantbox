using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Eze.AdminConsole.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Eze.AdminConsole.Environment
{
    [Route("api/services")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private IHubContext<ServiceMgmtHub> _context;
        private Topology _topology = new Topology();

        public ServicesController(IHubContext<ServiceMgmtHub> context)
        {
            _context = context;
        }
        // GET api/values
        [HttpGet()]
        public ActionResult<IEnumerable<Service>> Get()
        {
            try
            {
                var svcs = ServiceUtils.GetAllEzeServices();
                return svcs.ToArray();
            }
            catch (Exception)
            {
                return new Service[] { };
            }
        }

        // POST api/services/logs
        [HttpGet("{svcName}/logs")]
        public async Task<IActionResult> Logs(string svcName)
        {
            var svcs = ServiceUtils.GetAllEzeServices();
            var svc = svcs.Find(o => o.name == svcName);

            var DbRunner = new DatabaseRunner();
            var obj = DbRunner.GetServiceLogPath();


            var filePath = svc.path;
            Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            if (stream == null)
                return NotFound(); // returns a NotFoundResult with Status404NotFound response.
            return File(stream, "application/octet-stream"); // returns a FileStreamResult
        }

        // POST api/services/start
        [HttpPost("start/{svcName}")]
        public void Start(string svcName)
        {
            ServiceUtils.StartService(svcName);
        }

        // POST api/services/stop
        [HttpPost("stop/{svcName}")]
        public void Stop(string svcName)
        {
            ServiceUtils.StopService(svcName);
        }
    }
}