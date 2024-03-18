using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finstro.Serverless.Helper;
using Finstro.Serverless.Models.Request;
using Finstro.Serverless.Models.Response;
using FinstroServerless.Services.Clients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finstro.Serverless.API.Controllers.AdminPortal
{

    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientController : BaseController
    {
        private readonly CreditApplicationService creditApplicationService = new CreditApplicationService();


        [Route("Application/{id}")]
        [HttpPost]
        [Authorize]
        public ActionResult GetCreditApplication(string id)
        {
            try
            {
                var application = this.creditApplicationService.GetCreditApplication(id);

                return Ok(application);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }


        /// <summary>
        ///   Get list of current clients.
        /// </summary>
        /// <param name="request">Object carrying the UserID</param>
        /// <returns></returns>
        //[SwaggerResponse(HttpStatusCode.OK, "List of clients", typeof(IEnumerable<ClientListResponse>))]
        //[SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(NotFoundResult))]
        [Route("Applications")]
        [HttpPost]
        [Authorize(Policy = FinstroCustomPolicy.AdminUserPolicy.PolicyName)]
        public ActionResult<List<ClientListResponse>> GetClientList(SearchRequestModel request)
        {
            try
            {
                var list = this.creditApplicationService.GetCreditApplications();

                return Ok(list);

            }
            catch (BaseCustomException ex)
            {
                return StatusCode(500, ex);
            }

        }
    }
}