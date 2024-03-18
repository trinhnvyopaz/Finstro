using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Request.CreditApplication
{
    public class IndividualCreditCheckResult
    {
        public int AgeOfFile { get; set; }
        public float AdverseScore { get; set; }
        public int CreditScore { get; set; }
        public int Directorships { get; set; }
        public int BankruptOnlyInsolvencies { get; set; }
        public int ConsumerDefaults { get; set; }
        public int ConsumerEnquiries { get; set; }
        public int CommercialEnquiries { get; set; }
        public int Judgements { get; set; }
        public int WritsSummons { get; set; }
        public int FileNotes { get; set; }
        public int Proprietorships { get; set; }
        public int ExternalAdmin { get; set; }
        public int AgeOfSoleTrader { get; set; }
        public int TelcoDefaults12Months { get; set; }
    }

}
