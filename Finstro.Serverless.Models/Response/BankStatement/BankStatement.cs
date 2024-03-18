using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Finstro.Serverless.Models.Response
{


    public class BankStatement
    {
        public string DataVersion { get; set; }
        public string Reference { get; set; }
        public DateTime SubmissionTime { get; set; }
        public Bankdata BankData { get; set; }
        public List<DecisionMetric> DecisionMetrics { get; set; }
    }

    public class Bankdata
    {
        public string BankName { get; set; }
        public string BankSlug { get; set; }
        public List<BankAccount> BankAccounts { get; set; }
        public BankStatmentAddress UserAddress { get; set; }
    }

    public class BankStatmentAddress
    {
        public string Text { get; set; }
        public string StreetLine { get; set; }
        public string LevelNumber { get; set; }
        public string LevelType { get; set; }
        public string UnitNumber { get; set; }
        public string UnitType { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string StreetType { get; set; }
        public string Suburb { get; set; }
        public string StateCode { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public string CountryCode { get; set; }
    }

    public class BankAccount
    {
        public int Id { get; set; }
        public string AccountType { get; set; }
        public string AccountHolder { get; set; }
        public string AccountHolderType { get; set; }
        public string AccountName { get; set; }
        public string Bsb { get; set; }
        public string AccountNumber { get; set; }
        public string CurrentBalance { get; set; }
        public string AvailableBalance { get; set; }
        public List<Transaction> Transactions { get; set; }
        public Statementsummary StatementSummary { get; set; }
        public List<DayEndBalance> DayEndBalances { get; set; }
        public AdditionalDetails AdditionalDetails { get; set; }
        public List<StatementAnalysis> StatementAnalysis { get; set; }
    }

    public class Statementsummary
    {
        public string OpeningBalance { get; set; }
        public string TotalCredits { get; set; }
        public string TotalDebits { get; set; }
        public string ClosingBalance { get; set; }
        public string MinBalance { get; set; }
        public string MinDayEndBalance { get; set; }
        public int DaysInNegative { get; set; }
        public string SearchPeriodStart { get; set; }
        public string SearchPeriodEnd { get; set; }
        public string TransactionsStartDate { get; set; }
        public string TransactionsEndDate { get; set; }
    }

    public class AdditionalDetails
    {
        public string InterestRate { get; set; }
        public string Email { get; set; }
        public string OpenDate { get; set; }
        public string Phone { get; set; }
        public BankStatmentAddress AccountAddress { get; set; }
        public string BonusInterestRate { get; set; }
        public string DebitInterestRate { get; set; }
        public string InterestPaymentFrequency { get; set; }
        public string NextInterestPayment { get; set; }
        public string OverdraftLimit { get; set; }
        public string OverdraftRemaining { get; set; }
        public string OverdraftUsed { get; set; }
        public string CardholderRelationship { get; set; }
        public string CreditLimit { get; set; }
        public string LastPayment { get; set; }
        public string MinimumAmountDue { get; set; }
        public string MinimumAmountDueDate { get; set; }
        public string NextPaymentDue { get; set; }
    }

    public class Transaction
    {
        public string Date { get; set; }
        public string Text { get; set; }
        public float Amount { get; set; }
        public string Type { get; set; }
        public string Balance { get; set; }
        public List<Tag> Tags { get; set; }
    }

    public class Tag
    {
        public string Pending { get; set; }
        public string CreditDebit { get; set; }
        public string Category { get; set; }
        public string ThirdParty { get; set; }
        public string LenderType { get; set; }
    }

    public class DayEndBalance
    {
        public DateTime Date { get; set; }
        public string Balance { get; set; }
    }

    public class StatementAnalysis
    {
        public AnalysisCategory AnalysisCategory { get; set; }
    }

    public class AnalysisCategory
    {
        public string Name { get; set; }
        public List<AnalysisPoint> AnalysisPoints { get; set; }
        public List<TransactionGroup> TransactionGroups { get; set; }
    }

    public class AnalysisPoint
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
    }

    public class TransactionGroup
    {
        public string Name { get; set; }
        public List<AnalysisPoint> AnalysisPoints { get; set; }
        public List<Transaction> Transactions { get; set; }
    }


    public class DecisionMetric
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Descriptor { get; set; }
        public string Type { get; set; }
        public object Value { get; set; }
    }



}
