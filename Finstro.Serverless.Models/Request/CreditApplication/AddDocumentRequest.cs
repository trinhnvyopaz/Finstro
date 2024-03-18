using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Request.CreditApplication
{
    public class AddDocumentRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public IFormFileCollection File { get; set; }

    }
}
