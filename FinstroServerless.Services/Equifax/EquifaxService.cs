using Finstro.Serverless.Helper;
using Finstro.Serverless.Models;
using Finstro.Serverless.Models.Dynamo;
using Finstro.Serverless.Models.Request.CreditApplication;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FinstroServerless.Services.Equifax
{
    public class EquifaxService
    {
        private static RestClient client;
        public EquifaxService()
        {
            client = new RestClient(AppSettings.Equifax.Url);
        }


        #region OrgId

        public string GetOrgIdResult(CreditApplication application)
        {
            try
            {
                string orgIdRequest = ExtractOrgIdRequest(application);

                string destinationUrl = AppSettings.Equifax.Url + "/cta/sys1";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
                byte[] bytes;
                bytes = System.Text.Encoding.ASCII.GetBytes(orgIdRequest);
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

        public static string ExtractOrgIdRequest(CreditApplication application)
        {
            Dictionary<string, string> content = new Dictionary<string, string>();
            content.Add("USER", AppSettings.Equifax.Username);
            content.Add("PWD", AppSettings.Equifax.Password);
            content.Add("MODE", AppSettings.Equifax.FraudCheckMode);
            content.Add("SUBSCRIBERID", AppSettings.Equifax.FraudCheckSubscriberId);
            content.Add("CHECKSECURITY", AppSettings.Equifax.FraudCheckSecurity);


            content.Add("ABN", application.BusinessDetails.AsicBusiness.Abn);


            string requestXml = string.Empty;

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetName().Name + ".Equifax.Template.OrgIdRequest.xml";

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

        #endregion

        #region Individual

        public IndividualCreditCheckResult IndividualCreditCheck(CreditApplication application, BusinessCreditCheck businessCreditCheck)
        {
            string xmlRequest = ExtractIndividualRequest(application);
            IndividualCreditCheckResult result = new IndividualCreditCheckResult();
            var doc = new XmlDocument();

            string destinationUrl = AppSettings.Equifax.Url + "/cta/sys2/soap11/vedascore-apply-v2-0";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
            byte[] bytes;
            bytes = System.Text.Encoding.ASCII.GetBytes(xmlRequest);
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
                doc.LoadXml(responseStr);

                XElement xDoc = XElement.Load(new XmlNodeReader(doc));
                XNamespace vs = "http://vedaxml.com/vxml2/vedascore-apply-v2-0.xsd";

                string file = AwsHelper.UploadFiles(application.UserSubId, EnumIdFileType.IndividualCreditCheck, Encoding.ASCII.GetBytes(responseStr), "xml");
                businessCreditCheck.Files.Add(file);

                int tempInt = 0;


                //<vs:risk-odds>
                result.AdverseScore = float.Parse(doc.GetElementValue("vs:risk-odds"));

                //<vs:datetime-generated> minus <vs:individual-name last-reported-date - first-reported-date>
                DateTime lastReportedDate = Convert.ToDateTime(doc.GetElementValue("vs:individual-name", "last-reported-date"));
                DateTime DateofBirth = Convert.ToDateTime(doc.GetElementValue("vs:date-of-birth"));

                DateTime generatedDate = Convert.ToDateTime(doc.GetElementValue("vs:datetime-generated"));
                DateTime firstReportedDate = Convert.ToDateTime(doc.GetElementValue("vs:individual-name", "first-reported-date"));
                result.AgeOfFile = (generatedDate - firstReportedDate).Days;

                //<vs:datetime-generated> minus <vs:first-reported-date> 
                //<vs:score-masterscale>
                int.TryParse(doc.GetElementValue("vs:score-masterscale"), out tempInt);
                result.CreditScore = tempInt;

                //Number of current directorships
                int.TryParse(xDoc.GetCharacteristicValue(vs, "variable-name", "NP8500_dr_cur"), out tempInt);
                result.Directorships = tempInt;

                //Number of bankrupt only insolvencies in the last 84 months
                int.TryParse(xDoc.GetCharacteristicValue(vs, "variable-name", "NP7502_inslv_bkrpt_84m"), out tempInt);
                result.BankruptOnlyInsolvencies = tempInt;

                //Number of consumer telco or utility defaults/SCI in the last 60/84 months
                //Check with Matt, there is only: Number of commercial telco or utility defaults/SCI in the last 60/84 months
                //Number of consumer telco or utility defaults/SCI in the last 60/84 months
                int.TryParse(xDoc.GetCharacteristicValue(vs, "variable-name", "NX7520_df_tcut_60_84m"), out tempInt);
                result.ConsumerDefaults = tempInt;

                //Number of consumer enquiries in the last 60 months
                int.TryParse(xDoc.GetCharacteristicValue(vs, "variable-name", "NX8006_enq_60m"), out tempInt);
                result.ConsumerEnquiries = tempInt;

                //Number of commercial enquiries in the last 60 months
                int.TryParse(xDoc.GetCharacteristicValue(vs, "variable-name", "NY8006_enq_60m"), out tempInt);
                result.CommercialEnquiries = tempInt;

                //Total dollar amount of outstanding (not settled paid or discontinued) judgements in the last 60 months
                int.TryParse(xDoc.GetCharacteristicValue(vs, "variable-name", "NP7506_dj_out_amt_60m"), out tempInt);
                result.Judgements = tempInt;

                //Number of court writs in the last 60 months
                int.TryParse(xDoc.GetCharacteristicValue(vs, "variable-name", "NP7511_wr_60m"), out tempInt);
                result.WritsSummons = tempInt;

                int.TryParse(xDoc.GetCharacteristicValue(vs, "variable-name", "NA8922_file_notes"), out tempInt);
                result.FileNotes = tempInt;

                //Nummber of business names held
                int.TryParse(xDoc.GetCharacteristicValue(vs, "variable-name", "NP8508_pr_cur"), out tempInt);
                result.Proprietorships = tempInt;

                //Number of external administration companies for director or disqualified directorships in the last 60 months-ever
                int.TryParse(xDoc.GetCharacteristicValue(vs, "variable-name", "NP8506_dr_adv_disq_60m_ever"), out tempInt);
                result.ExternalAdmin = tempInt;

                result.AgeOfSoleTrader = (generatedDate - DateofBirth).Days;

                //Number of consumer telco or utility defaults/SCI in the last 12 months
                int.TryParse(xDoc.GetCharacteristicValue(vs, "variable-name", "NX7518_df_tcut_12m"), out tempInt);
                result.TelcoDefaults12Months = tempInt;

                return result;
            }
            else
            {
                throw FinstroErrorType.INCC.CouldNotGet24HourToken;
            }







        }

        private string ExtractIndividualRequest(CreditApplication application)
        {
            var contact = application.Contacts.FirstOrDefault();
            Dictionary<string, string> content = new Dictionary<string, string>();
            content.Add("USER", AppSettings.Equifax.Username);
            content.Add("PWD", AppSettings.Equifax.Password);
            content.Add("MODE", AppSettings.Equifax.FraudCheckMode);
            content.Add("SUBSCRIBERID", AppSettings.Equifax.FraudCheckSubscriberId);
            content.Add("CHECKSECURITY", AppSettings.Equifax.FraudCheckSecurity);


            content.Add("PERMISSION_TYPE_CODE", "Y");
            content.Add("SCORECARD_ID", "CCAI_2.0_Y_NR");
            content.Add("ACCOUNT_TYPE_CODE", "TF");
            content.Add("FAMILY", contact.FamilyName);
            content.Add("FIRST", contact.FirstGivenName);

            if (application.DrivingLicence != null)
            {
                content.Add("GENDER", application.DrivingLicence.Gender == "M" ? "M" : "F");
                content.Add("LICENCE", application.DrivingLicence.LicenceNumber);
                content.Add("DOB", application.DrivingLicence.DateOfBirth.Value.ToString("yyyy-MM-dd"));
            }
            else
            {
                content.Add("GENDER", application.MedicareCard.Gender == "M" ? "M" : "F");
            }



            content.Add("UNITNO", application.ResidentialAddress.UnitOrLevel);
            content.Add("STREETNO", application.ResidentialAddress.StreetNumber);
            content.Add("STREETNAME", application.ResidentialAddress.StreetName);
            content.Add("STREETTYPE", application.ResidentialAddress.StreetType);
            content.Add("SUBURB", application.ResidentialAddress.Suburb);
            content.Add("STATE", application.ResidentialAddress.State);
            content.Add("POSTCODE", application.ResidentialAddress.PostCode);

            string requestXml = string.Empty;

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetName().Name + ".Equifax.Template.IndividualVedaScoreRequest.xml";

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

        #endregion

        #region Company

        public CompanyCreditCheckResult CompanyCreditCheck(CreditApplication application, BusinessCreditCheck businessCreditCheck)
        {
            string xmlRequest = ExtractCompanyRequest(application);
            CompanyCreditCheckResult result = new CompanyCreditCheckResult();
            var doc = new XmlDocument();

            string destinationUrl = AppSettings.Equifax.Url + "/cta/sys2/company-trading-history-v3-2";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
            byte[] bytes;
            bytes = System.Text.Encoding.ASCII.GetBytes(xmlRequest);
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

                doc.LoadXml(responseStr);

                XElement xDoc = XElement.Load(new XmlNodeReader(doc));
                XNamespace ns = "http://vedaxml.com/vxml2/company-trading-history-v3-2.xsd";

                var node = xDoc.Descendants(ns + "company-response").FirstOrDefault();

                if (node == null)
                {
                    return null;
                }

                int tempInt = 0;

                string file = AwsHelper.UploadFiles(application.UserSubId, EnumIdFileType.BusinessCreditCheck, Encoding.ASCII.GetBytes(responseStr), "xml");
                businessCreditCheck.Files.Add(file);

                // Chance of Failure (Equifax Data Point)
                result.FailureScore = float.Parse(doc.GetElementValue("cth:probability-failure"));

                // Chance of Failure (Equifax Data Point)
                result.AdverseScore = float.Parse(doc.GetElementValue("cth:probability-adverse"));


                int.TryParse(xDoc.Descendants(ns + "company-response").Descendants(ns + "score").Descendants(ns + "bureau-score").FirstOrDefault().Value, out tempInt);
                result.CreditScore = tempInt;


                //<cth:report-create-date> minus <cth:file-creation-date>
                DateTime fileCreationDate = Convert.ToDateTime(doc.GetElementValue("cth:file-creation-date"));
                DateTime createdDate = Convert.ToDateTime(doc.GetElementValue("cth:report-create-date"));
                result.AgeOfFile = ((createdDate - fileCreationDate).Days/365);
                

                //<vs:datetime-generated> minus <vs:individual-name last-reported-date - first-reported-date>
                DateTime gstCreationDate = Convert.ToDateTime(doc.GetElementValue("cth:gst-status-from-date"));
                result.AgeOfABNRegisteredForGST = ((createdDate - gstCreationDate).Days/365);

                try
                {
                    DateTime incorporationDate = Convert.ToDateTime(doc.GetElementValue("cth:incorporation-date"));
                    //result.IncorporationDate = incorporationDate;
                    application.BusinessDetails.IncorporationDate = incorporationDate;

                    application.BusinessDetails.GstDate = gstCreationDate;
                    application.BusinessDetails.TimeTrading = fileCreationDate;


                }
                catch 
                {}


                //<cth:summary-name>Defaults</cth:summary-name>
                int.TryParse(xDoc.GetCharacteristicValue(ns, "summary-name", "Defaults"), out tempInt);
                result.Defaults = tempInt;

                //<cth:summary-name>Defaults-12</cth:summary-name>
                int.TryParse(xDoc.GetCharacteristicValue(ns, "summary-name", "Defaults-12"), out tempInt);
                result.Defaults12Months = tempInt;

                //<cth:summary-name>Defaults-Unpaid</cth:summary-name>
                int.TryParse(xDoc.GetCharacteristicValue(ns, "summary-name", "Defaults-Unpaid"), out tempInt);
                result.DefaultsUnpaid = tempInt;

                //<cth:summary-name>Judgements_Value</cth:summary-name> 
                int.TryParse(xDoc.GetCharacteristicValue(ns, "summary-name", "Judgements"), out tempInt);
                result.Judgements = tempInt;

                //<cth:summary-name>Writs_and_Summons</cth:summary-name>
                int.TryParse(xDoc.GetCharacteristicValue(ns, "summary-name", "Writs_and_Summons"), out tempInt);
                result.WritsSummons = tempInt;

                //<cth:summary-name>File_Notes</cth:summary-name>
                int.TryParse(xDoc.GetCharacteristicValue(ns, "summary-name", "File_Notes"), out tempInt);
                result.FileNotes = tempInt;

                //<cth:summary-name>Petitions</cth:summary-name>
                int.TryParse(xDoc.GetCharacteristicValue(ns, "summary-name", "Petitions"), out tempInt);
                result.Petitions = tempInt;

                //<cth:summary-name>External_Admin</cth:summary-name>
                int.TryParse(xDoc.GetCharacteristicValue(ns, "summary-name", "External_Admin"), out tempInt);
                result.ExternalAdmin = tempInt;

                //<cth:summary-name>Mercantile_Agent</cth:summary-name>
                int.TryParse(xDoc.GetCharacteristicValue(ns, "summary-name", "Mercantile_Agent"), out tempInt);
                result.MercantileAgent = tempInt;

                //<cth:summary-name>Mercantile_Enquiries_Value</cth:summary-name>
                int.TryParse(xDoc.GetCharacteristicValue(ns, "summary-name", "Mercantile_Enquiries_Value"), out tempInt);
                result.MercantileEnquiries = tempInt;

                //<cth:summary-name>Credit_Enquiries</cth:summary-name>
                int.TryParse(xDoc.GetCharacteristicValue(ns, "summary-name", "Credit_Enquiries"), out tempInt);
                result.CreditEnquiries = tempInt;

                //<cth:summary-name>Credit_Enquiries_Less_Than_12_Months_Value</cth:summary-name>
                int.TryParse(xDoc.GetCharacteristicValue(ns, "summary-name", "Credit_Enquiries_Less_Than_12_Months_Value"), out tempInt);
                result.CreditEnquiries12Months = tempInt;

                //<cth:summary-name>Telco_Defaults_Less_Than_12_Months</cth:summary-name>
                int.TryParse(xDoc.GetCharacteristicValue(ns, "summary-name", "Telco_Defaults_Less_Than_12_Months"), out tempInt);
                result.TelcoDefaults12MonthsCount = tempInt;

                //<cth:summary-name>Telco_Defaults_Less_Than_12_Months_Value</cth:summary-name>
                int.TryParse(xDoc.GetCharacteristicValue(ns, "summary-name", "Telco_Defaults_Less_Than_12_Months_Value"), out tempInt);
                result.TelcoDefaults12MonthsAmount = tempInt;

                //<cth:summary-name>Utility_Defaults_Less_Than_12_Months</cth:summary-name>
                int.TryParse(xDoc.GetCharacteristicValue(ns, "summary-name", "Utility_Defaults_Less_Than_12_Months"), out tempInt);
                result.UtilityDefaults12MonthsCount = tempInt;

                //<cth:summary-name>Utility_Defaults_Less_Than_12_Months_Value</cth:summary-name>
                int.TryParse(xDoc.GetCharacteristicValue(ns, "summary-name", "Utility_Defaults_Less_Than_12_Months_Value"), out tempInt);
                result.UtilityDefaults12MonthsAmount = tempInt;

                //<<cth:registration-type>ALLPAPNOEXCEPT</cth:registration-type> plus <cth:registration-type>ALLPAPWITHEXCEPT</cth:registration-type>
                int.TryParse(xDoc.GetCharacteristicValue(ns, "registration-type", "ALLPAPNOEXCEPT"), out tempInt);
                int allNoExcept = tempInt;
                int.TryParse(xDoc.GetCharacteristicValue(ns, "registration-type", "ALLPAPWITHEXCEPT"), out tempInt);
                int allWithExcept = tempInt;

                result.PPSR = allNoExcept + allWithExcept;

                // TODO - Create Rule for directors
                result.OnboardingDirectorVerified = false;


                var directors = xDoc.Descendants(ns + "company-director-list");

                foreach (var item in directors)
                {
                    Director director = new Director();

                    director.FirstName = item.Descendants(ns + "first-given-name").FirstOrDefault().Value;
                    director.LastName = item.Descendants(ns + "family-name").FirstOrDefault().Value;

                    fileCreationDate = Convert.ToDateTime(item.Descendants(ns + "file-creation-date").FirstOrDefault().Value);
                    director.AgeOfFile = ((createdDate - fileCreationDate).Days / 365);

                    //<cth:company-director-list><cth:commercial-individual><cth:score><cth:probability-adverse>
                    director.AdverseScore = float.Parse(item.Descendants(ns + "probability-adverse").FirstOrDefault().Value);

                    //<cth:company-director-list>…<cth:summary-name>Individual_Defaults</cth:summary-name>
                    int.TryParse(item.GetCharacteristicValue(ns, "summary-name", "Individual_Defaults", "summary-value"), out tempInt);
                    director.Defaults = tempInt;

                    //<cth:company-director-list>…<cth:summary-name>Individual_Judgements_Value</cth:summary-name>
                    int.TryParse(item.GetCharacteristicValue(ns, "summary-name", "Individual_Judgements_Value", "summary-value"), out tempInt);
                    director.Judgements = tempInt;

                    //<cth:company-director-list>…<cth:summary-name>Individual_Writs_and_Summons</cth:summary-name>
                    int.TryParse(item.GetCharacteristicValue(ns, "summary-name", "Individual_Writs_and_Summons", "summary-value"), out tempInt);
                    director.WritsSummons = tempInt;

                    //<cth:company-director-list>…<cth:summary-name>Individual_File_Notes</cth:summary-name>
                    int.TryParse(item.GetCharacteristicValue(ns, "summary-name", "Individual_File_Notes", "summary-value"), out tempInt);
                    director.FileNotes = tempInt;

                    //<cth:company-director-list>…<cth:summary-name>Individual_Previous_Directorships</cth:summary-name>
                    int.TryParse(item.GetCharacteristicValue(ns, "summary-name", "Individual_Previous_Directorships", "summary-value"), out tempInt);
                    director.Proprietorships = tempInt;


                    fileCreationDate = Convert.ToDateTime(item.Descendants(ns + "appointment-date").FirstOrDefault().Value);
                    director.DirectorAppointmentDate = (((createdDate - fileCreationDate).Days / 365) * 12);

                    fileCreationDate = Convert.ToDateTime(item.Descendants(ns + "date-of-birth").FirstOrDefault().Value);
                    director.AgeOfDirector = ((createdDate - fileCreationDate).Days / 365);

                    //<cth:company-director-list>…<cth:summary-name>Individual_Credit_Enquiries</cth:summary-name>
                    int.TryParse(item.GetCharacteristicValue(ns, "summary-name", "Individual_Credit_Enquiries", "summary-value"), out tempInt);
                    director.CreditEnquiries = tempInt;

                    //<cth:summary-name>Disqualified_Directors</cth:summary-name>
                    int.TryParse(item.GetCharacteristicValue(ns, "summary-name", "Disqualified_Directors", "summary-value"), out tempInt);
                    director.DisqualifiedDirectors = tempInt;

                    result.Directors.Add(director);

                }


                return result;
            }
            else
            {
                throw FinstroErrorType.INCC.CouldNotGet24HourToken;
            }







        }

        private string ExtractCompanyRequest(CreditApplication application)
        {
            var contact = application.Contacts.FirstOrDefault();

            Dictionary<string, string> content = new Dictionary<string, string>();
            content.Add("USER", AppSettings.Equifax.Username);
            content.Add("PWD", AppSettings.Equifax.Password);

            content.Add("ACCOUNT_TYPE_CODE", "TF");
            content.Add("ACCOUNT_TYPE_NAME", "TRADEFINANCE");
            content.Add("ORGNO", application.BusinessDetails.AsicBusiness.Acn);
            content.Add("AMOUNT", Convert.ToInt32(application.BusinessDetails.SelectedCreditAmount).ToString());

            content.Add("FAMILY", contact.FamilyName);
            content.Add("FIRST", contact.FirstGivenName);

            if (application.DrivingLicence != null)
            {
                content.Add("GENDER", application.DrivingLicence.Gender == "M" ? "male" : "female");
                content.Add("DOB", application.DrivingLicence.DateOfBirth.Value.ToString("yyyy-MM-dd"));
            }
            else
            {
                content.Add("GENDER", application.MedicareCard.Gender == "M" ? "male" : "female");
                content.Add("DOB", application.MedicareCard.DateOfBirth.Value.ToString("yyyy-MM-dd"));
            }


            content.Add("UNITNO", application.ResidentialAddress.UnitOrLevel);
            content.Add("STREETNO", application.ResidentialAddress.StreetNumber);
            content.Add("STREETNAME", application.ResidentialAddress.StreetName);
            content.Add("STREETTYPE", application.ResidentialAddress.StreetType);
            content.Add("SUBURB", application.ResidentialAddress.Suburb);
            content.Add("STATE", application.ResidentialAddress.State);
            content.Add("POSTCODE", application.ResidentialAddress.PostCode);

            string requestXml = string.Empty;

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetName().Name + ".Equifax.Template.CompanyTradingHistoryRequest.xml";

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
        #endregion
    }


}
