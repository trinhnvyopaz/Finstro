using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Finstro.Serverless.Helper;
using Finstro.Serverless.Models;
using Finstro.Serverless.Models.Request;
using Finstro.Serverless.Models.Request.Incc;
using Finstro.Serverless.Models.Response.Incc;
using FinstroServerless.Services.Clients;
using FinstroServerless.Services.Incc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finstro.Serverless.API.Controllers.Payment
{
    [Route("api/[controller]")]
    [ApiController]

    public class PaymentController : BaseController
    {
        private readonly InccService inccService = new InccService();
        private readonly CreditApplicationService creditApplicationService = new CreditApplicationService();

        #region Finstro Methods
        [Route("getPaymentAccount")]
        [HttpPost]
        [FisntroServiceCall]
        public ActionResult GetPaymentAccount(UserRequest request)
        {
            try
            {
                string type = creditApplicationService.GetPaymentAccount(request.UserId);

                return Ok(new { accountType = type });

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        #endregion

        #region INCC Methods

        [Route("Authorize")]
        [HttpPost]
        [FisntroServiceCall]
        public ActionResult GetPaymentAuthorization(GetAuthorizationRequest authorizationRequest)
        {
            try
            {

                GetAuthorizationResponse response = this.inccService.GetAuthorization(authorizationRequest);

                HttpContext.Response.Headers.Add("crt_card_token", response.CardToken);


                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }

        [Route("getsecutoken24")]
        [HttpPost]
        [FisntroServiceCall]
        public ActionResult GetShared24hToken()
        {
            try
            {
                return Ok(inccService.GetShared24hToken());

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        #endregion
    }
}