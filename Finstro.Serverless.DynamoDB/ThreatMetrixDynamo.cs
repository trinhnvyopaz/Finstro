using System;
using System.Collections.Generic;
using System.Linq;
using Finstro.Serverless.Models.Dynamo;
using ServiceStack.Aws.DynamoDb;
using ServiceStack.Text;

namespace Finstro.Serverless.DynamoDB
{
    public class ThreatMetrixDynamo : DynamoBase
    {
        private readonly IPocoDynamo db;

        public ThreatMetrixDynamo()
        {
            db = new PocoDynamo(CreateDynamoDbClient()).RegisterTable<ThreatMetrixResult>();

            db.InitSchema();

        }

        public ThreatMetrixResult GetThreatMetrixResult(string userId)
        {

            var q = db.FromQuery<ThreatMetrixResult>(x => x.UserSubId == userId);
            var result = db.Query(q);

            ThreatMetrixResult data;

            data = result.FirstOrDefault();

            if (data == null)
            {
                data = new ThreatMetrixResult()
                {
                    UserSubId = userId,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                };
                Update(data);
            }
            return data;
        }

        public ThreatMetrixResult Update(ThreatMetrixResult userDetail)
        {
            userDetail.ModifiedDate = DateTime.UtcNow;

            db.PutItem(userDetail);

            return userDetail;
        }

    }
}
