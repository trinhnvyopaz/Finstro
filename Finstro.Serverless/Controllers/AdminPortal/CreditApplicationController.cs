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

namespace Finstro.Serverless.API.Controllers.AdminPortal
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Policy = FinstroCustomPolicy.AdminUserPolicy.PolicyName)]
    public class CreditApplicationController : BaseController
    {

        private readonly CreditApplicationService creditApplicationService = new CreditApplicationService();

        /// <summary>
        ///   Get an Credit Application for the customer.
        /// </summary>
        /// <returns></returns>
        [Route("{UserId}/SaveResidentialAddress")]
        [HttpPost]
        [Authorize]
        public ActionResult<SaveBusinessDetailRequest> SaveResidentialAddress(string UserId, Serverless.Models.Dynamo.Address request)
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

        [Route("{UserId}/SaveDriversLicence")]
        [HttpPost]
        [Authorize]
        public ActionResult SaveDriversLicence(string UserId, Serverless.Models.Dynamo.DrivingLicence request)
        {
            try
            {
                var fccService = new FccServices(AuthorizationHeader);

                if (fccService.ValidadeDriverLicense(request.LicenceNumber, request.State))
                    creditApplicationService.SaveDriversLicence(UserId, request);
                else
                    return StatusCode(500, FinstroErrorType.FccServices.InvalidDriverLicense.ToFinstroError());

                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }

        [Route("{UserId}/SaveMedicare")]
        [HttpPost]
        [Authorize]
        public ActionResult SaveMedicare(string UserId, Serverless.Models.Dynamo.MedicareCard request)
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

        [Route("{UserId}/AddDocument")]
        [HttpPost]
        [Authorize]
        public ActionResult AddDocument(string UserId, [FromForm]AddDocumentRequest file)
        {
            try
            {

                if (Request.Form.Files.Count > 0)
                {
                    var item = file?.File[0];
                    file.File = Request.Form.Files;

                    var result = creditApplicationService.AddDocument(UserId, item.FileName, file.Name, file.Description, file.Type, item.OpenReadStream());

                    return Ok(result);
                }
                else
                {
                    throw FinstroErrorType.Schema.FileNotFound;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }
    }
}
