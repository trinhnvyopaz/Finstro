using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Finstro.Serverless.Models.Entity;
using Finstro.Serverless.Models.Response;

namespace Finstro.Serverless.Dapper.Repository
{
    public sealed class EmailTemplateRepository : Repository<EmailTemplateEntity>
    {
        public EmailTemplateRepository() : base("email_template", "id") { }        public EmailTemplateRepository(string tableName) : base(tableName) { }        public EmailTemplateRepository(IDatabaseConnectionFactory connectionFactory, string tableName) : base(connectionFactory, tableName) { }

        

        public EmailTemplateEntity GetTemplateByType(string templateType)
        {
            object param = new
            {
                TemplateType = templateType
            };

            return Get<EmailTemplateEntity>($@"
                                 SELECT id,
		                                file_text,
                                        is_active,
                                        template_type
                                  FROM email_template
                                 WHERE template_type = @TemplateType; ", param).FirstOrDefault();
        }


    }


}
