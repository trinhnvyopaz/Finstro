using System;
using System.Collections.Generic;
using System.Linq;
using Audit.Core;
using Finstro.Serverless.Models.Dynamo;
using Finstro.Serverless.Models.Entity;
using ServiceStack.Aws.DynamoDb;
using ServiceStack.Text;

namespace Finstro.Serverless.DynamoDB
{
    public class AuditEventsDynamo : DynamoBase
    {
        private readonly IPocoDynamo db;

        public AuditEventsDynamo()
        {
            db = new PocoDynamo(CreateDynamoDbClient()).RegisterTable<FinstroAuditEvent>();

            //db.InitSchema();

        }

        public FinstroAuditEvent GetEvent(string eventId)
        {
            var q = db.FromQuery<FinstroAuditEvent>(x => x.EventId == eventId);
            var result = db.Query(q);
            return result.FirstOrDefault();
        }

        public AuditEventUserGI GetByTransactionNumber(string userId, string autoNumber)
        {

            var result = db.FromQueryIndex<AuditEventUserGI>(x => x.UserSubId == userId
            && x.StartDate > DateTime.UtcNow.AddDays(-90)).Filter(n => n.NumberAuto == autoNumber).Exec();

            return result.FirstOrDefault();
        }

        public List<FinstroAuditEvent> GetByLatestByMinutes(string userId, int minutes)
        {

            var ids = db.FromQueryIndex<AuditEventUserGI>(x => x.UserSubId == userId
            && x.StartDate >= DateTime.UtcNow.AddMinutes(minutes)).Exec();

            var list = ids.Select(i => i.EventId).ToList();
            if (list.Count > 0)
            {
                var q = db.FromScan<FinstroAuditEvent>(x => list.Contains(x.EventId)).Exec();
                return q.ToList();
            }
            else
            {
                return new List<FinstroAuditEvent>();
            }

        }

        public FinstroAuditEvent Update(FinstroAuditEvent data)
        {

            db.PutItem(data);

            return data;
        }

    }
}
