using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Finstro.Serverless.Models.Request;
using Finstro.Serverless.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swagger.Net.Annotations;

namespace Finstro.Serverless.API.Controllers.AdminPortal
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PartnersController : ControllerBase
    {

        /// <summary>
        ///   Get list of partners for a user.
        /// </summary>
        /// <returns></returns>
        [Route("GetPartnersForUser")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, "List of partners", typeof(IEnumerable<PartnerForUser>))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(NotFoundResult))]
        public ActionResult<List<PartnerForUser>> GetPartnersForUser()
        {
            return Ok(new List<PartnerForUser>());
        }

    }
}
