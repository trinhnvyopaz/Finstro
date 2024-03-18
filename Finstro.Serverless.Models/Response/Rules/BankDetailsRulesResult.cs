using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Response.Rules
{
    public class BankDetailsRulesResult
    {
        public float AverageMonthlyCredits { get; set; }
        public float Average6MonthsTurnover { get; set; }
        public float Average12MonthsTurnoverWithoutLoans { get; set; }
        public int NumberPositiveCashflowMonth { get; set; }
        public float Average6MonthsTurnoverWithoutLoans { get; set; }
        public float TotalNetCashflow { get; set; }
        public float CreditinaMonth { get; set; }
        public float TotalCreditLessLoanandInternalTransfer { get; set; }
        public float GamblingTransactions { get; set; }
        public float DishonoursInLast90Days { get; set; }
        public float CenterlinkPayments { get; set; }
        public float TotalCredit { get; set; }
    }

}
