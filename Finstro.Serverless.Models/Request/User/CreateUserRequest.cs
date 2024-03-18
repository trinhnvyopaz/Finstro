using Finstro.Serverless.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Request.User
{

    public class CreateUserRequest
    {
        public bool Accepted { get; set; }
        public string EmailAddress { get; set; }
        public string FamilyName { get; set; }
        public string FirstGivenName { get; set; }
        public string MobilePhoneNumber { get; set; }
    }
}
