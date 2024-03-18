using System;
using System.IO;
using Finstro.Serverless.Models.Request.CreditApplication;
using Newtonsoft.Json;
using Finstro.Serverless.Models.Response;
using Finstro.Serverless.Models.Dynamo;
using Finstro.Serverless.Models;
using Amazon.S3;
using Amazon;
using Finstro.Serverless.Helper;
using Amazon.S3.Model;
using ServiceStack;
using ServiceStack.Aws;
using Finstro.Serverless.Models.Request.Rules;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Finstro.Serverless.DynamoDB;
using Finstro.Serverless.Helper;
using FinstroServerless.Services.Clients;

namespace FinstroServerless.Services.BankStatements
{
    public class BankStatementService
    {

        public void ProcessBankStatement(BankStatementFilesRequest bankStatementFilesRequest)
        {
            List<BankStatementRuleRequest> bankStatementRuleRequests = new List<BankStatementRuleRequest>();

            CreditApplicationDynamo _creditApplicationDynamo = new CreditApplicationDynamo();
            var application = _creditApplicationDynamo.GetCreditApplicationByExternalId(bankStatementFilesRequest.ReferrerCode.Replace(AppSettings.FinstroSettings.BankStatementPrefix, ""));

            if (application == null)
                throw FinstroErrorType.CreditApplication.ApplicationNotFound;


            Dictionary<string, byte[]> filesList = new Dictionary<string, byte[]>();


            BankStatement bankStatement = new BankStatement();
            string userId = bankStatementFilesRequest.ReferrerCode;

            foreach (var item in bankStatementFilesRequest.Files)
            {
                filesList.Add(item.FileName, item.OpenReadStream().ReadFullyBytes());

                if (item.FileName.EndsWith("json", StringComparison.CurrentCulture))
                {
                    var jsonStream = item.OpenReadStream();
                    using (StreamReader streamReader = new StreamReader(new MemoryStream(filesList[item.FileName])))
                    {
                        string json = streamReader.ReadToEnd();

                        bankStatement = JsonConvert.DeserializeObject<BankStatement>(json);
                    }

                }
            }



            if (bankStatement != null)
            {
                foreach (var item in bankStatement.DecisionMetrics)
                {
                    string type = item.Id;

                    string values = item.Value.ToString().Replace("{{", "{").Replace("}}", "}");
                    if (type.StartsWith("BF"))
                    {
                        try
                        {
                            var metrics = JsonConvert.DeserializeObject<Dictionary<string, object>>(values);

                            foreach (var metric in metrics)
                            {
                                string month = Regex.Match(metric.Key, @"\d+").Value;

                                bankStatementRuleRequests.Add(new BankStatementRuleRequest()
                                {
                                    Month = Convert.ToInt32(month),
                                    Type = type,
                                    Value = float.Parse(metric.Value.ToString())
                                });


                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        float tValue = 0;

                        float.TryParse(item.Value.ToString(), out tValue);

                        bankStatementRuleRequests.Add(new BankStatementRuleRequest()
                        {
                            Month = 0,
                            Type = type,
                            Value = tValue
                        });
                    }

                }

            }


            BankStatementProcess bankStatementProcess = new BankStatementProcess();

            if (application.BankStatementProcess == null)
                application.BankStatementProcess = new List<BankStatementProcess>();

            bankStatementProcess.Files = new List<BankStatementFiles>();

            foreach (var item in bankStatement.BankData.BankAccounts)
            {
                item.Transactions = null;
                item.StatementSummary = null;
                item.DayEndBalances = null;
                item.AdditionalDetails = null;
                item.StatementAnalysis = null;
            }

            bankStatementProcess.BankData = bankStatement.BankData;
            bankStatementProcess.Reference = bankStatement.Reference;
            bankStatementProcess.SubmissionTime = bankStatement.SubmissionTime;
            bankStatementProcess.DataVersion = bankStatement.DataVersion;
            bankStatementProcess.CreatedDate = DateTime.UtcNow;

            foreach (var item in filesList)
            {
                string ext = Path.GetExtension(item.Key).Replace(".", "");

                string file = AwsHelper.UploadFiles(bankStatementFilesRequest.ReferrerCode, EnumIdFileType.BankStatement, item.Value, ext);

                var tFile = new BankStatementFiles()
                {
                    Description = "Created on onboarding.",
                    Name = item.Key,
                    Type = ext,
                    UploadDate = DateTime.UtcNow,
                    Url = file
                };

                bankStatementProcess.Files.Add(tFile);
            }

            application.BankStatementProcess.Add(bankStatementProcess);
            application.BankStatementDone = true;

            _creditApplicationDynamo.Update(application);

            CreditApplicationService creditApplicationService = new CreditApplicationService();

            try
            {
                creditApplicationService.BusinessCreditCheck(application);
                creditApplicationService.ProcessBusinessRules(application, bankStatementRuleRequests);

            }
            catch {}

        }



    }
}
