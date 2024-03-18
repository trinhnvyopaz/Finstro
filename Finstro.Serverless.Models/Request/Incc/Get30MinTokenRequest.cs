using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Request.Incc
{
    public class Get30MinTokenRequest
    {
        [JsonProperty(PropertyName = "institution_id")]
        public string InstitutionId { get; set; }

        [JsonProperty(PropertyName = "corporate_id")]
        public string CorporateId { get; set; }

        [JsonProperty(PropertyName = "crt_card_token")]
        public string CardToken { get; set; }

        [JsonProperty(PropertyName = "crt_securtoken_24_e1")]
        public string Token24E1 { get; set; }

        [JsonProperty(PropertyName = "mac_value")]
        public string MacValue { get; set; }
    }
}

