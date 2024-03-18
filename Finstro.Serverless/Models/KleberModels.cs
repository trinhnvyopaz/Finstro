using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finstro.Serverless.API.Models
{
    public class AddressSearch
    {
        public string Address { get; set; }
        public string RequestKey { get; set; }
    }
    public class AddressListResponse
    {
        public DtResponse DtResponse { get; set; }
    }
    public class DtResponse
    {
        public string RequestId { get; set; }
        public string ResultCount { get; set; }
        public string ErrorMessage { get; set; }
        public List<Result> Result { get; set; }
    }
    public class Result
    {
        public string RecordId { get; set; }
        public string AddressLine { get; set; }
        public string Locality { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
    }
    public class AddressResponse
    {
        [JsonProperty("DtResponse")]
        public DtresponseAddress DtResponse { get; set; }
    }
    public class DtresponseAddress
    {
        [JsonProperty("RequestId")]
        public string RequestId { get; set; }

        [JsonProperty("ResultCount")]
        public string ResultCount { get; set; }

        [JsonProperty("ErrorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty("Result")]
        public List<AddressDetail> Result { get; set; }
    }
    public class AddressDetail
    {
        public string DPID { get; set; }
        public string UnitType { get; set; }
        public string UnitNumber { get; set; }
        public string LevelType { get; set; }
        public string LevelNumber { get; set; }
        public string LotNumber { get; set; }
        public string StreetNumber1 { get; set; }
        public string StreetNumberSuffix1 { get; set; }
        public string StreetNumber2 { get; set; }
        public string StreetNumberSuffix2 { get; set; }
        public string PostBoxNumber { get; set; }
        public string PostBoxNumberPrefix { get; set; }
        public string PostBoxNumberSuffix { get; set; }
        public string StreetName { get; set; }
        public string StreetType { get; set; }
        public string StreetSuffix { get; set; }
        public string PostBoxType { get; set; }
        public string BuildingName { get; set; }
        public string AddressLine { get; set; }
        public string Locality { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
    }

}
