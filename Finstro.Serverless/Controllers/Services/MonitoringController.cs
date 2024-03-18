using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinstroServerless.Services.Common;
using Microsoft.AspNetCore.Mvc;

namespace Finstro.Serverless.API.Controllers.Services
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonitoringController : BaseController
    {

        [HttpPost]
        [Route("heartbeat")]
        public ActionResult Heartbeat()
        {
            MonitoringService.LogHeartbeat();
            return Ok(new { Date = DateTime.UtcNow });
        }


    }
}