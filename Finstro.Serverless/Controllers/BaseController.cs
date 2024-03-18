using System;
using System.Linq;
using System.Security.Claims;
using FinstroServerless.Services.IdentityProvider;
using Microsoft.AspNetCore.Mvc;

namespace Finstro.Serverless.API.Controllers
{
    public class BaseController : ControllerBase
    {
        public string UserId
        {
            get
            {

                return User.Claims.FirstOrDefault(c => c.Type == CognitoAttribute.UserSub.AttributeName).Value;
            }
        }

        public string AuthorizationHeader
        {
            get {
                if (Request.Headers["Authorization"].Count > 0)
                    return Request.Headers["Authorization"].ToString();
                else
                    return "";
            }
        }
    }
}
