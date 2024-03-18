using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Request.Incc
{
    
    public class GetAuthorizationRequest
    {
        [JsonProperty(PropertyName = "corporate_id")]
        public string corporate_id { get; set; }

        [JsonProperty(PropertyName = "institution_id")]
        public string institution_id { get; set; }

        [JsonProperty(PropertyName = "crt_securtoken_24_f1")]
        public string crt_securtoken_24_f1 { get; set; }

        [JsonProperty(PropertyName = "msg_type")]
        public string msg_type { get; set; }

        [JsonProperty(PropertyName = "trx_card_token")]
        public string trx_card_token { get; set; }

        [JsonProperty(PropertyName = "trx_process_code")]
        public string trx_process_code { get; set; }

        [JsonProperty(PropertyName = "trx_amount")]
        public string trx_amount { get; set; }

        [JsonProperty(PropertyName = "transm_dat_time")]
        public string transm_dat_time { get; set; }

        [JsonProperty(PropertyName = "trx_stan")]
        public string trx_stan { get; set; }

        [JsonProperty(PropertyName = "local_time")]
        public string local_time { get; set; }

        [JsonProperty(PropertyName = "local_date")]
        public string local_date { get; set; }

        [JsonProperty(PropertyName = "trx_mcc")]
        public string trx_mcc { get; set; }

        [JsonProperty(PropertyName = "trx_acq_country")]
        public string trx_acq_country { get; set; }

        [JsonProperty(PropertyName = "trx_acq_inst")]
        public string trx_acq_inst { get; set; }

        [JsonProperty(PropertyName = "trs_rrn")]
        public string trs_rrn { get; set; }

        [JsonProperty(PropertyName = "trx_terminal_id")]
        public string trx_terminal_id { get; set; }

        [JsonProperty(PropertyName = "trx_accep_id")]
        public string trx_accep_id { get; set; }

        [JsonProperty(PropertyName = "crd_acc_name_loc")]
        public string crd_acc_name_loc { get; set; }

        [JsonProperty(PropertyName = "card_holder_billing_curr")]
        public string card_holder_billing_curr { get; set; }

        [JsonProperty(PropertyName = "amt_cash")]
        public string amt_cash { get; set; }

        [JsonProperty(PropertyName = "crt_numauto")]
        public string crt_numauto { get; set; }

        [JsonProperty(PropertyName = "mac_value")]
        public string mac_value { get; set; }

        [JsonProperty(PropertyName = "transaction_purpose")]
        public string transaction_purpose { get; set; }
    }



}

