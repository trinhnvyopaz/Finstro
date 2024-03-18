using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Finstro.Serverless.API.Models;
using Finstro.Serverless.Helper;
using Finstro.Serverless.Models;
using Finstro.Serverless.Models.Request;
using Finstro.Serverless.Models.Request.CreditApplication;
using Finstro.Serverless.Models.Response;
using FinstroServerless.Services.Clients;
using FinstroServerless.Services.IdentityProvider;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swagger.Net.Annotations;

namespace Finstro.Serverless.API.Controllers.Authentication
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class UserController : BaseController
    {


        private readonly UserService userService = new UserService();


        [Route("SaveFCMToken")]
        [HttpPost]
        [Authorize]
        public ActionResult SaveFCMToken(GenericRequest token)
        {
            try
            {
                userService.SaveFCMToken(UserId, token.Data);
                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }


        [Route("RemoveUser/{phoneNumber}")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> RemoveUser(string phoneNumber)
        {
            try
            {

                AmazonCognitoIdentityProviderClient cognito;

                cognito = new AmazonCognitoIdentityProviderClient(RegionEndpoint.USEast2);
                var cognitoUser = await userService.GetUserByAttribute(CognitoAttribute.PhoneNumber, "+" + phoneNumber).ConfigureAwait(false);

                if(cognitoUser == null)
                    cognitoUser = await userService.GetUserByAttribute(CognitoAttribute.Email, phoneNumber).ConfigureAwait(false);

                var delete = cognito.AdminDeleteUserAsync(new AdminDeleteUserRequest()
                {
                    Username = cognitoUser.Username,
                    UserPoolId = AppSettings.Cognito.PoolId
                });


                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToFinstroError());
            }

        }
    }
}
