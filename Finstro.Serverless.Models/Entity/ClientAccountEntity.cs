using Dapper.Contrib.Extensions;

namespace Finstro.Serverless.Models.Entity
{

    [Table("client_account")]
    public class ClientAccountEntity
    {
        [Key]        public long Client_account_id { get; set; }

        public string Account_name { get; set; }

        public int Status { get; set; }

        public string Type { get; set; }

        public long Party_id { get; set; }

        public long Master_setting_id { get; set; }

        public decimal Facility_limit { get; set; }

    }
}
