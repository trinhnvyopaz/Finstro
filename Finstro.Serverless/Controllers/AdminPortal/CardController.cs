using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finstro.Serverless.Helper;
using Finstro.Serverless.Models.Request;
using Finstro.Serverless.Models.Request.Incc;
using Finstro.Serverless.Models.Response;
using FinstroServerless.Services.Clients;
using FinstroServerless.Services.Incc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finstro.Serverless.API.Controllers.AdminPortal
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize]
    public class CardController : BaseController
    {


        private readonly InccService inccService = new InccService();

        /// <summary>
        ///   Get list of current clients.
        /// </summary>
        /// <param name="request">Object carrying the UserID</param>
        /// <returns></returns>
        //[SwaggerResponse(HttpStatusCode.OK, "List of clients", typeof(IEnumerable<ClientListResponse>))]
        //[SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(NotFoundResult))]
        [Route("CancelCard")]
        [HttpPost]
        [Authorize(Policy = FinstroCustomPolicy.AdminUserPolicy.PolicyName)]
        public ActionResult CancelCard(CardActionRequest request)
        {
            try
            {
                this.inccService.CancelCard(request);

                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }


        /// <summary>
        ///   Get list of current clients.
        /// </summary>
        /// <param name="request">Object carrying the ReplaceCard</param>
        /// <returns></returns>
        [Route("ReplaceCard")]
        [HttpPost]
        [Authorize(Policy = FinstroCustomPolicy.AdminUserPolicy.PolicyName)]
        public ActionResult ReplaceCard(CardActionRequest request)
        {
            try
            {
                this.inccService.ReplaceCard(request);

                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }
        /// <summary>
        ///   Get list of current clients.
        /// </summary>
        /// <param name="request">Object carrying the ReplaceCard</param>
        /// <returns></returns>
        [Route("RenewCard")]
        [HttpPost]
        [Authorize(Policy = FinstroCustomPolicy.AdminUserPolicy.PolicyName)]
        public ActionResult RenewCard(CardActionRequest request)
        {
            try
            {
                this.inccService.UpdateCard(request, true);

                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }

        /// <summary>
        ///   Get list of current clients.
        /// </summary>
        /// <param name="request">Object carrying the ReplaceCard</param>
        /// <returns></returns>
        [Route("UpdateCard")]
        [HttpPost]
        [Authorize(Policy = FinstroCustomPolicy.AdminUserPolicy.PolicyName)]
        public ActionResult UpdateCard(CardActionRequest request)
        {
            try
            {
                this.inccService.UpdateCard(request, false);

                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }

    }
}