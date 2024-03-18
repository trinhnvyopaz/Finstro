using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Response.Incc
{
    public class Get24HourTokenResponse
    {
        [JsonProperty(PropertyName = "institution_id")]
        public string InstitutionId { get; set; }

        [JsonProperty(PropertyName = "corporate_id")]
        public string CorporateId { get; set; }

        [JsonProperty(PropertyName = "cx_user")]
        public string User { get; set; }

        [JsonProperty(PropertyName = "cx_secur_token_24_e1")]
        public string Token24E1 { get; set; }

        [JsonProperty(PropertyName = "cx_secur_token_24_e2")]
        public string Token24E2 { get; set; }

        [JsonProperty(PropertyName = "mac_value")]
        public string MacValue { get; set; }
        public DateTime CreatedDate { get; set; }

        [JsonProperty(PropertyName = "crt_securtoken_24_f1")]
        public string Token24F1 { get; set; }

        [JsonProperty(PropertyName = "crt_securtoken_24_f2")]
        public string Token24F2 { get; set; }
    }

}

