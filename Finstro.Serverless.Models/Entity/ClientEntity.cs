using Dapper.Contrib.Extensions;

namespace Finstro.Serverless.Models.Entity
{

    [Table("client")]
    public class ClientEntity
    {
        [Key]
        public long Party_id { get; set; }

        public int Status { get; set; }

    }
}
