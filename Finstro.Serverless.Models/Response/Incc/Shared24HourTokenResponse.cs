using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Response.Incc
{
    public class Shared24HourTokenResponse
    {
        [JsonProperty(PropertyName = "crt_securtoken_24_f1")]
        public string Token24F1 { get; set; }

        [JsonProperty(PropertyName = "crt_securtoken_24_f2")]
        public string Token24F2 { get; set; }

        [JsonProperty(PropertyName = "crt_securtoken_expire")]
        public DateTime ExpireAt { get; set; }

    }

}

