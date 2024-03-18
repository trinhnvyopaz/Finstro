using System;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;

namespace Finstro.Serverless.Models.Dynamo
{
    public class UserDetail
    {
        public UserDetail() {
            FCMTokens = new List<string>();
            CreatedDate = DateTime.UtcNow;
        }
        public List<string> FCMTokens { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
