using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Request.Incc
{
    public class Get24HourTokenRequest
    {
        [JsonProperty(PropertyName = "institution_id")]
        public string InstitutionId { get; set; }

        [JsonProperty(PropertyName = "corporate_id")]
        public string CorporateId { get; set; }

        [JsonProperty(PropertyName = "cx_password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "cx_reason")]
        public string Reason { get; set; }

        [JsonProperty(PropertyName = "mac_value")]
        public string MacValue { get; set; }
    }
}

