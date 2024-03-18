using System;
namespace Finstro.Serverless.Models.Request.User
{
    public class ValidateVerificationCodeRequest
    {
        public string VerificationCode { get; set; }
        public string EmailAddress { get; set; }
        public string MobilePhoneNumber { get; set; }
    }
}
