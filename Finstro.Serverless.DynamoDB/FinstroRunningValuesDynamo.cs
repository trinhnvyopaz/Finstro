using System;
using System.Collections.Generic;
using System.Linq;
using Finstro.Serverless.Models.Dynamo;
using ServiceStack.Aws.DynamoDb;
using ServiceStack.Text;

namespace Finstro.Serverless.DynamoDB
{
    public class FinstroRunningValuesDynamo : DynamoBase
    {
        private readonly IPocoDynamo db;

        public FinstroRunningValuesDynamo()
        {
            db = new PocoDynamo(CreateDynamoDbClient()).RegisterTable<FinstroRunningValues>();

            db.InitSchema();

        }

        public FinstroRunningValues GetFinstroRunningValues()
        {

            var q = db.FromQuery<FinstroRunningValues>(x => x.Version == 1);
            var result = db.Query(q);




            FinstroRunningValues data;

            data = result.FirstOrDefault();

            if (data == null)
            {
                data = new FinstroRunningValues()
                {
                    InccToken = null,
                    Version = 1,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                };

                data.DefaultProducts = CreateDefaultProduct();

                Update(data);
            }
            return data;
        }

        public FinstroRunningValues Update(FinstroRunningValues data)
        {
            data.ModifiedDate = DateTime.UtcNow;

            db.PutItem(data);

            return data;
        }
        public void LogHeartBeat()
        {

            db.UpdateItem(1,
            put: () => new FinstroRunningValues
            {
                LastHeartbeat = DateTime.UtcNow
            });


        }

        private List<DefaultProduct> CreateDefaultProduct()
        {
            var settings = new Dictionary<string, object>();

            settings.Add("BillingFrequency", "Monthly");
            settings.Add("RepaymentTerm", "PaywithNextBill");
            settings.Add("OngoingFeesinFirstInstalment", true);
            settings.Add("PercentageTransactionFee", 1.95m);
            settings.Add("FlatTransactionFee", 2.5m);
            settings.Add("PercentageAdvanceFee", 1.00m);
            settings.Add("FlatAdvanceFee", 1.00m);
            settings.Add("FlatInterest", 1.00m);
            settings.Add("CompoundInterest", 1.00m);
            settings.Add("FlatFee", 0.00m);
            settings.Add("FactorRate", 1.00m);
            settings.Add("FlatArrearsRate", 0.10m);
            settings.Add("CompoundArrearsRate", 0.00m);
            settings.Add("FlatDishonourFee", 10.00m);
            settings.Add("PercentageDishonourFee", 0);
            settings.Add("DailyRate", 0.01m);
            settings.Add("FlatRescheduleFee", 0.00m);
            settings.Add("ProcessingFeeFlat", 1m);
            settings.Add("ProcessingFeePercentage", 0.1m);
            settings.Add("BillingDate", 1);
            settings.Add("BillingDelta", 7);
            settings.Add("BillingDeltaMax", 14);
            settings.Add("AccruedInterest", 0.1m);
            settings.Add("DishonourReschedulePeriod", 7);
            settings.Add("FlatRescheduleFee", 5);



            var list = new List<DefaultProduct>();
            list.Add(new DefaultProduct() { ProductType = "FistroPay", ProductSettings = settings });
            return list;
        }
    }
}
