using System;
using System.Xml.Serialization;

namespace Finstro.Serverless.Models.Request.Equifax
{

    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [XmlRoot(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public class IndividualVedaScoreRequest
    {

        private EnvelopeHeader headerField;

        private EnvelopeBody bodyField;

        /// <remarks/>    public EnvelopeHeader Header
        {
            get
            {
                return this.headerField;
            }
            set
            {
                this.headerField = value;
            }
        }

        /// <remarks/>    public EnvelopeBody Body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }
    }
    /// <remarks/>    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class EnvelopeHeader
    {

        private Security securityField;

        private string toField;

        private string actionField;
        /// <remarks/>        [XmlElement(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
        public Security Security
        {
            get
            {
                return this.securityField;
            }
            set
            {
                this.securityField = value;
            }
        }

        /// <remarks/>    [XmlElement(Namespace = "http://www.w3.org/2005/08/addressing")]
        public string To
        {
            get
            {
                return this.toField;
            }
            set
            {
                this.toField = value;
            }
        }

        /// <remarks/>    [XmlElement(Namespace = "http://www.w3.org/2005/08/addressing")]
        public string Action
        {
            get
            {
                return this.actionField;
            }
            set
            {
                this.actionField = value;
            }
        }
    }

    /// <remarks/>    [XmlType(AnonymousType = true, Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
    [XmlRoot(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", IsNullable = false)]
    public class Security
    {

        private SecurityUsernameToken usernameTokenField;

        /// <remarks/>    public SecurityUsernameToken UsernameToken
        {
            get
            {
                return this.usernameTokenField;
            }
            set
            {
                this.usernameTokenField = value;
            }
        }
    }

    /// <remarks/>[XmlType(AnonymousType = true, Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd" +
      "")]
    public class SecurityUsernameToken
    {

        private string usernameField;

        private string passwordField;













        /// <remarks/>    public string Username
        {
            get
            {
                return this.usernameField;
            }
            set
            {
                this.usernameField = value;
            }
        }













        /// <remarks/>    public string Password
        {
            get
            {
                return this.passwordField;
            }
            set
            {
                this.passwordField = value;
            }
        }
    }













    /// <remarks/>[XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class EnvelopeBody
    {

        private request requestField;













        /// <remarks/>    [XmlElement(Namespace = "http://vedaxml.com/vxml2/vedascore-apply-v2-0.xsd")]
        public request request
        {
            get
            {
                return this.requestField;
            }
            set
            {
                this.requestField = value;
            }
        }
    }













    /// <remarks/>[XmlType(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/vedascore-apply-v2-0.xsd")]
    [XmlRoot(Namespace = "http://vedaxml.com/vxml2/vedascore-apply-v2-0.xsd", IsNullable = false)]
    public class request
    {

        private requestEnquiryheader enquiryheaderField;

        private requestEnquirydata enquirydataField;













        /// <remarks/>    [XmlElement("enquiry-header")]
        public requestEnquiryheader enquiryheader
        {
            get
            {
                return this.enquiryheaderField;
            }
            set
            {
                this.enquiryheaderField = value;
            }
        }













        /// <remarks/>    [XmlElement("enquiry-data")]
        public requestEnquirydata enquirydata
        {
            get
            {
                return this.enquirydataField;
            }
            set
            {
                this.enquirydataField = value;
            }
        }
    }













    /// <remarks/>[XmlType(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/vedascore-apply-v2-0.xsd")]
    public class requestEnquiryheader
    {

        private string clientreferenceField;

        private string permissiontypecodeField;

        private string productdatalevelcodeField;

        private requestEnquiryheaderRequestedscores requestedscoresField;













        /// <remarks/>    [XmlElement("client-reference")]
        public string clientreference
        {
            get
            {
                return this.clientreferenceField;
            }
            set
            {
                this.clientreferenceField = value;
            }
        }













        /// <remarks/>    [XmlElement("permission-type-code")]
        public string permissiontypecode
        {
            get
            {
                return this.permissiontypecodeField;
            }
            set
            {
                this.permissiontypecodeField = value;
            }
        }













        /// <remarks/>    [XmlElement("product-data-level-code")]
        public string productdatalevelcode
        {
            get
            {
                return this.productdatalevelcodeField;
            }
            set
            {
                this.productdatalevelcodeField = value;
            }
        }













        /// <remarks/>    [XmlElement("requested-scores")]
        public requestEnquiryheaderRequestedscores requestedscores
        {
            get
            {
                return this.requestedscoresField;
            }
            set
            {
                this.requestedscoresField = value;
            }
        }
    }













    /// <remarks/>[XmlType(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/vedascore-apply-v2-0.xsd")]
    public class requestEnquiryheaderRequestedscores
    {

        private string scorecardidField;













        /// <remarks/>    [XmlElement("scorecard-id")]
        public string scorecardid
        {
            get
            {
                return this.scorecardidField;
            }
            set
            {
                this.scorecardidField = value;
            }
        }
    }













    /// <remarks/>[XmlType(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/vedascore-apply-v2-0.xsd")]
    public class requestEnquirydata
    {

        private requestEnquirydataIndividual individualField;

        private requestEnquirydataEnquiry enquiryField;













        /// <remarks/>    public requestEnquirydataIndividual individual
        {
            get
            {
                return this.individualField;
            }
            set
            {
                this.individualField = value;
            }
        }













        /// <remarks/>    public requestEnquirydataEnquiry enquiry
        {
            get
            {
                return this.enquiryField;
            }
            set
            {
                this.enquiryField = value;
            }
        }
    }













    /// <remarks/>[XmlType(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/vedascore-apply-v2-0.xsd")]
    public class requestEnquirydataIndividual
    {

        private requestEnquirydataIndividualCurrentname currentnameField;

        private requestEnquirydataIndividualAddresses addressesField;

        private requestEnquirydataIndividualDriverslicence driverslicenceField;

        private string gendercodeField;

        private System.DateTime dateofbirthField;













        /// <remarks/>    [XmlElement("current-name")]
        public requestEnquirydataIndividualCurrentname currentname
        {
            get
            {
                return this.currentnameField;
            }
            set
            {
                this.currentnameField = value;
            }
        }













        /// <remarks/>    public requestEnquirydataIndividualAddresses addresses
        {
            get
            {
                return this.addressesField;
            }
            set
            {
                this.addressesField = value;
            }
        }













        /// <remarks/>    [XmlElement("drivers-licence")]
        public requestEnquirydataIndividualDriverslicence driverslicence
        {
            get
            {
                return this.driverslicenceField;
            }
            set
            {
                this.driverslicenceField = value;
            }
        }













        /// <remarks/>    [XmlElement("gender-code")]
        public string gendercode
        {
            get
            {
                return this.gendercodeField;
            }
            set
            {
                this.gendercodeField = value;
            }
        }













        /// <remarks/>    [XmlElement("date-of-birth", DataType = "date")]
        public System.DateTime dateofbirth
        {
            get
            {
                return this.dateofbirthField;
            }
            set
            {
                this.dateofbirthField = value;
            }
        }
    }













    /// <remarks/>[XmlType(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/vedascore-apply-v2-0.xsd")]
    public class requestEnquirydataIndividualCurrentname
    {

        private string familynameField;

        private string firstgivennameField;













        /// <remarks/>    [XmlElement("family-name")]
        public string familyname
        {
            get
            {
                return this.familynameField;
            }
            set
            {
                this.familynameField = value;
            }
        }













        /// <remarks/>    [XmlElement("first-given-name")]
        public string firstgivenname
        {
            get
            {
                return this.firstgivennameField;
            }
            set
            {
                this.firstgivennameField = value;
            }
        }
    }













    /// <remarks/>[XmlType(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/vedascore-apply-v2-0.xsd")]
    public class requestEnquirydataIndividualAddresses
    {

        private requestEnquirydataIndividualAddressesAddress addressField;













        /// <remarks/>    public requestEnquirydataIndividualAddressesAddress address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }
    }













    /// <remarks/>[XmlType(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/vedascore-apply-v2-0.xsd")]
    public class requestEnquirydataIndividualAddressesAddress
    {

        private object streetnumberField;

        private string streetnameField;

        private string streettypeField;

        private string suburbField;

        private string stateField;

        private ushort postcodeField;

        private string typeField;













        /// <remarks/>    [XmlElement("street-number")]
        public object streetnumber
        {
            get
            {
                return this.streetnumberField;
            }
            set
            {
                this.streetnumberField = value;
            }
        }













        /// <remarks/>    [XmlElement("street-name")]
        public string streetname
        {
            get
            {
                return this.streetnameField;
            }
            set
            {
                this.streetnameField = value;
            }
        }













        /// <remarks/>    [XmlElement("street-type")]
        public string streettype
        {
            get
            {
                return this.streettypeField;
            }
            set
            {
                this.streettypeField = value;
            }
        }













        /// <remarks/>    public string suburb
        {
            get
            {
                return this.suburbField;
            }
            set
            {
                this.suburbField = value;
            }
        }













        /// <remarks/>    public string state
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }













        /// <remarks/>    public ushort postcode
        {
            get
            {
                return this.postcodeField;
            }
            set
            {
                this.postcodeField = value;
            }
        }













        /// <remarks/>    [XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }













    /// <remarks/>[XmlType(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/vedascore-apply-v2-0.xsd")]
    public class requestEnquirydataIndividualDriverslicence
    {

        private uint numberField;













        /// <remarks/>    public uint number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }
    }













    /// <remarks/>[XmlType(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/vedascore-apply-v2-0.xsd")]
    public class requestEnquirydataEnquiry
    {

        private string accounttypecodeField;

        private requestEnquirydataEnquiryEnquiryamount enquiryamountField;

        private bool iscreditreviewField;

        private byte relationshipcodeField;













        /// <remarks/>    [XmlElement("account-type-code")]
        public string accounttypecode
        {
            get
            {
                return this.accounttypecodeField;
            }
            set
            {
                this.accounttypecodeField = value;
            }
        }













        /// <remarks/>    [XmlElement("enquiry-amount")]
        public requestEnquirydataEnquiryEnquiryamount enquiryamount
        {
            get
            {
                return this.enquiryamountField;
            }
            set
            {
                this.enquiryamountField = value;
            }
        }













        /// <remarks/>    [XmlElement("is-credit-review")]
        public bool iscreditreview
        {
            get
            {
                return this.iscreditreviewField;
            }
            set
            {
                this.iscreditreviewField = value;
            }
        }













        /// <remarks/>    [XmlElement("relationship-code")]
        public byte relationshipcode
        {
            get
            {
                return this.relationshipcodeField;
            }
            set
            {
                this.relationshipcodeField = value;
            }
        }
    }













    /// <remarks/>[XmlType(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/vedascore-apply-v2-0.xsd")]
    public class requestEnquirydataEnquiryEnquiryamount
    {

        private string currencycodeField;

        private byte valueField;













        /// <remarks/>    [XmlAttributeAttribute("currency-code")]
        public string currencycode
        {
            get
            {
                return this.currencycodeField;
            }
            set
            {
                this.currencycodeField = value;
            }
        }













        /// <remarks/>    [XmlTextAttribute()]
        public byte Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }
}
