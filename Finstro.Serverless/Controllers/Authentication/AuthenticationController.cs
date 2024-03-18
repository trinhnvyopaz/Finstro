using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.S3;
using Finstro.Serverless.API.Models;
using Finstro.Serverless.Helper;
using Finstro.Serverless.Models;
using Finstro.Serverless.Models.Request.User;
using FinstroServerless.Services.IdentityProvider;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Finstro.Serverless.API.Controllers.Authentication
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private string _clientId = AppSettings.Cognito.ClientId;
        private string _userPoolId = AppSettings.Cognito.PoolId;
        private readonly RegionEndpoint _region = RegionEndpoint.USEast2;
        private AmazonCognitoIdentityProviderClient cognito;
        private static UserService _userService;

        public AuthenticationController()
        {
            cognito = new AmazonCognitoIdentityProviderClient(_region);
            _userService = new UserService();
        }

        [HttpPost]
        [Route("api/Authentication/CreateAccount")]
        public async Task<ActionResult> Register(CreateUserRequest user)
        {
            try
            {
                await _userService.CreateFinstroUser(user);

                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToFinstroError());
            }

        }




        [HttpPost]
        [Route("api/Authentication/ValidateVerificationCode")]
        public async Task<ActionResult<AuthResponseModel>> ValidateVerificationCode(ValidateVerificationCodeRequest user)
        {

            try
            {

                user.MobilePhoneNumber = ValidationHelper.FormatPhoneNumber(user.MobilePhoneNumber);

                var cognitoUser = await _userService.GetUserByAttribute(CognitoAttribute.PhoneNumber, user.MobilePhoneNumber);

                if (cognitoUser == null)
                    return BadRequest(FinstroError.GetErrors(FinstroErrorType.User.UserNotFound));

                // if (cognitoUser.UserStatus == UserStatusType.CONFIRMED)
                //     return BadRequest(new BaseCustomException("User already validated", "User already validated", 1001));


                string username = cognitoUser.Username;
                string userEmail = cognitoUser.Attributes.FirstOrDefault(a => a.Name == CognitoAttribute.PreferredUsername.AttributeName).Value;

                ConfirmSignUpRequest confirmRequest = new ConfirmSignUpRequest()
                {
                    Username = username,
                    ClientId = _clientId,
                    ConfirmationCode = user.VerificationCode
                };


                //UNCOMMENT
                ConfirmSignUpResponse confirmResult = await cognito.ConfirmSignUpAsync(confirmRequest);


                if (confirmResult.HttpStatusCode == HttpStatusCode.OK)
                {
                    string password = await CreateAccessCode(user, cognitoUser, userEmail);

                    var loginRequest = new AdminInitiateAuthRequest
                    {
                        UserPoolId = _userPoolId,
                        ClientId = _clientId,
                        AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH
                    };


                    loginRequest.AuthParameters.Add("USERNAME", userEmail);
                    loginRequest.AuthParameters.Add("PASSWORD", password);


                    var loginResponse = await cognito.AdminInitiateAuthAsync(loginRequest);

                    return Ok(new AuthResponseModel()
                    {
                        AccessToken = loginResponse.AuthenticationResult.IdToken,
                        ExpiresIn = loginResponse.AuthenticationResult.ExpiresIn,
                        RefreshToken = loginResponse.AuthenticationResult.RefreshToken,
                    });


                }
                else
                {
                    return BadRequest(FinstroError.GetErrors(FinstroErrorType.Auth.InvalidCode));
                }


            }
            catch (Exception ex)
            {
                var err = FinstroErrorType.Auth.InvalidCode;
                err.Description = ex.Message;

                return BadRequest(FinstroError.GetErrors(err));
            }

        }


        [HttpPost]
        [Route("api/Authentication/ResendAccessCode")]
        public async Task<ActionResult> ResendAccessCode(ValidateVerificationCodeRequest user)
        {

            try
            {
                UserType cognitoUser = null;



                if (!string.IsNullOrEmpty(user.MobilePhoneNumber))
                {
                    user.MobilePhoneNumber = ValidationHelper.FormatPhoneNumber(user.MobilePhoneNumber);
                    cognitoUser = await _userService.GetUserByAttribute(CognitoAttribute.PhoneNumber, user.MobilePhoneNumber);
                }

                if (cognitoUser == null && !string.IsNullOrEmpty(user.EmailAddress))
                    cognitoUser = await _userService.GetUserByAttribute(CognitoAttribute.Email, user.EmailAddress);

                if (cognitoUser == null)
                    return BadRequest(FinstroErrorType.User.UserNotFound.ToFinstroError());

                if (!string.IsNullOrEmpty(user.EmailAddress) && !string.IsNullOrEmpty(user.MobilePhoneNumber))
                {
                    if (cognitoUser.Attributes.FirstOrDefault(a => a.Name == CognitoAttribute.PreferredUsername.AttributeName).Value != user.EmailAddress ||
                       cognitoUser.Attributes.FirstOrDefault(a => a.Name == CognitoAttribute.PhoneNumber.AttributeName).Value != user.MobilePhoneNumber)
                    {
                        return BadRequest(FinstroErrorType.User.UserNotFound.ToFinstroError());
                    }
                }

                if (cognitoUser.UserStatus == UserStatusType.UNCONFIRMED)
                    return BadRequest(FinstroErrorType.User.UserNotConfirmed.ToFinstroError());

                if (string.IsNullOrEmpty(user.EmailAddress))
                {
                    user.EmailAddress = cognitoUser.Attributes.FirstOrDefault(a => a.Name == CognitoAttribute.PreferredUsername.AttributeName).Value;
                }



                string userEmail = user.EmailAddress;



                string password = await CreateAccessCode(user, cognitoUser, userEmail);

                if (!string.IsNullOrEmpty(user.MobilePhoneNumber))
                {
                    await _userService.SendSMSAccessCode(cognitoUser, password);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToFinstroError());
            }

        }


        private async Task<string> CreateAccessCode(ValidateVerificationCodeRequest user, UserType cognitoUser, string userEmail)
        {
            List<AttributeType> userAttrs = new List<AttributeType>();
            userAttrs.Add(new AttributeType() { Name = CognitoAttribute.PreferredUsername.AttributeName, Value = user.EmailAddress });

            await cognito.AdminUpdateUserAttributesAsync(new AdminUpdateUserAttributesRequest()
            {
                Username = cognitoUser.Username,
                UserPoolId = _userPoolId,
                UserAttributes = userAttrs
            });


            var newPassword = UserService.GenerateCode();

            await _userService.SendAccessCode(cognitoUser, newPassword);

            return newPassword;
        }

        [HttpPost]
        [Route("api/Authentication/ResendVerificationCode")]
        public async Task<ActionResult<IdentityResult>> ResendVerificationCode(ValidateVerificationCodeRequest user)
        {
            UserType cognitoUser = null;

            try
            {
                user.MobilePhoneNumber = ValidationHelper.FormatPhoneNumber(user.MobilePhoneNumber);

                if (!string.IsNullOrEmpty(user.MobilePhoneNumber))
                    cognitoUser = await _userService.GetUserByAttribute(CognitoAttribute.PhoneNumber, user.MobilePhoneNumber);

                if (cognitoUser == null && !string.IsNullOrEmpty(user.EmailAddress))
                    cognitoUser = await _userService.GetUserByAttribute(CognitoAttribute.Email, user.EmailAddress);

                if (cognitoUser == null)
                    return BadRequest(FinstroError.GetErrors(FinstroErrorType.User.UserNotFound));




                ResendConfirmationCodeRequest resendRequest = new ResendConfirmationCodeRequest()
                {
                    Username = cognitoUser.Username,
                    ClientId = _clientId,
                };

                //UNCOMMENT
                var confirmResult = await cognito.ResendConfirmationCodeAsync(resendRequest);

                return Ok();
            }
            catch (AmazonCognitoIdentityProviderException ex)
            {
                return BadRequest(ex.ToFinstroError());
            }
        }


        [HttpPost]
        [Route("api/Authentication/CurrentUserDetails")]
        [Authorize]
        public async Task<ActionResult<CreateUserRequest>> CurrentUserDetails()
        {
            try
            {


                ClaimsPrincipal user = this.User;

                string userName = user.FindFirstValue("cognito:username");
                var cognitoUser = await _userService.GetUserByAttribute(CognitoAttribute.UserName, userName);


                CreateUserRequest result = new CreateUserRequest()
                {
                    EmailAddress = user.FindFirstValue("preferred_username"),
                    FamilyName = cognitoUser.Attributes.FirstOrDefault(a => a.Name == CognitoAttribute.FamilyName.AttributeName)?.Value,
                    FirstGivenName = cognitoUser.Attributes.FirstOrDefault(a => a.Name == CognitoAttribute.GivenName.AttributeName)?.Value,
                    MobilePhoneNumber = user.FindFirstValue("phone_number"),
                    Accepted = cognitoUser.Attributes.FirstOrDefault(a => a.Name == CognitoAttribute.TermsAccepted.AttributeName)?.Value == "1"

                };

                return Ok(result);
            }
            catch (AmazonCognitoIdentityProviderException ex)
            {
                return BadRequest(ex.ToFinstroError());
            }
        }


        [HttpPost]
        [Route("api/Auth/Admin/Register")]
        public async Task<ActionResult> Register(UserModel user)
        {

            user.PhoneNumber = ValidationHelper.FormatPhoneNumber(user.PhoneNumber);

            if (string.IsNullOrEmpty(user.Username))
                user.Username = user.Email;


            var request = new SignUpRequest
            {
                ClientId = _clientId,
                Password = user.Password,
                Username = user.Username,
            };

            var emailAttribute = new AttributeType
            {
                Name = "email",
                Value = user.Email
            };
            request.UserAttributes.Add(emailAttribute);

            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                var phoneAttribute = new AttributeType
                {
                    Name = CognitoAttribute.PhoneNumber.AttributeName,
                    Value = user.PhoneNumber
                };
                request.UserAttributes.Add(phoneAttribute);
            }

            try
            {
                var response = await cognito.SignUpAsync(request);

                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToFinstroError());
            }

        }

        [HttpPost]
        [Route("api/Auth/Admin/SignIn")]
        [Route("api/Authentication/SignIn")]
        public async Task<ActionResult<AuthResponseModel>> SignIn(UserModel user)
        {

            var request = new AdminInitiateAuthRequest
            {
                UserPoolId = _userPoolId,
                ClientId = _clientId,
                AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH
            };

            request.AuthParameters.Add("USERNAME", user.Username);
            request.AuthParameters.Add("PASSWORD", user.Password);

            try
            {

                var response = await cognito.AdminInitiateAuthAsync(request);
                return Ok(new AuthResponseModel()
                {
                    AccessToken = response.AuthenticationResult.IdToken,
                    ExpiresIn = response.AuthenticationResult.ExpiresIn,
                    RefreshToken = response.AuthenticationResult.RefreshToken,
                });

            }
            catch (Exception ex)
            {
                if (ex is UserNotConfirmedException)
                    return BadRequest(FinstroErrorType.User.UserNotConfirmed.ToFinstroError());

                return BadRequest(ex.ToFinstroError());
            }
        }


        [HttpPost]
        [Route("api/Auth/Admin/RefreshToken")]
        [Route("api/Authentication/RefreshToken")]
        public async Task<ActionResult<AuthResponseModel>> RefreshToken(RefreshTokenModel refreshToken)
        {


            try
            {
                var response = await _userService.RefreshToken(refreshToken.RefreshToken);

                if (string.IsNullOrEmpty(response.AuthenticationResult.RefreshToken))
                    response.AuthenticationResult.RefreshToken = refreshToken.RefreshToken;

                return Ok(new AuthResponseModel()
                {
                    AccessToken = response.AuthenticationResult.IdToken,
                    ExpiresIn = response.AuthenticationResult.ExpiresIn,
                    RefreshToken = response.AuthenticationResult.RefreshToken,
                });


            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToFinstroError());
            }


        }



        [HttpPost]
        [Route("api/Auth/Admin/ConfirmEmail")]
        public async Task<ActionResult<IdentityResult>> ConfirmEmail(UserModel user)
        {

            try
            {
                ConfirmSignUpRequest confirmRequest = new ConfirmSignUpRequest()
                {
                    Username = user.Username,
                    ClientId = _clientId,
                    ConfirmationCode = user.ConfirmationCode
                };


                //UNCOMMENT
                var confirmResult = await cognito.ConfirmSignUpAsync(confirmRequest);


                var loginRequest = new AdminInitiateAuthRequest
                {
                    UserPoolId = _userPoolId,
                    ClientId = _clientId,
                    AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH
                };


                return IdentityResult.Success;
            }
            catch (AmazonCognitoIdentityProviderException ex)
            {
                return BadRequest(ex.ToFinstroError());
            }




        }

        [HttpPost]
        [Route("api/Auth/Admin/ConfirmAccount")]
        public async Task<ActionResult<IdentityResult>> Confirm(UserModel user)
        {


            user.PhoneNumber = ValidationHelper.FormatPhoneNumber(user.PhoneNumber);

            string attributeName = CognitoAttribute.PhoneNumber.AttributeName;

            //var username = User.Claims.FirstOrDefault(c => c.Type == "cognito:username").Value;
            var attributes = new List<string>();
            attributes.Add(CognitoAttribute.UserName.AttributeName);

            var request = new ListUsersRequest()
            {
                //AttributesToGet = attributes,
                Filter = $"{CognitoAttribute.PhoneNumber.AttributeName} = \"{user.PhoneNumber}\"",
                UserPoolId = _userPoolId,
                Limit = 10

            };
            try
            {
                var users = await cognito.ListUsersAsync(request);

                if (users.Users.Count == 0)
                    throw new ArgumentException(string.Format("Phone Number not fount"));

                string username = users.Users.FirstOrDefault().Username;

                if (attributeName != CognitoAttribute.PhoneNumber.AttributeName && attributeName != CognitoAttribute.Email.AttributeName)
                {
                    throw new ArgumentException(string.Format("Invalid attribute name, only {0} and {1} can be verified", CognitoAttribute.PhoneNumber, CognitoAttribute.Email), nameof(attributeName));
                }


                ConfirmSignUpRequest confirmRequest = new ConfirmSignUpRequest()
                {
                    Username = username,
                    ClientId = _clientId,
                    ConfirmationCode = user.ConfirmationCode
                };


                //UNCOMMENT
                var confirmResult = await cognito.ConfirmSignUpAsync(confirmRequest);



                var loginRequest = new AdminInitiateAuthRequest
                {
                    UserPoolId = _userPoolId,
                    ClientId = _clientId,
                    AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH
                };

                loginRequest.AuthParameters.Add("USERNAME", user.PhoneNumber);
                loginRequest.AuthParameters.Add("PASSWORD", user.PhoneNumber);


                var loginResponse = await cognito.AdminInitiateAuthAsync(loginRequest);

                var userAttrs = new List<AttributeType>();
                userAttrs.Add(new AttributeType() { Name = CognitoAttribute.Name.AttributeName, Value = user.FirstName });
                userAttrs.Add(new AttributeType() { Name = CognitoAttribute.MiddleName.AttributeName, Value = user.MiddleName });
                userAttrs.Add(new AttributeType() { Name = CognitoAttribute.FamilyName.AttributeName, Value = user.FamilyName });
                userAttrs.Add(new AttributeType() { Name = CognitoAttribute.Email.AttributeName, Value = user.Email });
                userAttrs.Add(new AttributeType() { Name = CognitoAttribute.Address.AttributeName, Value = user.Address });
                userAttrs.Add(new AttributeType() { Name = CognitoAttribute.BirthDate.AttributeName, Value = user.BirthDate.ToString("yyyy/MM/dd") });


                var updateRequest = new UpdateUserAttributesRequest()
                {
                    AccessToken = loginResponse.AuthenticationResult.AccessToken,
                    UserAttributes = userAttrs
                };

                var updated = await cognito.UpdateUserAttributesAsync(updateRequest);



                return IdentityResult.Success;
            }
            catch (AmazonCognitoIdentityProviderException ex)
            {
                return BadRequest(ex.ToFinstroError());
            }




        }

    }

}