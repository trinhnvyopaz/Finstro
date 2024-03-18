﻿using System;
using System.Xml.Serialization;

namespace Finstro.Serverless.Models.Request.Equifax
{

    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [XmlRoot(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public class IndividualVedaScoreRequest
    {

        private EnvelopeHeader headerField;

        private EnvelopeBody bodyField;

        /// <remarks/>
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

        /// <remarks/>
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

    public class EnvelopeHeader
    {

        private Security securityField;

        private string toField;

        private string actionField;

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

        /// <remarks/>
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

        /// <remarks/>
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

    /// <remarks/>
    [XmlRoot(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", IsNullable = false)]
    public class Security
    {

        private SecurityUsernameToken usernameTokenField;

        /// <remarks/>
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

    /// <remarks/>
      "")]
    public class SecurityUsernameToken
    {

        private string usernameField;

        private string passwordField;













        /// <remarks/>
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













        /// <remarks/>
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













    /// <remarks/>
    public class EnvelopeBody
    {

        private request requestField;













        /// <remarks/>
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













    /// <remarks/>
    [XmlRoot(Namespace = "http://vedaxml.com/vxml2/vedascore-apply-v2-0.xsd", IsNullable = false)]
    public class request
    {

        private requestEnquiryheader enquiryheaderField;

        private requestEnquirydata enquirydataField;













        /// <remarks/>
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













        /// <remarks/>
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













    /// <remarks/>
    public class requestEnquiryheader
    {

        private string clientreferenceField;

        private string permissiontypecodeField;

        private string productdatalevelcodeField;

        private requestEnquiryheaderRequestedscores requestedscoresField;













        /// <remarks/>
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













        /// <remarks/>
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













        /// <remarks/>
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













        /// <remarks/>
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













    /// <remarks/>
    public class requestEnquiryheaderRequestedscores
    {

        private string scorecardidField;













        /// <remarks/>
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













    /// <remarks/>
    public class requestEnquirydata
    {

        private requestEnquirydataIndividual individualField;

        private requestEnquirydataEnquiry enquiryField;













        /// <remarks/>
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













        /// <remarks/>
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













    /// <remarks/>
    public class requestEnquirydataIndividual
    {

        private requestEnquirydataIndividualCurrentname currentnameField;

        private requestEnquirydataIndividualAddresses addressesField;

        private requestEnquirydataIndividualDriverslicence driverslicenceField;

        private string gendercodeField;

        private System.DateTime dateofbirthField;













        /// <remarks/>
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













        /// <remarks/>
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













        /// <remarks/>
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













        /// <remarks/>
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













        /// <remarks/>
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













    /// <remarks/>
    public class requestEnquirydataIndividualCurrentname
    {

        private string familynameField;

        private string firstgivennameField;













        /// <remarks/>
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













        /// <remarks/>
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













    /// <remarks/>
    public class requestEnquirydataIndividualAddresses
    {

        private requestEnquirydataIndividualAddressesAddress addressField;













        /// <remarks/>
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













    /// <remarks/>
    public class requestEnquirydataIndividualAddressesAddress
    {

        private object streetnumberField;

        private string streetnameField;

        private string streettypeField;

        private string suburbField;

        private string stateField;

        private ushort postcodeField;

        private string typeField;













        /// <remarks/>
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













        /// <remarks/>
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













        /// <remarks/>
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













        /// <remarks/>
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













        /// <remarks/>
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













        /// <remarks/>
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













        /// <remarks/>
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













    /// <remarks/>
    public class requestEnquirydataIndividualDriverslicence
    {

        private uint numberField;













        /// <remarks/>
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













    /// <remarks/>
    public class requestEnquirydataEnquiry
    {

        private string accounttypecodeField;

        private requestEnquirydataEnquiryEnquiryamount enquiryamountField;

        private bool iscreditreviewField;

        private byte relationshipcodeField;













        /// <remarks/>
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













        /// <remarks/>
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













        /// <remarks/>
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













        /// <remarks/>
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













    /// <remarks/>
    public class requestEnquirydataEnquiryEnquiryamount
    {

        private string currencycodeField;

        private byte valueField;













        /// <remarks/>
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













        /// <remarks/>
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