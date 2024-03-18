using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using Audit.Core;
using Finstro.Serverless.Helper;
using Finstro.Serverless.Models;
using Finstro.Serverless.Models.Dynamo;
using Finstro.Serverless.Models.Request.User;
using Finstro.Serverless.Models.Response;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Aws.DynamoDb;
using ServiceStack.Text;
using static Finstro.Serverless.Helper.AppSettings;

namespace Finstro.Serverless.DynamoDB
{
    public class CreditApplicationDynamo : DynamoBase
    {
        private readonly IPocoDynamo db;

        public CreditApplicationDynamo()
        {
            db = new PocoDynamo(CreateDynamoDbClient()).RegisterTable<CreditApplication>();


            //db.DeleteTable<CreditApplication>();

            //db.InitSchema();

            //db.GetTableNames().PrintDump();
        }

        public AppRecoveryResponse GetAppRecovery(string userId)
        {

            var recovery = db.FromQuery<CreditApplication>(x => x.UserSubId == userId).Select<AppRecoveryResponse>()
                                .Exec()
                                .Map(x => x.ConvertTo<AppRecoveryResponse>()).FirstOrDefault() ;

            if (recovery == null)
            {
                recovery = CreateNewCreditApplication(userId).ConvertTo<AppRecoveryResponse>();
            }

            return recovery;
        }


        public CreditApplication GetCreditApplicationForUser(string userId)
        {

            var q = db.FromQuery<CreditApplication>(x => x.UserSubId == userId);
            var dbApplications = db.Query(q);

            CreditApplication creditApplication;

            creditApplication = dbApplications.OrderByDescending(a => a.CreatedDate)
                                .FirstOrDefault(a => a.CreditApplicationStatus != EnumCreditApplicationStatus.Complete.ToString());

            if (creditApplication == null)
            {
                creditApplication = CreateNewCreditApplication(userId);
            }
            return creditApplication;
        }

        private CreditApplication CreateNewCreditApplication(string userId)
        {
            CreditApplication creditApplication = new CreditApplication()
            {
                UserSubId = userId,
                CreditApplicationStatus = EnumCreditApplicationStatus.Incomplete.ToString(),
                CreatedDate = DateTime.UtcNow,
                CreditAssessmentStatus = EnumCreditAssessmentStatus.None.GetAttributeOfType<DescriptionAttribute>().Description,
                ExternalId = Guid.NewGuid().ToString().ToLower()
            };
            var dynamo = new FinstroRunningValuesDynamo();
            FinstroRunningValues settings = dynamo.GetFinstroRunningValues();

            var product = settings.DefaultProducts.FirstOrDefault(p => p.ProductType == FinstroSettings.ProductNameFinstroPay);
            if (product != null)
            {
                creditApplication.ProductType = product.ProductType;
                creditApplication.ProductSettings = product.ProductSettings;
            }

            Update(creditApplication);
            return creditApplication;
        }

        public CreditApplication GetCreditApplication(string Id)
        {

            var creditApplication = db.GetItem<CreditApplication>(Id);
            return creditApplication;
        }


        public IEnumerable<ClientListResponse> GetCreditApplications()
        {

            var data = db.FromScan<CreditApplication>(a => a.BusinessDetails != null).Exec().ToList();

            var list = data.Select(x => new ClientListResponse()
            {
                Abn = x.BusinessDetails.AsicBusiness.Abn,
                ClientStatus = x.CreditApplicationStatus,
                CompanyName = x.BusinessDetails.AsicBusiness?.CompanyLegalName,
                DirectorName = $"{x.Contacts?.FirstOrDefault().FirstGivenName} {x.Contacts?.FirstOrDefault().FamilyName}",
                FacilityLimit = x.BusinessDetails?.SelectedCreditAmount == null ? 0 : x.BusinessDetails.SelectedCreditAmount,
                ModifiedOn = x.ModifiedDate,
                CreatedDate = x.CreatedDate,
                ProductType = "Finstro Pay",
                Id = x.UserSubId

            }).ToList();

            return list;

        }

        public CreditApplication GetCreditApplicationByExternalId(string externalId)
        {

            var creditApplication = db.FromScan<CreditApplication>()
                                            .Filter(x => x.ExternalId == externalId)
                                            .Exec()
                                            .FirstOrDefault();


            return creditApplication;
        }

        public CreditApplication GetCardByToken(string cardToken)
        {

            var creditApplication = db.FromScan<CreditApplication>()
                                            .Filter("FinstroCards[0].InccCard.CardToken = :cardToken", new Dictionary<string, string> { { "cardToken", cardToken } })
                                            .Exec()
                                            .FirstOrDefault();

            return creditApplication;
        }


        public AuditEvent GetLog(string userId)
        {

            var auditEvent = db.FromQuery<AuditEvent>()
                                            .Filter("UserSubId = :userId", new Dictionary<string, string> { { "userId", userId } })
                                            .Exec()
                                            .FirstOrDefault();

            return auditEvent;
        }


        public CreditApplication Update(CreditApplication application)
        {
            application.ModifiedDate = DateTime.UtcNow;

            db.PutItem(application);

            return application;
        }


    }
}
