using Finstro.Serverless.Models.Request.CreditApplication;
using Finstro.Serverless.Models.Response.Rules;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Request.Rules
{
  
    public class CreditAssessmentRuleRequest
    {
        public BankDetailsRulesResult bankStatement { get; set; }

        [JsonProperty("businessCreditCheck", NullValueHandling = NullValueHandling.Ignore)]
        public CompanyCreditCheckResult BusinessCreditCheck { get; set; }

        [JsonProperty("companyCreditCheck", NullValueHandling = NullValueHandling.Ignore)]
        public CompanyCreditCheckResult CompanyCreditCheck { get; set; }

        [JsonProperty("individual", NullValueHandling = NullValueHandling.Ignore)]
        public IndividualCreditCheckResult individual { get; set; }

        [JsonProperty("directors", NullValueHandling = NullValueHandling.Ignore)]
        public List<Director> directors { get; set; }

        [JsonProperty("proprietors", NullValueHandling = NullValueHandling.Ignore)]
        public List<Director> proprietors { get; set; }
    }


}
