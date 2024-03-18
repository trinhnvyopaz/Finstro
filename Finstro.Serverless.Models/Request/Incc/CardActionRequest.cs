using Finstro.Serverless.Models.Dynamo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Request.Incc
{
    public class CardActionRequest
    {
        public string CardToken { get; set; }
        public string UserId { get; set; }
        public Address PostalAddress { get; set; }

    }
}

