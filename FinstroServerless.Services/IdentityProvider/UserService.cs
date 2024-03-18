using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Finstro.Serverless.Dapper.Repository;
using Finstro.Serverless.DynamoDB;
using Finstro.Serverless.Helper;
using Finstro.Serverless.Models;
using Finstro.Serverless.Models.Entity;
using Finstro.Serverless.Models.Request.User;
using FinstroServerless.Services.Email;
using PhoneNumbers;

namespace FinstroServerless.Services.IdentityProvider
{

    public class UserService
    {
        private string _clientId = AppSettings.Cognito.ClientId;
        private string _userPoolId = AppSettings.Cognito.PoolId;
        private readonly RegionEndpoint _region = RegionEndpoint.USEast2;
        private AmazonCognitoIdentityProviderClient cognito;

        private CreditApplicationDynamo _creditApplicationDynamo;
        public UserService()
        {
            cognito = new AmazonCognitoIdentityProviderClient(_region);
            _creditApplicationDynamo = new CreditApplicationDynamo();
        }

        public async Task<bool> CreateFinstroUser(CreateUserRequest user)
        {

            UserType cognitoUser = null;

            if (!user.Accepted)
                throw FinstroErrorType.User.UserTermsNotAccepted;

            user.MobilePhoneNumber = ValidationHelper.FormatPhoneNumber(user.MobilePhoneNumber);

            if (!ValidationHelper.IsValidEmail(user.EmailAddress))
                throw FinstroErrorType.Schema.InvalidEmail;


            if (cognitoUser == null)
                cognitoUser = await GetUserByAttribute(CognitoAttribute.Email, user.EmailAddress);

            if (cognitoUser != null && cognitoUser.UserStatus == UserStatusType.CONFIRMED)
                throw FinstroErrorType.User.UserExist;


            if (cognitoUser == null || cognitoUser.UserStatus == UserStatusType.UNCONFIRMED)
                cognitoUser = await GetUserByAttribute(CognitoAttribute.PhoneNumber, user.MobilePhoneNumber);

            if (cognitoUser != null && cognitoUser.UserStatus == UserStatusType.CONFIRMED)
                throw FinstroErrorType.User.PhoneExist;

            if (cognitoUser != null && cognitoUser.UserStatus == UserStatusType.UNCONFIRMED)
            {
                var deleted = await cognito.AdminDeleteUserAsync(new AdminDeleteUserRequest()
                {
                    Username = cognitoUser.Username,
                    UserPoolId = _userPoolId
                });
            }

            var request = new SignUpRequest
            {
                ClientId = _clientId,
                Password = user.EmailAddress,
                Username = user.EmailAddress,

            };

            request.UserAttributes.Add(new AttributeType { Name = CognitoAttribute.Email.AttributeName, Value = user.EmailAddress });
            request.UserAttributes.Add(new AttributeType { Name = CognitoAttribute.PreferredUsername.AttributeName, Value = user.EmailAddress });
            request.UserAttributes.Add(new AttributeType { Name = CognitoAttribute.GivenName.AttributeName, Value = user.FirstGivenName });
            request.UserAttributes.Add(new AttributeType { Name = CognitoAttribute.FamilyName.AttributeName, Value = user.FamilyName });
            request.UserAttributes.Add(new AttributeType { Name = CognitoAttribute.TermsAccepted.AttributeName, Value = Convert.ToInt32(user.Accepted).ToString() });

            if (!string.IsNullOrEmpty(user.MobilePhoneNumber))
            {
                request.UserAttributes.Add(new AttributeType { Name = CognitoAttribute.PhoneNumber.AttributeName, Value = user.MobilePhoneNumber });
            }


            var response = await cognito.SignUpAsync(request);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                cognitoUser = await GetUserByAttribute(CognitoAttribute.PhoneNumber, user.MobilePhoneNumber);

                await cognito.AdminAddUserToGroupAsync(new AdminAddUserToGroupRequest()
                {
                    GroupName = FinstroCognitoUserGroups.FinstroAppUser.ToString(),
                    Username = cognitoUser.Username,
                    UserPoolId = _userPoolId
                });

            }
            return true;
        }

