using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Response.Rules
{
  

    public class CreditAssessmentRulesResult
    {
        public string BusinessType { get; set; }
        public string Message { get; set; }
        public string Result { get; set; }
        public float ClientFacilityLimit { get; set; }
        public Dictionary<string, RuleSetResult> entries { get; set; }
    }

    public class RuleSetResult
    {
        public string code { get; set; }
        public string type { get; set; }
        public string input { get; set; }
        public string result { get; set; }
    }

}
