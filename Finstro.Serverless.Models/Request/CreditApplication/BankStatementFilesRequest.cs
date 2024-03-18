using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Request.CreditApplication
{

    public class BankStatementFilesRequest
    {
        public string ReferrerCode { get; set; }
        public string Name { get; set; }
        public int FileCount { get; set; }
        public IFormFileCollection Files { get; set; }

    }
}
