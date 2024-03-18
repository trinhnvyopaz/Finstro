using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Response.Incc
{
    public class CreateCardResponse
    {
        [JsonIgnore]
        [JsonProperty(PropertyName = "crt_securtoken_24_f2")]
        public string Token24F2 { get; set; }

        [JsonProperty(PropertyName = "crt_card_token")]
        public string CardToken { get; set; }

        [JsonProperty(PropertyName = "consumer_id")]
        public string ConsumerId { get; set; }

        [JsonProperty(PropertyName = "crt_code_product")]
        public string ProductCode { get; set; }


        public string CardMaskedNumber { get; set; }
        public string CardEmbossName { get; set; }
        public string Status { get; set; }
        public string InccStatusCode { get; set; }
        public decimal OpenToBuy { get; set; }


        [JsonIgnore]
        [JsonProperty(PropertyName = "error_msg")]
        public string ErrorMessage { get; set; }

        [JsonIgnore]
        [JsonProperty(PropertyName = "error_code")]
        public string ErrorCode { get; set; }
        [JsonIgnore]
        [JsonProperty(PropertyName = "mac_value")]
        public string MacValue { get; set; }
    }

}

