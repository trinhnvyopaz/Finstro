using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Request.Rules
{

    public class BankStatementRuleRequest
    {
        [JsonProperty(PropertyName = "month")]
        public int Month { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "value")]
        public float Value { get; set; }
    }

}
