using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Request.Incc
{
    public class GetAuthorizationResponse
    {
        [JsonProperty(PropertyName = "trx_card_token")]
        public string CardToken { get; set; }

        [JsonProperty(PropertyName = "corporate_id")]
        public string CorporateId { get; set; }

        [JsonProperty(PropertyName = "institution_id")]
        public string InstitutionId { get; set; }

        [JsonProperty(PropertyName = "msg_type")]
        public string MessageType { get; set; }

        [JsonProperty(PropertyName = "crt_securtoken_24_e2")]
        public string Token24E1 { get; set; }

        [JsonProperty(PropertyName = "trx_stan")]
        public string Stan { get; set; }
        
        [JsonProperty(PropertyName = "crt_numauto")]
        public string NumberAuto { get; set; }

        [JsonProperty(PropertyName = "trx_amount")]
        public string Amount { get; set; }

        [JsonProperty(PropertyName = "trs_rrn")]
        public string ReferenceNumber { get; set; }

        [JsonProperty(PropertyName = "trx_terminal_id")]
        public string TerminalId { get; set; }

        [JsonProperty(PropertyName = "transm_dat_time")]
        public string TransmissionDateTime { get; set; }

        [JsonProperty(PropertyName = "local_time")]
        public string LocalTime { get; set; }

        [JsonProperty(PropertyName = "local_date")]
        public string LocalDate { get; set; }

        [JsonProperty(PropertyName = "error_code")]
        public string ErrorCode { get; set; }

        [JsonProperty(PropertyName = "error_msg")]
        public string ErrorMessage { get; set; }

    }
}

