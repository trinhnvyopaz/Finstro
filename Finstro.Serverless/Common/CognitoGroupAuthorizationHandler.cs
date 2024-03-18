using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Finstro.Serverless.API.Common
{
    public class CognitoGroupAuthorizationHandler : AuthorizationHandler<CognitoGroupAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CognitoGroupAuthorizationRequirement requirement)
        {
            bool auth = false;

            foreach (var item in requirement.CognitoGroup.Split(','))
            {
                if (context.User.HasClaim(c => c.Type == "cognito:groups" &&
                                          c.Value == item.Trim()))
                {
                    auth = true;
                    break;
                }
                
            }

            if(auth)
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;

        }

    }
}
