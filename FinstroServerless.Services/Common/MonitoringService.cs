using Finstro.Serverless.DynamoDB;
using Finstro.Serverless.Models.Dynamo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinstroServerless.Services.Common
{
    public class MonitoringService
    {



        public static void LogHeartbeat()
        {
            FinstroRunningValuesDynamo dynamo = new FinstroRunningValuesDynamo();
            dynamo.LogHeartBeat();

        }
    }
}
