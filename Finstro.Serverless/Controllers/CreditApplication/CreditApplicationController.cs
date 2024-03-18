using System;
using System.IO;
using Finstro.Serverless.Helper;
using Finstro.Serverless.Models;
using Finstro.Serverless.Models.Request;
using Finstro.Serverless.Models.Request.CreditApplication;
using FinstroServerless.Services.BankStatements;
using FinstroServerless.Services.Clients;
using FinstroServerless.Services.Common;
using FinstroServerless.Services.Fcc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finstro.Serverless.API.Controllers.Client
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class CreditApplicationController : BaseController
    {
        private readonly CreditApplicationService creditApplicationService = new CreditApplicationService();

        /// <summary>
        ///   Get an Credit Application for the customer.
        /// </summary>
        /// <returns></returns>
        [Route("AppRecovery")]
        [HttpPost]
        [Authorize]
        public ActionResult GetAppRecorevy(AppRecoveryRequest request = null)
        {
            try
            {
                if (request == null)
                    request = new AppRecoveryRequest() { ForceCardUpdate = false };

                string userId = this.UserId;
                var application = this.creditApplicationService.GetAppRecovery(userId, request.ForceCardUpdate);

                return Ok(application);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }

        /// <summary>
        ///   Get an Credit Application for the customer.
        /// </summary>
        /// <returns></returns>
        [Route("SaveBusinessDetail")]
        [HttpPost]
        [Authorize]
        public ActionResult<SaveBusinessDetailRequest> SaveBusinessDetail(Serverless.Models.Dynamo.BusinessDetails request)
        {
            try
            {

                var application = this.creditApplicationService.SaveBusinessDetail(UserId, request);

                return Ok(application);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }

        /// <summary>
        ///   Get an Credit Application for the customer.
        /// </summary>
        /// <returns></returns>
        [Route("SaveResidentialAddress")]
        [HttpPost]
        [Authorize]
        public ActionResult<SaveBusinessDetailRequest> SaveResidentialAddress(Serverless.Models.Dynamo.Address request)
        {
            try
            {

                var application = this.creditApplicationService.SaveResidentialAddress(UserId, request);

                return Ok(application);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }

        /// <summary>
        ///   Get an Credit Application for the customer.
        /// </summary>
        /// <returns></returns>
        [Route("SavePhoto")]
        [HttpPost]
        [Authorize]
        [RequestFormSizeLimit(valueCountLimit: 200000)]
        public ActionResult SavePhoto(SavePhotoRequest request)
        {
            try
            {
                foreach (var item in request.Photos)
                {
                    var bytes = Convert.FromBase64String(item.Base64Image);
                    var contents = new MemoryStream(bytes);

                    var url = creditApplicationService.UploadPhoto(UserId, item.Type, contents);

                }

                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }

        [Route("SaveMedicare")]
        [HttpPost]
        [Authorize]
        public ActionResult SaveMedicare(Serverless.Models.Dynamo.MedicareCard request)
        {
            try
            {

                creditApplicationService.SaveMedicare(UserId, request);

                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }

        [Route("SaveDriversLicence")]
        [HttpPost]
        [Authorize]
        public ActionResult SaveDriversLicence(Serverless.Models.Dynamo.DrivingLicence request)
        {
            try
            {
                var fccService = new FccServices(AuthorizationHeader);

                //if (fccService.ValidadeDriverLicense(request.LicenceNumber, request.State))
                    creditApplicationService.SaveDriversLicence(UserId, request);
                //else
                //    return StatusCode(500, FinstroErrorType.FccServices.InvalidDriverLicense.ToFinstroError());

                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }

        [Route("SaveThreatMetrixSessionID")]
        [HttpPost]
        [Authorize]
        public ActionResult SaveThreatMetrixSessionID(Serverless.Models.Dynamo.ThreatMetrix request)
        {
            try
            {

                creditApplicationService.SaveThreatMetrixSessionID(UserId, request);

                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }

        [Route("TriggerIdMatrixCheck")]
        [HttpPost]
        [Authorize]
        public ActionResult TriggerIdMatrixCheck()
        {
            try
            {
                ThreatMetrixService threatMetrixService = new ThreatMetrixService();

                var status = threatMetrixService.DeviceThreatMetrix(UserId);

                if (status != EnumThreatMetrixStatus.fail)
                    this.creditApplicationService.IDMatrixCheck(UserId);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }

        [Route("GetPaymentCredentials")]
        [HttpPost]
        [Authorize]
        public ActionResult GetPaymentCredentials()
        {
            try
            {
                // Implement TriggerIdMatrixCheck
                return Ok(new
                {
                    AppSettings.FinstroSettings.MerchantSuiteKey
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }

        [Route("SaveCreditCard")]
        [HttpPost]
        [Authorize]
        public ActionResult<Serverless.Models.Dynamo.CreditCardDetail> SaveCreditCard(Serverless.Models.Dynamo.CreditCardDetail request)
        {
            try
            {

                creditApplicationService.SaveCreditCard(UserId, request);

                return Ok(request);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }


        [Route("SaveDirectDebitAuthority")]
        [HttpPost]
        [Authorize]
        public ActionResult SirectDebitAuthority()
        {
            try
            {
                creditApplicationService.SaveDirectDebitAuthority(UserId);
                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }

        [Route("SaveSmallTerms")]
        [HttpPost]
        [Authorize]
        public ActionResult SaveSmallTerms()
        {
            try
            {
                creditApplicationService.SaveSmallTerms(UserId);
                

                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }

        [Route("SavePostalAddress")]
        [HttpPost]
        [Authorize]
        public ActionResult SavePostalAddress(GenericRequest addressType)
        {
            try
            {
                creditApplicationService.SavePostalAddress(UserId, addressType.Data);
                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }


        [Route("ProcessBankStatementFiles")]
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ProcessBankStatementFiles([FromForm]BankStatementFilesRequest files)
        {
            try
            {
                BankStatementService bankStatementService = new BankStatementService();

                if (Request.Form.Files.Count > 0)
                    files.Files = Request.Form.Files;

                files.FileCount = Request.Form.Files.Count;

                bankStatementService.ProcessBankStatement(files);

                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }

    }

   
}
