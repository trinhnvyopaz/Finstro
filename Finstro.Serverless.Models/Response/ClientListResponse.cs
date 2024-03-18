using Newtonsoft.Json;
using System;

namespace Finstro.Serverless.Models.Response
{
    
    public class ClientListResponse
    {

        public string Id { get; set; }        public DateTime ModifiedOn { get; set; }        public DateTime CreatedDate { get; set; }        public string Abn { get; set; }
        public string CompanyName { get; set; }
        public string DirectorName { get; set; }
        public string ClientStatus { get; set; }
        public string ProductType { get; set; }
        public decimal? FacilityLimit { get; set; }

    }

}
