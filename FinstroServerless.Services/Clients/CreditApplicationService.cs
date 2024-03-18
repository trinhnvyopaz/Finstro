using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Finstro.Serverless.Dapper.Repository;
using Finstro.Serverless.Models.Request;
//using Finstro.Serverless.Models.Response;
using Finstro.Serverless.Helper;
using Finstro.Serverless.Models;
using System.Dynamic;
using Finstro.Serverless.DynamoDB;
using Finstro.Serverless.Models.Dynamo;
using Finstro.Serverless.Models.Request.CreditApplication;
using Amazon.S3;
using Amazon;
using ServiceStack.IO;
using FinstroServerless.Services.Common;
using ServiceStack.Aws;
using Amazon.S3.Model;
using ServiceStack.Text;
using ServiceStack;
using System.IO;
using System.Threading.Tasks;
using FinstroServerless.Services.IdentityProvider;
using System.Reflection;
using System.Net;
using Finstro.Serverless.Models.Response.Equifax;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using FinstroServerless.Services.Incc;
using FinstroServerless.Services.Equifax;
using System.Globalization;
using Finstro.Serverless.Models.Request.Rules;
using FinstroServerless.Services.Fcc;
using Finstro.Serverless.Models.Response.Rules;
using System.Text.RegularExpressions;

namespace FinstroServerless.Services.Clients
{
    public class CreditApplicationService
    {
        private ClientRepository _clientRepository;
        private CreditApplicationDynamo _creditApplicationDynamo;
        private EquifaxService _equifax;

        private static readonly RegionEndpoint _region = RegionEndpoint.USEast2;

        public CreditApplicationService()
        {
            _clientRepository = new ClientRepository();
            _creditApplicationDynamo = new CreditApplicationDynamo();
            _equifax = new EquifaxService();
        }

