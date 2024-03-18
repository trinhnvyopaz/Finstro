using System;
using ServiceStack.DataAnnotations;

namespace Finstro.Serverless.Models.Dynamo
{

    public class ThreatMetrixResult
    {

        [AutoIncrement]
        public int Id { get; set; }

        [HashKey]
        public string UserSubId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public string account_address_country { get; set; }
        public string account_first_name { get; set; }
        public string account_last_name { get; set; }
        public string account_login { get; set; }
        public string account_login_first_seen { get; set; }
        public string account_login_last_event { get; set; }
        public string account_login_last_update { get; set; }
        public string account_login_result { get; set; }
        public string account_login_score { get; set; }
        public string account_login_worst_score { get; set; }
        public string account_name { get; set; }
        public string account_name_first_seen { get; set; }
        public string account_name_last_event { get; set; }
        public string account_name_last_update { get; set; }
        public string account_name_result { get; set; }
        public string account_name_score { get; set; }
        public string account_name_worst_score { get; set; }
        public string agent_app_info { get; set; }
        public Agent_Battery_Status agent_battery_status { get; set; }
        public string agent_brand { get; set; }
        public string agent_connection_type { get; set; }
        public string agent_device_id { get; set; }
        public string agent_device_state { get; set; }
        public string agent_language { get; set; }
        public string agent_locale { get; set; }
        public string agent_model { get; set; }
        public string agent_os { get; set; }
        public string agent_os_version { get; set; }
        public string agent_profiling_delta { get; set; }
        public string agent_publickey { get; set; }
        public string agent_publickey_hash { get; set; }
        public string agent_publickey_hash_first_seen { get; set; }
        public string agent_publickey_hash_last_event { get; set; }
        public string agent_publickey_hash_last_update { get; set; }
        public string agent_publickey_hash_result { get; set; }
        public string agent_publickey_hash_score { get; set; }
        public string agent_publickey_hash_worst_score { get; set; }
        public string agent_type { get; set; }
        public string agent_version { get; set; }
        public string api_call_datetime { get; set; }
        public string api_type { get; set; }
        public string api_version { get; set; }
        public string app_install_time { get; set; }
        public string apprep_selfhash { get; set; }
        public string apprep_selfhash_sha256 { get; set; }
        public string browser_language { get; set; }
        public string browser_string { get; set; }
        public string browser_string_hash { get; set; }
        public string challenger_policy { get; set; }
        public string challenger_policy_score { get; set; }
        public string[] challenger_reason_code { get; set; }
        public string challenger_review_status { get; set; }
        public string challenger_risk_rating { get; set; }
        public string custom_match_7 { get; set; }
        public string custom_match_8 { get; set; }
        public string custom_mobile_1 { get; set; }
        public string custom_mobile_2 { get; set; }
        public string customer_event_type { get; set; }
        public string device_first_seen { get; set; }
        public string device_id { get; set; }
        public string device_id_confidence { get; set; }
        public string device_last_event { get; set; }
        public string device_last_update { get; set; }
        public string device_match_result { get; set; }
        public string device_model { get; set; }
        public string device_name { get; set; }
        public string device_result { get; set; }
        public string device_score { get; set; }
        public string device_worst_score { get; set; }
        public string digital_id { get; set; }
        public string digital_id_confidence { get; set; }
        public string digital_id_confidence_rating { get; set; }
        public string digital_id_first_seen { get; set; }
        public string digital_id_last_event { get; set; }
        public string digital_id_last_update { get; set; }
        public string digital_id_result { get; set; }
        public string digital_id_trust_score { get; set; }
        public string digital_id_trust_score_rating { get; set; }
        public string[] digital_id_trust_score_reason_code { get; set; }
        public string dns_ip { get; set; }
        public string[] dns_ip_assert_history { get; set; }
        public string[] dns_ip_attributes { get; set; }
        public string dns_ip_city { get; set; }
        public string dns_ip_connection_type { get; set; }
        public string dns_ip_geo { get; set; }
        public string dns_ip_home { get; set; }
        public string dns_ip_isp { get; set; }
        public string dns_ip_latitude { get; set; }
        public string dns_ip_line_speed { get; set; }
        public string dns_ip_longitude { get; set; }
        public string dns_ip_organization { get; set; }
        public string dns_ip_postal_code { get; set; }
        public string dns_ip_region { get; set; }
        public string dns_ip_routing_type { get; set; }
        public string enabled_ck { get; set; }
        public string enabled_fl { get; set; }
        public string enabled_im { get; set; }
        public string enabled_js { get; set; }
        public string[] enabled_services { get; set; }
        public string event_datetime { get; set; }
        public string event_type { get; set; }
        public string extra_port_conn { get; set; }
        public string flash_guid { get; set; }
        public string fuzzy_device_first_seen { get; set; }
        public string fuzzy_device_id { get; set; }
        public string fuzzy_device_id_confidence { get; set; }
        public string fuzzy_device_last_event { get; set; }
        public string fuzzy_device_last_update { get; set; }
        public string fuzzy_device_match_result { get; set; }
        public string fuzzy_device_result { get; set; }
        public string fuzzy_device_score { get; set; }
        public string fuzzy_device_worst_score { get; set; }
        public string headers_name_value_hash { get; set; }
        public string headers_order_string_hash { get; set; }
        public string http_os_sig_adv_mss { get; set; }
        public string http_os_sig_rcv_mss { get; set; }
        public string http_os_sig_snd_mss { get; set; }
        public string http_os_signature { get; set; }
        public string http_referer { get; set; }
        public string http_referer_domain { get; set; }
        public string http_referer_url { get; set; }
        public string image_loaded { get; set; }
        public string jb_root { get; set; }
        public string js_browser_string { get; set; }
        public string js_browser_string_hash { get; set; }
        public string local_ipv4 { get; set; }
        public string local_ipv6 { get; set; }
        public string org_id { get; set; }
        public string os { get; set; }
        public string os_fonts_hash { get; set; }
        public string os_fonts_number { get; set; }
        public string os_version { get; set; }
        public string policy { get; set; }
        public Policy_Details_Api policy_details_api { get; set; }
        public string policy_score { get; set; }
        public string profiled_domain { get; set; }
        public string profiled_url { get; set; }
        public string profiling_datetime { get; set; }
        public string[] reason_code { get; set; }
        public string request_duration { get; set; }
        public string request_id { get; set; }
        public string request_result { get; set; }
        public string review_status { get; set; }
        public string risk_rating { get; set; }
        public string screen_res { get; set; }
        public string service_type { get; set; }
        public string session_id { get; set; }
        public string session_id_query_count { get; set; }
        public string summary_risk_score { get; set; }
        public string system_state { get; set; }
        public string tcp_os_signature { get; set; }
        public string third_party_cookie { get; set; }
        public string time_zone { get; set; }
        public string time_zone_dst_offset { get; set; }
        public string[] tmx_reason_code { get; set; }
        public string tmx_risk_rating { get; set; }
        public Tmx_Variables tmx_variables { get; set; }
        public string true_ip { get; set; }
        public string[] true_ip_assert_history { get; set; }
        public string true_ip_city { get; set; }
        public string true_ip_connection_type { get; set; }
        public string true_ip_first_seen { get; set; }
        public string true_ip_geo { get; set; }
        public string true_ip_home { get; set; }
        public string true_ip_isp { get; set; }
        public string true_ip_last_event { get; set; }
        public string true_ip_last_update { get; set; }
        public string true_ip_latitude { get; set; }
        public string true_ip_line_speed { get; set; }
        public string true_ip_longitude { get; set; }
        public string true_ip_organization { get; set; }
        public string true_ip_organization_type { get; set; }
        public string true_ip_postal_code { get; set; }
        public string true_ip_region { get; set; }
        public string true_ip_result { get; set; }
        public string true_ip_routing_type { get; set; }
        public string true_ip_score { get; set; }
        public string true_ip_worst_score { get; set; }
        public string ua_browser { get; set; }
        public string ua_browser_alt { get; set; }
        public string ua_browser_version_alt { get; set; }
        public string ua_mobile { get; set; }
        public string ua_os { get; set; }
        public string ua_os_alt { get; set; }
        public string ua_os_version_alt { get; set; }
        public string ua_platform { get; set; }
        public string[] vpn_reason { get; set; }
        public string vpn_score { get; set; }
    }

