using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Finstro.Serverless.API.Models;
using Finstro.Serverless.Helper;
using Finstro.Serverless.Models;
using Finstro.Serverless.Models.Request;
using Finstro.Serverless.Models.Request.CreditApplication;
using Finstro.Serverless.Models.Response;
using FinstroServerless.Services.Clients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swagger.Net.Annotations;

namespace Finstro.Serverless.API.Controllers.Client
{

	[Route("api/[controller]")]
	[ApiController]
	[Authorize]

	public class ClientController : BaseController
    {
		private readonly ClientService service = new ClientService();
        private readonly CreditApplicationService  creditApplicationService = new CreditApplicationService();

        /// <summary>
        ///   Get list of current clients.
        /// </summary>
        /// <param name="request">Object carrying the UserID</param>
        /// <returns></returns>
        //[SwaggerResponse(HttpStatusCode.OK, "List of clients", typeof(IEnumerable<ClientListResponse>))]
        //[SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(NotFoundResult))]
        [Route("GetClientList")]
		[HttpPost]
		[Authorize (Policy = FinstroCustomPolicy.AdminUserPolicy.PolicyName)]
        public ActionResult<List<ClientListResponse>> GetClientList(SearchRequestModel request)
		{
			try
			{
				var list = this.service.GetClientList(request);

				return Ok(list);
				
			}
			catch (BaseCustomException ex)
			{
				return StatusCode(500, ex);
			}

		}

        /// <summary>
		///   Get list of current clients.
		/// </summary>
		/// <param name="request">Object carrying the UserID</param>
		/// <returns></returns>
		[Route("GetCientDetailsByID/{id}")]
        [HttpPost]
        [Authorize]
        public ActionResult<List<ClientListResponse>> GetClientDetailsByID(string id)
        {
            try
            {
                var list = this.creditApplicationService.GetCreditApplication(id);

                return Ok(list);

            }
            catch (BaseCustomException ex)
            {
                return StatusCode(500, ex);
            }

        }

    }
}
