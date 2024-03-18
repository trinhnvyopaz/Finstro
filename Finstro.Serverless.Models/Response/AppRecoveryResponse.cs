using Finstro.Serverless.Models.Dynamo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Finstro.Serverless.Models.Response
{
    public class AppRecoveryResponse
    {
        public int Id { get; set; }
        public string UserSubId { get; set; }
        public string CreditApplicationStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ExternalId { get; set; }
        public string CreditAssessmentStatus { get; set; }
        public BusinessDetails BusinessDetails { get; set; }
        public Address ClientCardAddress { get; set; }
        public List<CreditCardDetail> CreditCardDetails { get; set; }
        public DrivingLicence DrivingLicence { get; set; }
        public List<Contact> Contacts { get; set; }
        public Contact FirstContact { get; set; }
        public MedicareCard MedicareCard { get; set; }
        public Address ResidentialAddress { get; set; }
        public ThreatMetrix ThreatMetrix { get; set; }
        public IdImages IdImages { get; set; }
        public List<FinstroCardDetail> FinstroCards { get; set; }
        public bool BankStatementDone { get; set; }
        public bool DirectDebitAuthorityDone { get; set; }
        public bool IdCheckPassed { get; set; }
        public bool RepaymentDone { get; set; }
        public bool SmallTermsConfirmed { get; set; }
        public Bankdata BankData { get; set; }

        [JsonIgnore]
        public List<BankStatementProcess> BankStatementProcess { get; set; }
    }
}