        public void IDMatrixCheck(string userId)
        {
            try
            {
                var application = GetCreditApplicationForUser(userId);

                if (application.CreditChecks == null)
                    application.CreditChecks = new List<CreditCheckID>();

                var creditCheck = application.CreditChecks.FirstOrDefault(c => c.Type == EnumIDMetrixType.Identity.GetAttributeOfType<DescriptionAttribute>().Description);

                if (creditCheck == null)
                {
                    creditCheck = new CreditCheckID()
                    {
                        CreatedDate = DateTime.UtcNow,
                        Type = EnumIDMetrixType.Identity.GetAttributeOfType<DescriptionAttribute>().Description,
                        RequestCount = 0,
                        Success = false
                    };
                }

                if (creditCheck.RequestCount > 2)
                {
                    throw FinstroErrorType.CreditApplication.AttemptsExceeded;
                }

                string idMatrix = GetIDMatrixResult(application);

                if (!string.IsNullOrEmpty(creditCheck.ResultUrl))
                    _ = AwsHelper.RemoveFile(creditCheck.ResultUrl);

                string xmlUrl = UploadXml(application, EnumIdFileType.IdMetrix, idMatrix);

                var doc = new XmlDocument();
                doc.LoadXml(idMatrix);

                creditCheck.ResultUrl = xmlUrl;
                creditCheck.Success = doc.GetElementValue("ns5:overall-outcome") == "ACCEPT";
                creditCheck.RequestCount++;
                creditCheck.OverallPoints = Convert.ToInt32(doc.GetElementValue("ns5:total-points"));//response.Body.response.componentresponses.verificationresponse.verificationoutcome.totalpoints;
                creditCheck.EnquiryId = doc.GetElementValue("ns5:response", "enquiry-id");//response.Body.response.enquiryid;
                Envelope response = new Envelope();

                if (creditCheck.Success)
                {
                    application.IdCheckPassed = true;
                }

                //System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Envelope));

                //using (StringReader stringReader = new StringReader(idMatrix))
                //{
                //    response = (Envelope)serializer.Deserialize(stringReader);
                //}




                application.CreditChecks.Add(creditCheck);

                _creditApplicationDynamo.Update(application);

                if (creditCheck.RequestCount > 2)
                {
                    throw FinstroErrorType.CreditApplication.AttemptsExceeded;
                }

                if (!creditCheck.Success)
                {
                    throw FinstroErrorType.CreditApplication.IdentityCheckFailed;
                }

            }
            catch (Exception ex)
            {
                if (!(ex is FinstroErrorType))
                {
                    if (ex.Message.Contains("(500) Internal"))
                        return;

                    var error = FinstroErrorType.Schema.UnexpectedError;
                    error.Message = ex.Message;
                    throw error;
                }
                else
                    throw ex;
            }
        }
        public void ProcessBusinessRules(CreditApplication application, List<BankStatementRuleRequest> bankStatementRuleRequests)
        {
            var fccService = new FccServices("");

            BankDetailsRulesResult bankDetailsRulesResult = fccService.BankDetailsRulesProcess(bankStatementRuleRequests);

            var creditCheck = application.BusinessDetails.BusinessCreditcheck.OrderByDescending(c => c.CreatedDate).FirstOrDefault();

            if (creditCheck != null)
            {
                creditCheck.BankDetailsRules = bankDetailsRulesResult;
                _creditApplicationDynamo.Update(application);
            }
            if (creditCheck?.CompanyCreditCheckResult != null)
            {

                CreditAssessmentRuleRequest creditAssessmentRuleRequest = new CreditAssessmentRuleRequest()
                {
                    bankStatement = bankDetailsRulesResult,
                    CompanyCreditCheck = creditCheck.CompanyCreditCheckResult,
                    directors = creditCheck.CompanyCreditCheckResult.Directors
                };

                CreditAssessmentRulesResult creditAssessmentRulesResult = fccService.CreditAssessmentRulesProcess(creditAssessmentRuleRequest);

                if (creditAssessmentRulesResult.entries.Count > 0)
                {
                    creditCheck.RuleEngineResults = new List<RuleEngineResult>();
                    foreach (var item in creditAssessmentRulesResult.entries)
                    {
                        creditCheck.RuleEngineResults.Add(new RuleEngineResult()
                        {
                            Code = item.Value.code,
                            Input = item.Value.input,
                            Result = item.Value.result,
                            Type = item.Value.type
                        });
                    }
                }
                creditCheck.CreditAssessmentRules = creditAssessmentRulesResult;
                _creditApplicationDynamo.Update(application);
            }
        }
        public void BusinessCreditCheck(CreditApplication application, bool force = false)
        {

            if (application.BusinessDetails.BusinessCreditcheck == null)
                application.BusinessDetails.BusinessCreditcheck = new List<BusinessCreditCheck>();

            if (force || application.BusinessDetails.BusinessCreditcheck.Count == 0)
            {
                if (application.BusinessDetails.SelectedCreditAmount > AppSettings.FinstroSettings.AutoApproveCreditAmount)
                {
                    BusinessCreditCheck businessCreditcheck = new BusinessCreditCheck();
                    businessCreditcheck.CreditCheckStatus = EnumCreditAssessmentStatus.Refer.GetDescription();

                    if (application.BusinessDetails.AsicBusiness.Type != "SOLE_TRADER")
                    {
                        if (string.IsNullOrEmpty(application.BusinessDetails.AsicBusiness.Acn) ||
                            string.IsNullOrEmpty(application.BusinessDetails.AsicBusiness.BusinessNameId))
                        {

                            string orgIdResponse = _equifax.GetOrgIdResult(application);

                            var doc = new XmlDocument();
                            doc.LoadXml(orgIdResponse);


                            string xmlUrl = UploadXml(application, EnumIdFileType.OrgId, orgIdResponse);
                            businessCreditcheck.Files.Add(xmlUrl);

                            application.BusinessDetails.AsicBusiness.Acn = doc.GetElementValue("asic-organisation-number");
                            application.BusinessDetails.AsicBusiness.BusinessNameId = doc.GetElementValue("veda-business-name-id");

                            if (string.IsNullOrEmpty(application.BusinessDetails.AsicBusiness.Acn))
                                application.BusinessDetails.AsicBusiness.Acn = doc.GetElementValue("organisation-number");

                            if (string.IsNullOrEmpty(application.BusinessDetails.AsicBusiness.Acn))
                            {
                                application.BusinessDetails.AsicBusiness.Acn = application.BusinessDetails.AsicBusiness.Abn.Substring(2, application.BusinessDetails.AsicBusiness.Abn.Length - 2);
                            }

                        }
                        if (application.BusinessDetails.AsicBusiness.Type == "COMPANY")
                        {

                            var companyCreditCheck = _equifax.CompanyCreditCheck(application, businessCreditcheck);
                            if (companyCreditCheck != null)
                            {
                                companyCreditCheck.EMAIL = application.Contacts.FirstOrDefault().EmailAddress;
                                businessCreditcheck.CompanyCreditCheckResult = companyCreditCheck;
                            }
                        }
                    }
                    else
                    {
                        var individualCreditCheck = _equifax.IndividualCreditCheck(application, businessCreditcheck);
                        businessCreditcheck.IndividualCreditCheckResult = individualCreditCheck;


                    }



                    businessCreditcheck.CreatedDate = DateTime.UtcNow;

                    application.BusinessDetails.BusinessCreditcheck.Add(businessCreditcheck);

                    _creditApplicationDynamo.Update(application);
                }
            }
        }



        #region Xml Helpers


