using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finstro.Serverless.API.Models
{
    public class UserModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string ConfirmationCode { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string FamilyName { get; set; }
        public string Address { get; set; }
        public DateTime BirthDate { get; set; }

    }

    public class RefreshTokenModel
    {
        public string RefreshToken { get; set; }

    }
}
