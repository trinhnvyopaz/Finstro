using System;
using System.Collections.Generic;
using Finstro.Serverless.Models.Response.Incc;
using ServiceStack.DataAnnotations;

namespace Finstro.Serverless.Models.Dynamo
{

    public class FinstroRunningValues
    {

        [AutoIncrement]
        public int Id { get; set; }

        [HashKey]
        public int Version { get; set; }

        public Get24HourTokenResponse InccToken { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime LastHeartbeat { get; set; }
        public List<DefaultProduct> DefaultProducts { get; set; }

    }

    public class DefaultProduct {
        public string ProductType { get; set; }
        public Dictionary<string, object> ProductSettings { get; set; }

    }


}
