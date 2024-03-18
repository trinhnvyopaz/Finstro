using Finstro.Serverless.Helper;
using Finstro.Serverless.Models.Request.Rules;
using Finstro.Serverless.Models.Response.Rules;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FinstroServerless.Services.Fcc
{
    public class FccServices
    {
        private string _authorization;
        private RestClient client;
        public FccServices(string authorization)
        {
            _authorization = authorization;
            client = new RestClient(AppSettings.FccServices.URL);
        }

        public bool ValidadeDriverLicense(string number, string state)
        {
            bool isValid = false;

            var request = new RestRequest("rules/validateDriverLicence", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            

            dynamic data = new 
            {
                licenceNumber = number,
                state = state
                
            };

            request.AddJsonBody(JsonConvert.SerializeObject(data));

            var result = client.Execute(request);

            if (result.StatusCode == HttpStatusCode.OK)
            {
                isValid = JsonConvert.DeserializeObject<bool>(result.Content);
            }
            else
            {
                throw FinstroErrorType.FccServices.FccServiceFail;
            }

            return isValid;
        }

        public BankDetailsRulesResult BankDetailsRulesProcess(List<BankStatementRuleRequest> bankStatementRuleRequests)
        {
            BankDetailsRulesResult data = new BankDetailsRulesResult();

            var request = new RestRequest("rules/processBankStatementInputs", Method.POST);
            request.AddHeader("Content-Type", "application/json");

            request.AddJsonBody(JsonConvert.SerializeObject(bankStatementRuleRequests));

            var result = client.Execute(request);

            if (result.StatusCode == HttpStatusCode.OK)
            {
                data = JsonConvert.DeserializeObject<BankDetailsRulesResult>(result.Content);
            }
            else
            {
                throw FinstroErrorType.FccServices.FccServiceFail;
            }

            return data;
        }


        public CreditAssessmentRulesResult CreditAssessmentRulesProcess(CreditAssessmentRuleRequest creditAssessmentRuleRequest)
        {
            CreditAssessmentRulesResult data = new CreditAssessmentRulesResult();

            var request = new RestRequest("rules/processCreditAssessment", Method.POST);
            request.AddHeader("Content-Type", "application/json");

            request.AddJsonBody(JsonConvert.SerializeObject(creditAssessmentRuleRequest));

            var result = client.Execute(request);

            if (result.StatusCode == HttpStatusCode.OK)
            {
                data = JsonConvert.DeserializeObject<CreditAssessmentRulesResult>(result.Content);
            }
            else
            {
                throw FinstroErrorType.FccServices.FccServiceFail;
            }

            return data;
        }

    }
}
