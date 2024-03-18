using System;
using System.Collections.Generic;
using System.Net;

namespace Finstro.Serverless.Helper
{


    public class FinstroCustomPolicy : Attribute
    {
        public string Name { get; set; }
        public string Roles { get;}

        public class AdminUserPolicy
        {
            public const string PolicyName = "AdminUserPolicy";
            public const string PolicyRoles = "AdminPortalSuperUser,AdminPortalBackOffice";
        }

        public class AdminSuperUserPolicy
        {
            public const string PolicyName = "AdminSuperUserPolicy";
            public const string PolicyRoles = "AdminPortalSuperUser";
        }

        public class FinstroAppPolicy
        {
            public const string PolicyName = "FinstroAppPolicy";
            public const string PolicyRoles = "FinstroAppUser";
        }

    }


}