        public async Task<InitiateAuthResponse> RefreshToken(string refreshToken)
        {

            var request = new InitiateAuthRequest
            {
                ClientId = _clientId,
                AuthFlow = AuthFlowType.REFRESH_TOKEN_AUTH,

            };

            request.AuthParameters.Add("REFRESH_TOKEN", refreshToken);

            try
            {

                var response = await cognito.InitiateAuthAsync(request);
                return response;

            }
            catch
            {
                throw FinstroErrorType.Auth.InvalidRefreshToken;
            }

        }




        public async Task<UserType> GetUserByAttribute(CognitoAttribute attribute, string attributeValue)
        {
            var request = new ListUsersRequest()
            {
                Filter = $"{attribute.AttributeName} = \"{attributeValue}\"",
                UserPoolId = _userPoolId,
                Limit = 10,
            };
            var users = await cognito.ListUsersAsync(request);

            if (users.Users.Count == 0)
                return null;

            var usersList = users.Users.OrderByDescending(u => u.UserCreateDate);

            var cognitoUser = usersList.FirstOrDefault();

            return cognitoUser;
        }


        public async Task<bool> SendAccessCode(UserType cognitoUser, string password = null)
        {
            try
            {
                EmailService emailService = new EmailService();
                var templateType = emailService.GetAccessCodeEmailTemplate();


                string userEmail = cognitoUser.Attributes.FirstOrDefault(a => a.Name == CognitoAttribute.PreferredUsername.AttributeName).Value;
                string firstName = cognitoUser.Attributes.FirstOrDefault(a => a.Name == CognitoAttribute.GivenName.AttributeName).Value;

                if (string.IsNullOrEmpty(password))
                    password = GenerateCode();


                var changePass = await cognito.AdminSetUserPasswordAsync(new AdminSetUserPasswordRequest()
                {
                    Username = cognitoUser.Username,
                    Password = password,
                    Permanent = true,
                    UserPoolId = _userPoolId

                });

                string emailTemplate = templateType.Replace(@"${accessCode}' />", @"' /> " + password);

                emailService.SendEmail(userEmail, firstName, "Access Code", emailTemplate);


                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }



        public async Task<bool> SendSMSAccessCode(UserType cognitoUser, string password)
        {
            try
            {
                string phoneNumber = cognitoUser.Attributes.FirstOrDefault(a => a.Name == CognitoAttribute.PhoneNumber.AttributeName).Value;
                string firstName = cognitoUser.Attributes.FirstOrDefault(a => a.Name == CognitoAttribute.GivenName.AttributeName).Value;

                string smsText = $"Hi {firstName} your new password is {password}.";

                AmazonSimpleNotificationServiceClient snsClient = new AmazonSimpleNotificationServiceClient(Amazon.RegionEndpoint.USEast1);
                PublishRequest pubRequest = new PublishRequest();
                pubRequest.Message = smsText;
                pubRequest.PhoneNumber = phoneNumber;

                PublishResponse pubResponse = await snsClient.PublishAsync(pubRequest);
                Console.WriteLine(pubResponse.MessageId);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }


        public static string GenerateCode()
        {

            Random generator = new Random();
            string passCode = generator.Next(0, 999999).ToString("D6");
            return passCode;
        }

        public void SaveFCMToken(string userId, string fcmToken)
        {
            var application = _creditApplicationDynamo.GetCreditApplicationForUser(userId);

            if (application.UserDetail == null)
            {
                application.UserDetail = new Finstro.Serverless.Models.Dynamo.UserDetail();
            }

            if (!application.UserDetail.FCMTokens.Contains(fcmToken))
            {
                application.UserDetail.ModifiedDate = DateTime.UtcNow;
                application.UserDetail.FCMTokens.Add(fcmToken);
                _creditApplicationDynamo.Update(application);
            }
            

        }

    }
}