    public class Agent_Battery_Status
    {
        public string level { get; set; }
        public string status { get; set; }
    }

    public class Policy_Details_Api
    {
        public Policy_Detail_Api[] policy_detail_api { get; set; }
    }

    public class Policy_Detail_Api
    {
        public string type { get; set; }
        public string id { get; set; }
        public Customer customer { get; set; }
    }

    public class Customer
    {
        public string score { get; set; }
        public string pvid { get; set; }
        public string review_status { get; set; }
        public string risk_rating { get; set; }
        public Rule[] rules { get; set; }
    }

    public class Rule
    {
        public string rid { get; set; }
        public string reason_code { get; set; }
        public string score { get; set; }
    }

    public class Tmx_Variables
    {
        public string _acclogin_local_velocity_hour { get; set; }
        public string _acctemails_per_exactid_hour { get; set; }
        public string _acnt_login_per_exactid_hour { get; set; }
        public string _acnt_login_per_smartid_hour { get; set; }
        public string _acntemails_per_exactid_gbl_hour { get; set; }
        public string _acntemails_per_smartid_hour { get; set; }
        public string _cc_per_exactid_hour { get; set; }
        public string _cc_per_smartid_gbl_hour { get; set; }
        public string _cc_per_smartid_hour { get; set; }
        public string _exactid_gbl_velocity_hour { get; set; }
        public string _exactid_local_velocity_hour { get; set; }
        public string _smartid_gbl_velocity_hour { get; set; }
        public string _smartid_local_velocity_hour { get; set; }
    }




}