        private static string GetIDMatrixResult(CreditApplication application)
        {
            try
            {
                string iDMatrixRequest = ExtractIDMatrixRequest(application);

                if (application.DrivingLicence != null)
                    iDMatrixRequest = ExtractDriversLicence(application, iDMatrixRequest);
                else
                {
                    iDMatrixRequest = iDMatrixRequest.Replace("%LICENCECONSENT%", "")
                        .Replace("%DRIVERSINFO%", "");
                }

                if (application.MedicareCard != null)
                    iDMatrixRequest = ExtractMedicare(application, iDMatrixRequest);
                else
                {
                    iDMatrixRequest = iDMatrixRequest.Replace("%MEDICARECONSENT%", "")
                        .Replace("%MEDICAREINFO%", "");
                }

                iDMatrixRequest = iDMatrixRequest.Replace("<idm:other-given-name></idm:other-given-name>", "")
                                .Replace("<idm:unit-number></idm:unit-number>", "")
                                .Replace("<idm:home-phone-number verify=\"false\"></idm:home-phone-number>", "")
                                .Replace("<idm:card-colour></idm:card-colour>", "")
                                .Replace("<idm:date-of-expiry></idm:date-of-expiry>", "")
                                .Replace("<idm:middle-name-on-card></idm:middle-name-on-card>", "");


                string destinationUrl = AppSettings.Equifax.Url + "/cta/sys2/idmatrix-v4";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
                byte[] bytes;
                bytes = System.Text.Encoding.ASCII.GetBytes(iDMatrixRequest);
                request.ContentType = "text/xml; encoding='utf-8'";
                request.ContentLength = bytes.Length;
                request.Method = "POST";
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                HttpWebResponse response;
                response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    string responseStr = new StreamReader(responseStream).ReadToEnd();
                    return responseStr;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private static string ExtractIDMatrixRequest(CreditApplication application)
        {
            Dictionary<string, string> content = new Dictionary<string, string>();
            content.Add("USER", AppSettings.Equifax.Username);
            content.Add("PWD", AppSettings.Equifax.Password);
            var contact = application.Contacts.FirstOrDefault();

            string firstName = contact.FirstGivenName;
            string lastName = contact.FamilyName;
            bool isFirstContact = true;



            content.Add("CID", application.Id.ToString());

            if (application.DrivingLicence != null)
            {
                content.Add("DOB", application.DrivingLicence.DateOfBirth?.ToString("yyyy-MM-dd"));
                content.Add("GENDER", application.DrivingLicence.Gender == "M" ? "male" : "female");

                if (isFirstContact)
                {
                    firstName = application.DrivingLicence.FirstName;
                    lastName = application.DrivingLicence.Surname;
                    isFirstContact = false;
                }
            }
            else
            {
                if (application.MedicareCard != null)
                {
                    content.Add("DOB", application.MedicareCard.DateOfBirth?.ToString("yyyy-MM-dd"));
                    content.Add("GENDER", application.MedicareCard.Gender == "M" ? "male" : "female");

                    if (isFirstContact)
                    {
                        firstName = application.MedicareCard.FirstName;
                        lastName = application.MedicareCard.Surname;
                        isFirstContact = false;
                    }
                }
            }


            content.Add("UNITNO", application.ResidentialAddress.UnitOrLevel);
            content.Add("STREETNO", application.ResidentialAddress.StreetNumber);
            content.Add("STREETNAME", application.ResidentialAddress.StreetName);
            content.Add("STREETTYPE", application.ResidentialAddress.StreetType);
            content.Add("SUBURB", application.ResidentialAddress.Suburb);
            content.Add("STATE", application.ResidentialAddress.State);
            content.Add("POSTCODE", application.ResidentialAddress.PostCode);
            content.Add("EMAIL", contact.EmailAddress);
            content.Add("ALTEMAIL", "");
            content.Add("HOMEPHONE", "");
            content.Add("MOBILEPHONE", ValidationHelper.FormatPhoneNumberNational(contact.MobilePhoneNumber, "AU").Replace(" ", ""));
            content.Add("ENQUIRYID", application.ExternalId);

            content.Add("FAMILY", lastName);
            content.Add("FIRST", firstName);
            content.Add("OTHER", "");


            string requestXml = string.Empty;

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetName().Name + ".Equifax.Template.IDMatrixRequest.xml";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    requestXml = reader.ReadToEnd();
                }
            }

            foreach (var item in content)
            {
                requestXml = requestXml.Replace($"%{item.Key}%", item.Value);
            }

            var x = requestXml;
            return requestXml;
        }
        private static string ExtractDriversLicence(CreditApplication application, string xmlData)
        {
            Dictionary<string, string> content = new Dictionary<string, string>();
            content.Add("STATE", application.DrivingLicence.State);
            content.Add("LICENCENUMBER", application.DrivingLicence.LicenceNumber);
            content.Add("EXPIRY", application.DrivingLicence.ValidTo?.ToString("yyyy-MM-dd"));
            content.Add("CARDNUMBER", string.Empty);

            string requestXml = string.Empty;

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "FinstroServerless.Services.Equifax.Template.DriversLicence.xml";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    requestXml = reader.ReadToEnd();
                }
            }
            foreach (var item in content)
            {
                requestXml = requestXml.Replace($"%{item.Key}%", item.Value);
            }

