using Audit.Core;
using Finstro.Serverless.Models.Request.Incc;
using ServiceStack.Aws.DynamoDb;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Entity
{

    [References(typeof(AuditEventUserGI))]
    public class FinstroAuditEvent : AuditEvent
    {

        public FinstroAuditEvent()
        {
            EventId = Guid.NewGuid().ToString();
        }
        public FinstroAuditEvent(string userId, string autoNumber, GetAuthorizationRequest request)
        {
            EventId = Guid.NewGuid().ToString();
            UserSubId = userId;
            NumberAuto = autoNumber;
            AuthorizationRequest = request;
        }
        public FinstroAuditEvent(string userId)
        {
            EventId = Guid.NewGuid().ToString();
            UserSubId = userId;
        }


        [HashKey]
        public string EventId { get; set; }

        [RangeKey]
        public string UserSubId { get; set; }

        public string NumberAuto { get; set; }

        public GetAuthorizationRequest AuthorizationRequest { get; set; }

    }

    public class AuditEventUserGI : IGlobalIndex<FinstroAuditEvent>
    {
        [HashKey]
        public string UserSubId { get; set; }


        [Index]
        public DateTime StartDate { get; set; }

        [Index]
        public string NumberAuto { get; set; }

        [Index]
        public string EventType { get; set; }


        public string EventId { get; set; }

    }


}
