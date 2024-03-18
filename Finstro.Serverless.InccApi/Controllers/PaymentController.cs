using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finstro.Serverless.Helper;
using Finstro.Serverless.Models.Request.Incc;
using FinstroServerless.Services.Incc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Finstro.Serverless.InccApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly InccService inccService = new InccService();

        public PaymentController(ILogger<PaymentController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        [Route("HealthCheck")]
        public ActionResult HealthCheck()
        {
            var RemoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
            return Ok("OK!");
        }

        [HttpGet]
        [Route("Test")]
        public ActionResult Index()
        {
            var RemoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
            inccService.LogCall("Get-Test", RemoteIpAddress.MapToIPv4().ToString());
            return Ok("OK!");
        }

        [HttpPost]
        [Route("Test")]
        public ActionResult Test()
        {
            var RemoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
            inccService.LogCall("Post-Test", RemoteIpAddress.MapToIPv4().ToString());
            return Ok("OK!");
        }



        #region INCC Methods

        [Route("Authorize")]
        [HttpPost]
        [FisntroServiceCall]
        public ActionResult GetPaymentAuthorization(GetAuthorizationRequest authorizationRequest)
        {
            try
            {

                var RemoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;

                inccService.LogCall("Get Authorization", RemoteIpAddress.MapToIPv4().ToString());
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
        public ActionResult GetShared24hToken()
        {
            try
            {
                var RemoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;

                inccService.LogCall("Get Token", RemoteIpAddress.MapToIPv4().ToString());

                return Ok(inccService.GetShared24hToken(RemoteIpAddress.MapToIPv4().ToString()));

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        #endregion

    }
}