            string result = xmlData.Replace("%DRIVERSINFO%", requestXml)
                .Replace("%LICENCECONSENT%", @"<idm:consent status=""1"">DL</idm:consent>");

            return result;
        }
        private static string ExtractMedicare(CreditApplication application, string xmlData)
        {
            Dictionary<string, string> content = new Dictionary<string, string>();
            content.Add("CARDNUMBER", application.MedicareCard.CardNumber);
            content.Add("REFERENCENUMBER", application.MedicareCard.CardNumberRef.ToString());
            content.Add("MIDDLENAME", application.MedicareCard.MiddleInitial);
            content.Add("CARDCOLOR", application.MedicareCard.CardColor.ToString());
            content.Add("EXPIRY", application.MedicareCard.ValidTo);

            string requestXml = string.Empty;

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "FinstroServerless.Services.Equifax.Template.Medicare.xml";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    requestXml = reader.ReadToEnd();
                }
            }
            foreach (var item in content)
            {
                requestXml = requestXml.Replace($"%{item.Key}%", item.Value);
            }

            string result = xmlData.Replace("%MEDICAREINFO%", requestXml)
                .Replace("%MEDICARECONSENT%", @"<idm:consent status=""1"">MEDICARE-CARD</idm:consent>");

            return result;
        }

        #endregion


        public CreditApplication GetCreditApplicationOld(int creditApplicationId)//actually acting as BusinessPartyId for the moment
        {
            try
            {

                var response = new CreditApplication();
                response.BusinessDetails = new BusinessDetails();
                dynamic data = _clientRepository.GetCreditApplicationSummary(creditApplicationId);

                response.CreditApplicationSummary = new CreditApplicationSummary()
                {
                    Available = data.Available,
                    Balance = data.Balance,
                    CompanyName = data.CompanyName,
                    DateJoined = data.DateJoined,
                    Limit = data.Limit,
                    Overdue = data.Overdue
                };


                response.BusinessDetails.BusinessTradingAddress = new Address()
                {
                    //AddressId = data.address_id,
                    Country = data.country,
                    PostCode = data.post_code,
                    State = data.state,
                    StreetName = data.street_name,
                    StreetNumber = data.street_number,
                    StreetType = data.street_type,
                    Suburb = data.suburb,
                    UnitOrLevel = data.unit_or_level
                };

                response.BusinessDetails.AsicBusiness = new AsicBusiness()
                {
                    Abn = data.abn,
                    Acn = data.acn,
                    CompanyLegalName = data.company_legal_name,
                    CompanyName = data.company_name,
                    Type = data.BusinessType
                };



                data = _clientRepository.GetCreditApplicationFirstContact(creditApplicationId);


                //response.FirstContact = new FirstContact()
                //{
                //    Accepted = data.first_contact_accepted == null ? null : Convert.ToBoolean(data.first_contact_accepted),
                //    EmailAddress = data.first_contact_email,
                //    FamilyName = data.first_contact_last_name,
                //    FirstGivenName = data.first_contact_first_name,
                //    MobilePhoneNumber = data.first_contact_mobile,
                //    PartyId = data.first_contact_party_id == null ? null : Convert.ToInt32(data.first_contact_party_id),
                //    Uuid = data.first_contact_uuid
                //};



                //data = _clientRepository.GetCreditApplicationMedicare(response.FirstContact.PartyId);


                //if (data != null && !string.IsNullOrEmpty(data.medicare_card_number))
                //{
                //    response.MedicareCard = new MedicareCard()
                //    {

                //        CardColor = data.medicare_card_color,
                //        CardNumber = data.medicare_card_number,
                //        CardNumberRef = data.medicare_card_number_ref,
                //        IdentificationId = data.medicare_identification_id == null ? null : Convert.ToInt32(data.medicare_identification_id),
                //        ValidTo = data.medicare_valid_to == null ? null : Convert.ToDateTime(data.medicare_valid_to).ToString("MM/yyyy"),

                //    };
                //}

                //data = _clientRepository.GetCreditApplicationDriversLicense(response.FirstContact.PartyId);

                //response.DrivingLicence = new DrivingLicence()
                //{
                //    //IdentificationId = data.driver_licence_identification_id == null ? null : Convert.ToInt32(data.driver_licence_identification_id),
                //    //CardNumber = data.driver_licence_card_number,
                //    DateOfBirth = data.driver_licence_date_of_birth == null ? null : Convert.ToDateTime(data.driver_licence_date_of_birth),
                //    LicenceNumber = data.driver_licence_licence_number,
                //    State = data.driver_licence_state,
                //    ValidTo = data.driver_licence_valid_to == null ? null : Convert.ToDateTime(data.driver_licence_valid_to),
                //};

                //data = _clientRepository.GetCreditApplicationBusinessAddress(creditApplicationId);


                response.ResidentialAddress = new Address()
                {
                    //AddressId = data.residential_address_id,
                    Country = data.residential_country,
                    PostCode = data.residential_post_code,
                    State = data.residential_state,
                    StreetName = data.residential_street_name,
                    StreetNumber = data.residential_street_number,
                    StreetType = data.residential_street_type,
                    Suburb = data.residential_suburb,
                    UnitOrLevel = data.residential_unit_or_level
                };




                response.BusinessDetails.SelectedCreditAmount = Convert.ToDecimal(data.SelectedCreditAmount);
                return response;            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public Finstro.Serverless.Models.Response.AppRecoveryResponse GetAppRecovery(string userId, bool forceCardUpdate = false)
        {
            var application = _creditApplicationDynamo.GetAppRecovery(userId);


            if (application.BankStatementProcess?.Count() > 0)
            {
                application.BankData = application.BankStatementProcess.OrderByDescending(b => b.CreatedDate).FirstOrDefault().BankData;
            }



            if (forceCardUpdate)
            {

                InccService inccService = new InccService();
                if (application.FinstroCards != null)
                {

                    var card = application.FinstroCards.FirstOrDefault();

                    if (card != null)
                    {
                        string cardToken = card.InccCard.CardToken;
                        var token30Min = inccService.Get30MinToken(userId, cardToken);


                        var cardDataRequest = new Finstro.Serverless.Models.Request.Incc.GetCardDataRequest()
                        {
                            Token30min = token30Min.Token30min,
                            CardToken = cardToken,
                            CallType = EnumInccCardCallType.MaskedData.GetDescription()
                        };
                        var cardData = inccService.GetCardData(userId, cardDataRequest);
                        application = _creditApplicationDynamo.GetAppRecovery(userId);
                    }
                }
            }

            if (application.Contacts == null || application.Contacts.Count == 0)
            {
                CreateFirstContact(userId);
                application = _creditApplicationDynamo.GetAppRecovery(userId);
            }

            var firstContact = application.Contacts.FirstOrDefault(c => c.IsFirstContact);

            if (firstContact == null)
                firstContact = application.Contacts.FirstOrDefault();

            application.FirstContact = firstContact;

            return application;

        }

        private void CreateFirstContact(string userId, CreditApplication application = null)
        {
            if (application == null)
                application = _creditApplicationDynamo.GetCreditApplicationForUser(userId);

            string userName = userId;
            UserService _userService = new UserService();
            var cognitoUser = _userService.GetUserByAttribute(CognitoAttribute.UserName, userName).Result;

            application.Contacts = new List<Contact>();

            application.Contacts.Add(new Contact()
            {

                EmailAddress = cognitoUser.Attributes.FirstOrDefault(a => a.Name == CognitoAttribute.PreferredUsername.AttributeName)?.Value,
                FamilyName = cognitoUser.Attributes.FirstOrDefault(a => a.Name == CognitoAttribute.FamilyName.AttributeName)?.Value,
                FirstGivenName = cognitoUser.Attributes.FirstOrDefault(a => a.Name == CognitoAttribute.GivenName.AttributeName)?.Value,
                MobilePhoneNumber = cognitoUser.Attributes.FirstOrDefault(a => a.Name == CognitoAttribute.PhoneNumber.AttributeName)?.Value,
                Accepted = cognitoUser.Attributes.FirstOrDefault(a => a.Name == CognitoAttribute.TermsAccepted.AttributeName)?.Value == "1",
                IsFirstContact = true

            });
            _creditApplicationDynamo.Update(application);
        }

        public CreditApplication GetCreditApplicationForUser(string userId)
        {
            var application = _creditApplicationDynamo.GetCreditApplicationForUser(userId);

            if (application.Contacts == null || application.Contacts.Count == 0)
            {
                CreateFirstContact(userId, application);
            }
            return application;

        }

        public string GetPaymentAccount(string userId)
        {
            var application = _creditApplicationDynamo.GetCreditApplicationForUser(userId);

            string paymentMethod = "NONE";

            if (application.CreditCardDetails != null && application.CreditCardDetails.Count > 0)
            {
                var card = application.CreditCardDetails.FirstOrDefault(c => c.MainAccount);

                if (card == null)
                {
                    card = application.CreditCardDetails.FirstOrDefault();
                }

                paymentMethod = card.Type.ToUpper();
            }

            return paymentMethod;

        }


        public CreditApplication GetCreditApplication(string Id)
        {
            var application = _creditApplicationDynamo.GetCreditApplication(Id);

            return application;

        }

        public IEnumerable<Finstro.Serverless.Models.Response.ClientListResponse> GetCreditApplications()
        {
            var applications = _creditApplicationDynamo.GetCreditApplications().OrderByDescending(a => a.CreatedDate).Take(50);

            return applications;

        }

        public BusinessDetails SaveBusinessDetail(string userId, BusinessDetails businessDetails)
        {

            if (businessDetails.BusinessTradingAddress == null)
                throw FinstroErrorType.Schema.ErrorNotEmpty("Business Trading Address");

            if (businessDetails.AsicBusiness == null)
                throw FinstroErrorType.Schema.ErrorNotEmpty("Business Details");

            if (businessDetails.SelectedCreditAmount < 500)
                throw FinstroErrorType.Schema.SelectedCreditAmount;

            ValidationHelper.CheckRequired(businessDetails.BusinessTradingAddress);

            var application = _creditApplicationDynamo.GetCreditApplicationForUser(userId);

            if (application.BusinessDetails == null)
            {
                application.BusinessDetails = new BusinessDetails();
            }

            application.BusinessDetails.BusinessTradingAddress = businessDetails.BusinessTradingAddress;
            application.BusinessDetails.AsicBusiness = businessDetails.AsicBusiness;
            application.BusinessDetails.SelectedCreditAmount = businessDetails.SelectedCreditAmount;


            application.BusinessDetails.Category = businessDetails.Category;
            application.BusinessDetails.Email = businessDetails.Email;
            application.BusinessDetails.PhoneNumber = businessDetails.PhoneNumber;
            application.BusinessDetails.Website = businessDetails.Website;
            application.BusinessDetails.Facebook = businessDetails.Facebook;
            application.BusinessDetails.Twitter = businessDetails.Twitter;
            application.BusinessDetails.Instagram = businessDetails.Instagram;
            application.BusinessDetails.Skype = businessDetails.Skype;
            application.BusinessDetails.Linkedin = businessDetails.Linkedin;
            application.BusinessDetails.Other = businessDetails.Other;


            _creditApplicationDynamo.Update(application);


            if (application.Contacts == null || application.Contacts.Count == 0)
            {
                CreateFirstContact(userId);
            }
            return businessDetails;

        }

        public Address SaveResidentialAddress(string userId, Address address)
        {


            var application = _creditApplicationDynamo.GetCreditApplicationForUser(userId);

            if (application.BusinessDetails == null)
            {
                application.BusinessDetails = new BusinessDetails();
            }

            application.ResidentialAddress = address;

            _creditApplicationDynamo.Update(application);

            return address;

        }

        public bool IsMedicareFormatValid(string medicareNumber)
        {
            if (!(medicareNumber?.Length >= 10 && medicareNumber.Length < 12) || !medicareNumber.All(char.IsDigit))
                return false;

            var digits = medicareNumber.Select(c => (int)char.GetNumericValue(c)).ToArray();
            return digits[8] == GetMedicareChecksum(digits.Take(8).ToArray());
        }

        private int GetMedicareChecksum(int[] digits)
        {
            return digits.Zip(new[] { 1, 3, 7, 9, 1, 3, 7, 9 }, (m, d) => m * d).Sum() % 10;
        }

        public void SaveMedicare(string userId, MedicareCard medicare)
        {

            long n;

            if (!IsMedicareFormatValid(medicare.CardNumber.ToString()))
                throw FinstroErrorType.Schema.MedicareNumber;

            if (!long.TryParse(medicare.CardNumber, out n))
                throw FinstroErrorType.Schema.MedicareNumber;

            if (medicare.CardNumberRef < 1 || medicare.CardNumberRef > 9)
                throw FinstroErrorType.Schema.MedicareReference;

            DateTime dateValue;

            try
            {
                if (!DateTime.TryParseExact(medicare.ValidTo, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
                    throw FinstroErrorType.Schema.MedicareValidTo;

            }
            catch
            {
                if (!DateTime.TryParseExact(medicare.ValidTo, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
                    throw FinstroErrorType.Schema.MedicareValidTo;
            }

            if (dateValue.Year < DateTime.UtcNow.Year)
                throw FinstroErrorType.Schema.MedicareValidTo;
            else
            {
                if (dateValue.Year == DateTime.UtcNow.Year && dateValue.Month < DateTime.UtcNow.Month)
                    throw FinstroErrorType.Schema.MedicareValidTo;
            }

            medicare.ValidTo = dateValue.ToString("yyyy-MM-dd");

            if (medicare.DateOfBirth == null)
                throw FinstroErrorType.Schema.MedicareDOB;


            if (string.IsNullOrEmpty(medicare.FirstName))
                throw FinstroErrorType.Schema.MedicareName;


            if (!(new string[] { "G", "B", "Y" }).Contains(medicare.CardColor))
                throw FinstroErrorType.Schema.MedicareColour;

            var application = _creditApplicationDynamo.GetCreditApplicationForUser(userId);

            if (application.MedicareCard == null)
            {
                application.MedicareCard = new MedicareCard();
            }

            application.MedicareCard = medicare;

            _creditApplicationDynamo.Update(application);

        }

        public void SaveDriversLicence(string userId, DrivingLicence drivingLicence)
        {
            var application = _creditApplicationDynamo.GetCreditApplicationForUser(userId);

            if (application.DrivingLicence == null)
            {
                application.DrivingLicence = new DrivingLicence();
            }

            application.DrivingLicence = drivingLicence;

            _creditApplicationDynamo.Update(application);

        }

        public void SaveThreatMetrixSessionID(string userId, ThreatMetrix threatMetrix)
        {


            var application = _creditApplicationDynamo.GetCreditApplicationForUser(userId);

            if (application.ThreatMetrix == null)
            {
                application.ThreatMetrix = new ThreatMetrix();
                application.ThreatMetrix.Status = EnumThreatMetrixStatus.none.GetAttributeOfType<DescriptionAttribute>().Description;
            }


            application.ThreatMetrix.SessionId = threatMetrix.SessionId;

            _creditApplicationDynamo.Update(application);

        }

        public void SaveCreditCard(string userId, CreditCardDetail creditCard)
        {


            var application = _creditApplicationDynamo.GetCreditApplicationForUser(userId);

            if (application.CreditCardDetails == null)
            {
                application.CreditCardDetails = new List<CreditCardDetail>();
            }


            var cc = application.CreditCardDetails.FirstOrDefault(c => c.ExternalId == creditCard.ExternalId);

            if (cc != null)
                application.CreditCardDetails.Remove(cc);

            application.CreditCardDetails.Add(creditCard);

            application.RepaymentDone = true;

            _creditApplicationDynamo.Update(application);

        }

        public void SaveDirectDebitAuthority(string userId)
        {


            var application = _creditApplicationDynamo.GetCreditApplicationForUser(userId);

            application.DirectDebitAuthorityDone = true;

            _creditApplicationDynamo.Update(application);

        }

        public void SaveSmallTerms(string userId)
        {


            var application = _creditApplicationDynamo.GetCreditApplicationForUser(userId);

            application.SmallTermsConfirmed = true;


            if (application.FinstroCards.Count == 0 || application.FinstroCards.FirstOrDefault().InccCard == null)
            {
                try
                {
                    InccService inccService = new InccService();

                    var token = inccService.CreateCard(userId);

                    if (token != null)
                    {
                        var token30 = inccService.Get30MinToken(userId, token.CardToken);

                        var cardDetails = inccService.GetCardData(userId, new Finstro.Serverless.Models.Request.Incc.GetCardDataRequest
                        {
                            CallType = EnumInccDeliveryMethod.StandardAustraliaPost.GetAttributeOfType<DescriptionAttribute>().Description,
                            CardToken = token.CardToken,
                            Token30min = token30.Token30min
                        });
                        if (cardDetails != null)
                        {
                            application = _creditApplicationDynamo.GetCreditApplicationForUser(userId);


                            var tCard = application.FinstroCards.FirstOrDefault();

                            tCard.InccCard.CardMaskedNumber = cardDetails.PanNumber;
                            tCard.InccCard.CardEmbossName = $"{cardDetails.FirstName} {cardDetails.LastName}";



                            var userDetail = application.UserDetail;

                            if (userDetail != null)
                            {
                                dynamic data = new
                                {
                                    payloadType = "appRecovery"
                                };

                                _ = PushNotificationHelper.SendSilentNotification(userDetail.FCMTokens.ToArray(), data);
                            }
                        }
                    }
                }
                catch
                {
                    throw FinstroErrorType.INCC.CouldNotCreateCard;
                }

            }


            _creditApplicationDynamo.Update(application);
        }
        public void SavePostalAddress(string userId, string addressType)
        {


            var application = _creditApplicationDynamo.GetCreditApplicationForUser(userId);

            if (application.FinstroCards == null)
                application.FinstroCards = new List<FinstroCardDetail>();

            FinstroCardDetail card = application.FinstroCards.FirstOrDefault();

            bool add = false;

            if (card == null)
            {
                card = new FinstroCardDetail();
                add = true;
            }

            switch (addressType.ToLower())
            {
                case "businesstradingaddress":
                    card.PostalAddress = application.BusinessDetails.BusinessTradingAddress;
                    break;
                default:
                    card.PostalAddress = application.ResidentialAddress;
                    break;
            }

            if (add)
                application.FinstroCards.Add(card);

            _creditApplicationDynamo.Update(application);

        }


        public List<SupportingDocument> AddDocument(string userId, string fullName, string name, string description, string type, Stream stream)
        {
            var application = _creditApplicationDynamo.GetCreditApplicationForUser(userId);


            using (AmazonS3Client client = new AmazonS3Client(_region))
            {
                try
                {
                    string folder = $"{application.UserSubId}/{EnumIdFileType.SupportingDocuments.ToString()}";

                    string ext = Path.GetExtension(fullName).Replace(".", "");

                    string url = AwsHelper.UploadFiles(folder, EnumIdFileType.SupportingDocuments, stream.ReadFullyBytes(), ext);

                    //string url = $"https://{bucketName}.s3.{_region.SystemName}.amazonaws.com/{fileName}";


                    if (application.SupportingDocuments == null)
                        application.SupportingDocuments = new List<SupportingDocument>();

                    application.SupportingDocuments.Add(new SupportingDocument()
                    {
                        Description = description,
                        Name = name,
                        Type = type,
                        Url = url,
                        UploadedDate = DateTime.UtcNow
                    });


                    _creditApplicationDynamo.Update(application);


                    return application.SupportingDocuments;

                }
                catch
                {
                    throw FinstroErrorType.Schema.FileNotFound;
                }
            }
        }


        public string UploadPhoto(string userId, EnumIdFileType fileType, Stream stream)
        {
            var application = _creditApplicationDynamo.GetCreditApplicationForUser(userId);


            using (AmazonS3Client client = new AmazonS3Client(_region))
            {

                string bucketName = AppSettings.AwsSettings.FinstroBucketName;

                string fileName = $"{application.UserSubId}/{Guid.NewGuid().ToString()}_{fileType.ToString()}.jpg";

                try
                {


                    var upload = client.PutObject(new PutObjectRequest
                    {
                        BucketName = bucketName,
                        Key = fileName,
                        ContentType = MimeTypes.ImagePng,
                        InputStream = stream,
                        StorageClass = S3StorageClass.Standard,
                        CannedACL = S3CannedACL.PublicRead

                    });

                    string url = $"https://{bucketName}.s3.{_region.SystemName}.amazonaws.com/{fileName}";


                    if (application.IdImages == null)
                        application.IdImages = new IdImages();


                    switch (fileType)
                    {
                        case EnumIdFileType.IdBack:
                            if (!string.IsNullOrEmpty(application.IdImages.IdBack))
                                _ = AwsHelper.RemoveFile(client, application.IdImages.IdBack);
                            application.IdImages.IdBack = url;

                            break;
                        case EnumIdFileType.IdFront:
                            if (!string.IsNullOrEmpty(application.IdImages.IdFront))
                                _ = AwsHelper.RemoveFile(client, application.IdImages.IdFront);
                            application.IdImages.IdFront = url;
                            break;
                        case EnumIdFileType.Face:
                            if (!string.IsNullOrEmpty(application.IdImages.Face))
                                _ = AwsHelper.RemoveFile(client, application.IdImages.Face);
                            application.IdImages.Face = url;
                            break;
                    }

                    _creditApplicationDynamo.Update(application);


                    return url;

                }
                catch
                {
                    throw FinstroErrorType.Schema.FileNotFound;
                }
            }
        }

        public string UploadXml(CreditApplication application, EnumIdFileType fileType, string xml)
        {
            using (AmazonS3Client client = new AmazonS3Client(_region))
            {

                string bucketName = AppSettings.AwsSettings.FinstroBucketName;

                string fileName = $"{application.UserSubId}/{Guid.NewGuid().ToString()}_{fileType.ToString()}.xml";

                try
                {
                    var upload = client.PutObject(new PutObjectRequest
                    {
                        BucketName = bucketName,
                        Key = fileName,
                        ContentType = MimeTypes.XmlText,
                        ContentBody = xml,
                        StorageClass = S3StorageClass.Standard,
                        CannedACL = S3CannedACL.PublicRead

                    });

                    string url = $"https://{bucketName}.s3.{_region.SystemName}.amazonaws.com/{fileName}";

                    return url;
                }
                catch
                {
                    throw FinstroErrorType.Schema.FileNotFound;
                }
            }
        }


    }

}
