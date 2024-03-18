using System;
using System.Collections.Generic;
using Finstro.Serverless.Models.Request.CreditApplication;
using Finstro.Serverless.Models.Response;
using Finstro.Serverless.Models.Response.Incc;
using Finstro.Serverless.Models.Response.Rules;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ServiceStack.Aws.DynamoDb;
using ServiceStack.DataAnnotations;

namespace Finstro.Serverless.Models.Dynamo
{
    public class CreditApplication
    {
        [AutoIncrement]
        public int Id { get; set; }

        [HashKey]
        public string UserSubId { get; set; }
        public string CreditApplicationStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public string ExternalId { get; set; }
        public string CreditAssessmentStatus { get; set; }

        public BusinessDetails BusinessDetails { get; set; }
        public CreditApplicationSummary CreditApplicationSummary { get; set; }
        public Address ClientCardAddress { get; set; }
        public List<CreditCardDetail> CreditCardDetails { get; set; }
        public DrivingLicence DrivingLicence { get; set; }
        public List<Contact> Contacts { get; set; }
        public MedicareCard MedicareCard { get; set; }
        public Address ResidentialAddress { get; set; }
        public ThreatMetrix ThreatMetrix { get; set; }
        public IdImages IdImages { get; set; }
        public List<FinstroCardDetail> FinstroCards { get; set; }
        public List<CreditCheckID> CreditChecks { get; set; }


        public bool BankStatementDone { get; set; }
        public List<BankStatementProcess> BankStatementProcess { get; set; }
        public bool DirectDebitAuthorityDone { get; set; }
        public bool IdCheckPassed { get; set; }
        public bool RepaymentDone { get; set; }
        public bool SmallTermsConfirmed { get; set; }

        public int ClientCardAddressId { get; set; }

        public string ProductType { get; set; }
        public Dictionary<string, object> ProductSettings { get; set; }

        public UserDetail UserDetail { get; set; }

        public List<FinstroCardDetail> FinstroInactiveCards { get; set; }

        public List<SupportingDocument> SupportingDocuments { get; set; }

    }

    public class CreditApplicationSummary
    {
        public string CompanyName { get; set; }
        public decimal Available { get; set; }
        public decimal Balance { get; set; }
        public decimal Overdue { get; set; }
        public decimal Limit { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime DateJoined { get; set; }
    }

    public class BankStatementProcess
    {

        public BankStatementProcess()
        {
            Files = new List<BankStatementFiles>();
        }
        public string DataVersion { get; set; }
        public string Reference { get; set; }
        public DateTime SubmissionTime { get; set; }
        public Bankdata BankData { get; set; }
        public List<BankStatementFiles> Files { get; set; }
        public DateTime CreatedDate { get; set; }

    }

    public class BankStatementFiles
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public DateTime UploadDate { get; set; }

    }


    public class BusinessDetails
    {
        [Required]
        public AsicBusiness AsicBusiness { get; set; }
        [Required]
        public Address BusinessTradingAddress { get; set; }
        public List<BusinessCreditCheck> BusinessCreditcheck { get; set; }

        [Required]
        public decimal SelectedCreditAmount { get; set; }

        public string Category { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Website { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Skype { get; set; }
        public string Linkedin { get; set; }
        public string Other { get; set; }


        public DateTime? IncorporationDate { get; set; }
        public DateTime? GstDate { get; set; }
        public DateTime? TimeTrading { get; set; }

    }

    public class AsicBusiness
    {

        [Required]
        public string Abn { get; set; }
        public string Acn { get; set; }
        [Required]
        public string CompanyLegalName { get; set; }
        public string CompanyName { get; set; }
        [Required]
        public string Type { get; set; }
        public string BusinessNameId { get; set; }
    }

    public class Address
    {

        [Required]
        public int AddressId { get; set; }

        [Required]
        public string Country { get; set; }
        [Required]
        public string PostCode { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string StreetName { get; set; }
        public string StreetNumber { get; set; }
        public string StreetType { get; set; }
        [Required]
        public string Suburb { get; set; }
        public string UnitOrLevel { get; set; }
    }

    public class Contact
    {
        public Contact()
        {
            IsFirstContact = false;
            Uuid = Guid.NewGuid().ToString();
        }
        public bool Accepted { get; set; }

        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string FamilyName { get; set; }
        [Required]
        public string FirstGivenName { get; set; }
        [Required]
        public string MobilePhoneNumber { get; set; }
        public int PartyId { get; set; }
        public string Uuid { get; set; }
        public bool IsFirstContact { get; set; }

    }

    public class MedicareCard
    {
        public string CardColor { get; set; }
        public string CardNumber { get; set; }
        public int CardNumberRef { get; set; }

        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public int? IdentificationId { get; set; }
        public string MiddleInitial { get; set; }
        public string Surname { get; set; }
        public string ValidTo { get; set; }
    }

    public class DrivingLicence
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string Gender { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? DateOfBirth { get; set; }
        public string LicenceNumber { get; set; }
        public string State { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? ValidTo { get; set; }
    }

    class CustomDateTimeConverter : IsoDateTimeConverter
    {
        public CustomDateTimeConverter()
        {
            base.DateTimeFormat = "yyyy-MM-dd";
        }
    }

    public class CreditCardDetail
    {

        public string CardNumber { get; set; }        public string CardType { get; set; }        public string ExpiryDate { get; set; }        public string ExternalId { get; set; }        public bool MainAccount { get; set; }        public string Name { get; set; }        public string PaymentProvider { get; set; }        public string Status { get; set; }        public string Type { get; set; }
    }

    public class ThreatMetrix
    {
        public string SessionId { get; set; }
        public string Status { get; set; }
        public string RequestId { get; set; }
        public int RequestCount { get; set; }

        public string DeviceId { get; set; }
        public string PolicyScore { get; set; }
        public List<string> Reasons { get; set; }
        public string FileUrl { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class IdImages
    {
        public string IdFront { get; set; }
        public string IdBack { get; set; }
        public string Face { get; set; }
    }


    public class SupportingDocument
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public DateTime UploadedDate { get; set; }
    }

    public class Equifax
    {
        public int FailCount { get; set; }
    }

    public class FinstroCardDetail
    {
        public CreateCardResponse InccCard { get; set; }        public Address PostalAddress { get; set; }
    }
    public class CreditCheckID
    {
        public string Type { get; set; }        public bool Success { get; set; }        public string EnquiryId { get; set; }
        public int OverallPoints { get; set; }
        public string ResultUrl { get; set; }
        public int RequestCount { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class BusinessCreditCheck
    {
        public BusinessCreditCheck()
        {
            Files = new List<string>();
            RuleEngineResults = new List<RuleEngineResult>();
            BusinessCreditcheckId = Guid.NewGuid().ToString();
        }
        public string BusinessCreditcheckId { get; set; }
        public CompanyCreditCheckResult CompanyCreditCheckResult { get; set; }
        public IndividualCreditCheckResult IndividualCreditCheckResult { get; set; }
        public List<string> Files { get; set; }
        public List<RuleEngineResult> RuleEngineResults { get; set; }
        public string CreditCheckStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public BankDetailsRulesResult BankDetailsRules { get; set; }
        public CreditAssessmentRulesResult CreditAssessmentRules { get; set; }
    }

    public class RuleEngineResult
    {
        public string Code { get; set; }
        public string Type { get; set; }
        public string Input { get; set; }
        public string Result { get; set; }
    }



}

