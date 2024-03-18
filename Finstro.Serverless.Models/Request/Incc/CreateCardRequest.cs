using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Request.Incc
{
    public class CreateCardRequest
    {

        [JsonProperty(PropertyName = "crt_securtoken_24_e1", NullValueHandling = NullValueHandling.Ignore)]
        public string Token24E1 { get; set; }
        
        [JsonProperty(PropertyName = "cx_securtoken_24_e1", NullValueHandling = NullValueHandling.Ignore)]
        public string CxToken24E1 { get; set; }


        [JsonProperty(PropertyName = "crt_first_name", NullValueHandling = NullValueHandling.Ignore)]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "crt_last_name", NullValueHandling = NullValueHandling.Ignore)]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "crt_emboss_name", NullValueHandling = NullValueHandling.Ignore)]
        public string EmbossName { get; set; }

        [JsonProperty(PropertyName = "cmpl_npai", NullValueHandling = NullValueHandling.Ignore)]
        public string Npai { get; set; }

        [JsonProperty(PropertyName = "enr_mail", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "crt_code_post", NullValueHandling = NullValueHandling.Ignore)]
        public string PostCode { get; set; }

        [JsonProperty(PropertyName = "ttl_adrs1", NullValueHandling = NullValueHandling.Ignore)]
        public string RecipientName { get; set; }

        [JsonProperty(PropertyName = "ttl_adrs2", NullValueHandling = NullValueHandling.Ignore)]
        public string AddressLine1 { get; set; }

        [JsonProperty(PropertyName = "ttl_adrs3", NullValueHandling = NullValueHandling.Ignore)]
        public string AddressLine2 { get; set; }

        [JsonProperty(PropertyName = "consumer_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ConsumerId { get; set; }

        [JsonProperty(PropertyName = "mobile_num", NullValueHandling = NullValueHandling.Ignore)]
        public string Mobile { get; set; }

        [JsonProperty(PropertyName = "crt_code_product", NullValueHandling = NullValueHandling.Ignore)]
        public string ProductCode { get; set; }

        [JsonProperty(PropertyName = "delivery_method", NullValueHandling = NullValueHandling.Ignore)]
        public string DeliveryMethod { get; set; }

        [JsonProperty(PropertyName = "suburb", NullValueHandling = NullValueHandling.Ignore)]
        public string Suburb { get; set; }

        [JsonProperty(PropertyName = "state_cde", NullValueHandling = NullValueHandling.Ignore)]
        public string State { get; set; }

        [JsonProperty(PropertyName = "mac_value", NullValueHandling = NullValueHandling.Ignore)]
        public string MacValue { get; set; }

        [JsonProperty(PropertyName = "cpt_devise", NullValueHandling = NullValueHandling.Ignore)]
        public string Device { get; set; }


        [JsonProperty(PropertyName = "institution_id")]
        public string InstitutionId { get; set; }

        [JsonProperty(PropertyName = "corporate_id")]
        public string CorporateId { get; set; }

        [JsonProperty(PropertyName = "crt_renew")]
        public string Renew { get; set; }

    }
}

