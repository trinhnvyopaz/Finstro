using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Response.Incc
{
    public class GetCardDataResponse
    {

        [JsonProperty(PropertyName = "crt_card_token")]
        public string CardToken { get; set; }

        [JsonProperty(PropertyName = "pan")]
        public string PanNumber { get; set; }

        [JsonProperty(PropertyName = "cvv")]
        public string Cvv { get; set; }

        [JsonProperty(PropertyName = "exp_date")]
        public string ExpireDate { get; set; }

        [JsonProperty(PropertyName = "balance")]
        public string Balance { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "crt_first_name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "crt_last_name")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "transactions")]
        public object Transactions { get; set; }

        [JsonProperty(PropertyName = "error_msg")]
        public string ErrorMessage { get; set; }

        [JsonProperty(PropertyName = "error_code")]
        public string ErrorCode { get; set; }

    }
}

