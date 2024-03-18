using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Request.CreditApplication
{
    public class CompanyCreditCheckResult
    {
        public CompanyCreditCheckResult() {
            Directors = new List<Director>();
        }
        public string EMAIL { get; set; }
        public string BusinessIndustry { get; set; }
        public float FailureScore { get; set; }
        public int AgeOfFile { get; set; }
        public float AdverseScore { get; set; }
        public int CreditScore { get; set; }
        public int AgeOfABNRegisteredForGST { get; set; }
        public int Defaults { get; set; }
        public int Defaults12Months { get; set; }
        public int DefaultsUnpaid { get; set; }
        public int Judgements { get; set; }
        public int WritsSummons { get; set; }
        public int FileNotes { get; set; }
        public int DisqualifiedDirectors { get; set; }
        public int Petitions { get; set; }
        public int ExternalAdmin { get; set; }
        public int MercantileAgent { get; set; }
        public int MercantileEnquiries { get; set; }
        public int CreditEnquiries { get; set; }
        public int CreditEnquiries12Months { get; set; }
        public int TelcoDefaults12MonthsAmount { get; set; }
        public int TelcoDefaults12MonthsCount { get; set; }
        public int UtilityDefaults12MonthsAmount { get; set; }
        public int UtilityDefaults12MonthsCount { get; set; }
        public int PPSR { get; set; }
        [JsonProperty(PropertyName = "ONBOARDING_DIRECTOR_VERIFIED")]
        public bool OnboardingDirectorVerified { get; set; }


        public List<Director> Directors { get; set; }
    }


    public class Director
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CreditScore { get; set; }
        public int AgeOfFile { get; set; }
        public float AdverseScore { get; set; }
        public int Defaults { get; set; }
        public int Judgements { get; set; }
        public int WritsSummons { get; set; }
        public int FileNotes { get; set; }
        public int AgeOfDirector { get; set; }
        public int DirectorAppointmentDate { get; set; }
        public int Proprietorships { get; set; }
        public int CreditEnquiries { get; set; }
        public int DisqualifiedDirectors { get; set; }
    }

}
