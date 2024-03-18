using Dapper.Contrib.Extensions;

namespace Finstro.Serverless.Models.Entity
{

    [Table("email_template")]
    public class EmailTemplateEntity
    {
        [Key]
        public long Id { get; set; }

        public string File_text { get; set; }

        public bool Is_active { get; set; }

        public string Template_type { get; set; }

    }
}
