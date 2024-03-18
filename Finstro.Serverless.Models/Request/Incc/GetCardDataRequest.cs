using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Request.Incc
{
    public class GetCardDataRequest
    {
        [JsonProperty(PropertyName = "crt_card_token")]
        public string CardToken { get; set; }

        [JsonProperty(PropertyName = "crt_securtoken_30")]
        public string Token30min { get; set; }

        [JsonProperty(PropertyName = "call_type")]
        public string CallType { get; set; }
    }
}

