using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Response
{

    public class CreditApplicationResponsexxxx
    {
        //public bool BankStatementDone { get; set; }
        //public BusinessDetails BusinessDetails { get; set; }
        //public CreditApplicationSummary CreditApplicationSummary { get; set; }
        //public object ClientCardAddressId { get; set; }
        //public string CreditAssessment { get; set; }
        //public List<object> CreditCardDetails { get; set; }
        //public bool DirectDebitAuthorityDone { get; set; }
        //public DrivingLicence DrivingLicence { get; set; }
        //public FirstContact FirstContact { get; set; }
        //public bool IdCheckPassed { get; set; }
        //public MedicareCard MedicareCard { get; set; }
        //public bool RepaymentDone { get; set; }
        //public Address ResidentialAddress { get; set; }
        //public bool SmallTermsConfirmed { get; set; }
    }

    public class CreditApplicationSummary
    {
        public string CompanyName { get; set; }
        public decimal Available { get; set; }
        public decimal Balance { get; set; }
        public decimal Overdue { get; set; }
        public decimal Limit { get; set; }
        public DateTime DateJoined { get; set; }
    }

    public class AsicBusiness
    {
        public string Abn { get; set; }
        public string Acn { get; set; }
        public string CompanyLegalName { get; set; }
        public string CompanyName { get; set; }
        public string Type { get; set; }
    }

    public class FirstContact
    {
        public bool Accepted { get; set; }
        public string EmailAddress { get; set; }
        public string FamilyName { get; set; }
        public string FirstGivenName { get; set; }
        public string MobilePhoneNumber { get; set; }
        public int PartyId { get; set; }
        public object Uuid { get; set; }
    }

}
