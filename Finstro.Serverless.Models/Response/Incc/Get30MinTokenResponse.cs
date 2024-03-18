using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Response.Incc
{
    public class Get30MinTokenResponse
    {
        [JsonProperty(PropertyName = "institution_id")]
        public string InstitutionId { get; set; }

        [JsonProperty(PropertyName = "corporate_id")]
        public string CorporateId { get; set; }

        [JsonProperty(PropertyName = "crt_card_token")]
        public string CardToken { get; set; }

        [JsonProperty(PropertyName = "crt_securtoken_24_f2")]
        public string Token24F2 { get; set; }

        [JsonProperty(PropertyName = "crt_securtoken_30")]
        public string Token30min { get; set; }

        [JsonProperty(PropertyName = "zpk")]
        public string PublicKey { get; set; }

        [JsonProperty(PropertyName = "error_msg")]
        public string ErrorMessage { get; set; }

        [JsonProperty(PropertyName = "error_code")]
        public string ErrorCode { get; set; }

        [JsonProperty(PropertyName = "mac_value")]
        public string MacValue { get; set; }
    }
}

