using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Response.Equifax
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public partial class Envelope
    {

        private EnvelopeHeader headerField;

        private EnvelopeBody bodyField;

        /// <remarks/>
        public EnvelopeHeader Header
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
        public EnvelopeBody Body
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

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public partial class EnvelopeHeader
    {

        private string messageIDField;

        private string relatesToField;

        private string toField;

        private From fromField;

        private string actionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2005/08/addressing")]
        public string MessageID
        {
            get
            {
                return this.messageIDField;
            }
            set
            {
                this.messageIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2005/08/addressing")]
        public string RelatesTo
        {
            get
            {
                return this.relatesToField;
            }
            set
            {
                this.relatesToField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2005/08/addressing")]
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
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2005/08/addressing")]
        public From From
        {
            get
            {
                return this.fromField;
            }
            set
            {
                this.fromField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2005/08/addressing")]
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
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2005/08/addressing")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2005/08/addressing", IsNullable = false)]
    public partial class From
    {

        private string addressField;

        /// <remarks/>
        public string Address
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
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public partial class EnvelopeBody
    {

        private response responseField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
        public response response
        {
            get
            {
                return this.responseField;
            }
            set
            {
                this.responseField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd", IsNullable = false)]
    public partial class response
    {

        private responseResponseoutcome responseoutcomeField;

        private responseComponentresponses componentresponsesField;

        private string clientreferenceField;

        private string enquiryidField;

        private string profilenameField;

        private byte profileversionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("response-outcome")]
        public responseResponseoutcome responseoutcome
        {
            get
            {
                return this.responseoutcomeField;
            }
            set
            {
                this.responseoutcomeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("component-responses")]
        public responseComponentresponses componentresponses
        {
            get
            {
                return this.componentresponsesField;
            }
            set
            {
                this.componentresponsesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("client-reference")]
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
        [System.Xml.Serialization.XmlAttributeAttribute("enquiry-id")]
        public string enquiryid
        {
            get
            {
                return this.enquiryidField;
            }
            set
            {
                this.enquiryidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("profile-name")]
        public string profilename
        {
            get
            {
                return this.profilenameField;
            }
            set
            {
                this.profilenameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("profile-version")]
        public byte profileversion
        {
            get
            {
                return this.profileversionField;
            }
            set
            {
                this.profileversionField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseResponseoutcome
    {

        private string overalloutcomeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("overall-outcome")]
        public string overalloutcome
        {
            get
            {
                return this.overalloutcomeField;
            }
            set
            {
                this.overalloutcomeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponses
    {

        private responseComponentresponsesVerificationresponse verificationresponseField;

        private responseComponentresponsesFraudassessmentresponse fraudassessmentresponseField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("verification-response")]
        public responseComponentresponsesVerificationresponse verificationresponse
        {
            get
            {
                return this.verificationresponseField;
            }
            set
            {
                this.verificationresponseField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("fraud-assessment-response")]
        public responseComponentresponsesFraudassessmentresponse fraudassessmentresponse
        {
            get
            {
                return this.fraudassessmentresponseField;
            }
            set
            {
                this.fraudassessmentresponseField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponse
    {

        private responseComponentresponsesVerificationresponseVerificationoutcome verificationoutcomeField;

        private responseComponentresponsesVerificationresponseRuleresult[] rulesresultsField;

        private responseComponentresponsesVerificationresponseAnalysisresult[] analysisresultsField;

        private responseComponentresponsesVerificationresponseSearchresult[] searchresultsField;

        private object recordsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("verification-outcome")]
        public responseComponentresponsesVerificationresponseVerificationoutcome verificationoutcome
        {
            get
            {
                return this.verificationoutcomeField;
            }
            set
            {
                this.verificationoutcomeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute("rules-results")]
        [System.Xml.Serialization.XmlArrayItemAttribute("rule-result", IsNullable = false)]
        public responseComponentresponsesVerificationresponseRuleresult[] rulesresults
        {
            get
            {
                return this.rulesresultsField;
            }
            set
            {
                this.rulesresultsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute("analysis-results")]
        [System.Xml.Serialization.XmlArrayItemAttribute("analysis-result", IsNullable = false)]
        public responseComponentresponsesVerificationresponseAnalysisresult[] analysisresults
        {
            get
            {
                return this.analysisresultsField;
            }
            set
            {
                this.analysisresultsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute("search-results")]
        [System.Xml.Serialization.XmlArrayItemAttribute("search-result", IsNullable = false)]
        public responseComponentresponsesVerificationresponseSearchresult[] searchresults
        {
            get
            {
                return this.searchresultsField;
            }
            set
            {
                this.searchresultsField = value;
            }
        }

        /// <remarks/>
        public object records
        {
            get
            {
                return this.recordsField;
            }
            set
            {
                this.recordsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseVerificationoutcome
    {

        private string indicatorField;

        private byte totalpointsField;

        private string selfverificationurlField;

        /// <remarks/>
        public string indicator
        {
            get
            {
                return this.indicatorField;
            }
            set
            {
                this.indicatorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("total-points")]
        public byte totalpoints
        {
            get
            {
                return this.totalpointsField;
            }
            set
            {
                this.totalpointsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("self-verification-url")]
        public string selfverificationurl
        {
            get
            {
                return this.selfverificationurlField;
            }
            set
            {
                this.selfverificationurlField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseRuleresult
    {

        private string indicatorField;

        private string reasonField;

        private string nameField;

        /// <remarks/>
        public string indicator
        {
            get
            {
                return this.indicatorField;
            }
            set
            {
                this.indicatorField = value;
            }
        }

        /// <remarks/>
        public string reason
        {
            get
            {
                return this.reasonField;
            }
            set
            {
                this.reasonField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseAnalysisresult
    {

        private object categoryField;

        private string searchnameField;

        private byte rawscoreField;

        private byte minimumvalueField;

        private byte filteredscoreField;

        private decimal weightField;

        private byte pointsField;

        private object contributingfactorsField;

        /// <remarks/>
        public object category
        {
            get
            {
                return this.categoryField;
            }
            set
            {
                this.categoryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("search-name")]
        public string searchname
        {
            get
            {
                return this.searchnameField;
            }
            set
            {
                this.searchnameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("raw-score")]
        public byte rawscore
        {
            get
            {
                return this.rawscoreField;
            }
            set
            {
                this.rawscoreField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("minimum-value")]
        public byte minimumvalue
        {
            get
            {
                return this.minimumvalueField;
            }
            set
            {
                this.minimumvalueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filtered-score")]
        public byte filteredscore
        {
            get
            {
                return this.filteredscoreField;
            }
            set
            {
                this.filteredscoreField = value;
            }
        }

        /// <remarks/>
        public decimal weight
        {
            get
            {
                return this.weightField;
            }
            set
            {
                this.weightField = value;
            }
        }

        /// <remarks/>
        public byte points
        {
            get
            {
                return this.pointsField;
            }
            set
            {
                this.pointsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("contributing-factors")]
        public object contributingfactors
        {
            get
            {
                return this.contributingfactorsField;
            }
            set
            {
                this.contributingfactorsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseSearchresult
    {

        private responseComponentresponsesVerificationresponseSearchresultIndividualname individualnameField;

        private responseComponentresponsesVerificationresponseSearchresultMedicare medicareField;

        private responseComponentresponsesVerificationresponseSearchresultDateofbirth dateofbirthField;

        private responseComponentresponsesVerificationresponseSearchresultGender genderField;

        private responseComponentresponsesVerificationresponseSearchresultCurrentaddress currentaddressField;

        private responseComponentresponsesVerificationresponseSearchresultPhonenumbers phonenumbersField;

        private string matchindicatorField;

        private byte matchscoreField;

        private bool matchscoreFieldSpecified;

        private string searchnameField;

        private string searchtypeField;

        private byte serviceresultcodeField;

        private string serviceresultdetailField;

        private string serviceresultstringField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("individual-name")]
        public responseComponentresponsesVerificationresponseSearchresultIndividualname individualname
        {
            get
            {
                return this.individualnameField;
            }
            set
            {
                this.individualnameField = value;
            }
        }

        /// <remarks/>
        public responseComponentresponsesVerificationresponseSearchresultMedicare medicare
        {
            get
            {
                return this.medicareField;
            }
            set
            {
                this.medicareField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("date-of-birth")]
        public responseComponentresponsesVerificationresponseSearchresultDateofbirth dateofbirth
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

        /// <remarks/>
        public responseComponentresponsesVerificationresponseSearchresultGender gender
        {
            get
            {
                return this.genderField;
            }
            set
            {
                this.genderField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("current-address")]
        public responseComponentresponsesVerificationresponseSearchresultCurrentaddress currentaddress
        {
            get
            {
                return this.currentaddressField;
            }
            set
            {
                this.currentaddressField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("phone-numbers")]
        public responseComponentresponsesVerificationresponseSearchresultPhonenumbers phonenumbers
        {
            get
            {
                return this.phonenumbersField;
            }
            set
            {
                this.phonenumbersField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("match-indicator")]
        public string matchindicator
        {
            get
            {
                return this.matchindicatorField;
            }
            set
            {
                this.matchindicatorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("match-score")]
        public byte matchscore
        {
            get
            {
                return this.matchscoreField;
            }
            set
            {
                this.matchscoreField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool matchscoreSpecified
        {
            get
            {
                return this.matchscoreFieldSpecified;
            }
            set
            {
                this.matchscoreFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("search-name")]
        public string searchname
        {
            get
            {
                return this.searchnameField;
            }
            set
            {
                this.searchnameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("search-type")]
        public string searchtype
        {
            get
            {
                return this.searchtypeField;
            }
            set
            {
                this.searchtypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("service-result-code")]
        public byte serviceresultcode
        {
            get
            {
                return this.serviceresultcodeField;
            }
            set
            {
                this.serviceresultcodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("service-result-detail")]
        public string serviceresultdetail
        {
            get
            {
                return this.serviceresultdetailField;
            }
            set
            {
                this.serviceresultdetailField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("service-result-string")]
        public string serviceresultstring
        {
            get
            {
                return this.serviceresultstringField;
            }
            set
            {
                this.serviceresultstringField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseSearchresultIndividualname
    {

        private responseComponentresponsesVerificationresponseSearchresultIndividualnameFamilyname familynameField;

        private responseComponentresponsesVerificationresponseSearchresultIndividualnameFirstgivenname firstgivennameField;

        private responseComponentresponsesVerificationresponseSearchresultIndividualnameOthergivenname othergivennameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("family-name")]
        public responseComponentresponsesVerificationresponseSearchresultIndividualnameFamilyname familyname
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
        [System.Xml.Serialization.XmlElementAttribute("first-given-name")]
        public responseComponentresponsesVerificationresponseSearchresultIndividualnameFirstgivenname firstgivenname
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("other-given-name")]
        public responseComponentresponsesVerificationresponseSearchresultIndividualnameOthergivenname othergivenname
        {
            get
            {
                return this.othergivennameField;
            }
            set
            {
                this.othergivennameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseSearchresultIndividualnameFamilyname
    {

        private string searchvalueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("search-value")]
        public string searchvalue
        {
            get
            {
                return this.searchvalueField;
            }
            set
            {
                this.searchvalueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseSearchresultIndividualnameFirstgivenname
    {

        private string searchvalueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("search-value")]
        public string searchvalue
        {
            get
            {
                return this.searchvalueField;
            }
            set
            {
                this.searchvalueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseSearchresultIndividualnameOthergivenname
    {

        private string searchvalueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("search-value")]
        public string searchvalue
        {
            get
            {
                return this.searchvalueField;
            }
            set
            {
                this.searchvalueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseSearchresultMedicare
    {

        private responseComponentresponsesVerificationresponseSearchresultMedicareCardnumber cardnumberField;

        private responseComponentresponsesVerificationresponseSearchresultMedicareReferencenumber referencenumberField;

        private responseComponentresponsesVerificationresponseSearchresultMedicareExpirydate expirydateField;

        private responseComponentresponsesVerificationresponseSearchresultMedicareCardcolour cardcolourField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("card-number")]
        public responseComponentresponsesVerificationresponseSearchresultMedicareCardnumber cardnumber
        {
            get
            {
                return this.cardnumberField;
            }
            set
            {
                this.cardnumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("reference-number")]
        public responseComponentresponsesVerificationresponseSearchresultMedicareReferencenumber referencenumber
        {
            get
            {
                return this.referencenumberField;
            }
            set
            {
                this.referencenumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("expiry-date")]
        public responseComponentresponsesVerificationresponseSearchresultMedicareExpirydate expirydate
        {
            get
            {
                return this.expirydateField;
            }
            set
            {
                this.expirydateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("card-colour")]
        public responseComponentresponsesVerificationresponseSearchresultMedicareCardcolour cardcolour
        {
            get
            {
                return this.cardcolourField;
            }
            set
            {
                this.cardcolourField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseSearchresultMedicareCardnumber
    {

        private string searchvalueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("search-value")]
        public string searchvalue
        {
            get
            {
                return this.searchvalueField;
            }
            set
            {
                this.searchvalueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseSearchresultMedicareReferencenumber
    {

        private string searchvalueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("search-value")]
        public string searchvalue
        {
            get
            {
                return this.searchvalueField;
            }
            set
            {
                this.searchvalueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseSearchresultMedicareExpirydate
    {

        private string searchvalueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("search-value")]
        public string searchvalue
        {
            get
            {
                return this.searchvalueField;
            }
            set
            {
                this.searchvalueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseSearchresultMedicareCardcolour
    {

        private string searchvalueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("search-value")]
        public string searchvalue
        {
            get
            {
                return this.searchvalueField;
            }
            set
            {
                this.searchvalueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseSearchresultDateofbirth
    {

        private System.DateTime searchvalueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("search-value", DataType = "date")]
        public System.DateTime searchvalue
        {
            get
            {
                return this.searchvalueField;
            }
            set
            {
                this.searchvalueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseSearchresultGender
    {

        private string searchvalueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("search-value")]
        public string searchvalue
        {
            get
            {
                return this.searchvalueField;
            }
            set
            {
                this.searchvalueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseSearchresultCurrentaddress
    {

        private responseComponentresponsesVerificationresponseSearchresultCurrentaddressUnitnumber unitnumberField;

        private responseComponentresponsesVerificationresponseSearchresultCurrentaddressStreetnumber streetnumberField;

        private responseComponentresponsesVerificationresponseSearchresultCurrentaddressStreetname streetnameField;

        private responseComponentresponsesVerificationresponseSearchresultCurrentaddressStreettype streettypeField;

        private responseComponentresponsesVerificationresponseSearchresultCurrentaddressSuburb suburbField;

        private responseComponentresponsesVerificationresponseSearchresultCurrentaddressState stateField;

        private responseComponentresponsesVerificationresponseSearchresultCurrentaddressPostcode postcodeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("unit-number")]
        public responseComponentresponsesVerificationresponseSearchresultCurrentaddressUnitnumber unitnumber
        {
            get
            {
                return this.unitnumberField;
            }
            set
            {
                this.unitnumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("street-number")]
        public responseComponentresponsesVerificationresponseSearchresultCurrentaddressStreetnumber streetnumber
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
        [System.Xml.Serialization.XmlElementAttribute("street-name")]
        public responseComponentresponsesVerificationresponseSearchresultCurrentaddressStreetname streetname
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
        [System.Xml.Serialization.XmlElementAttribute("street-type")]
        public responseComponentresponsesVerificationresponseSearchresultCurrentaddressStreettype streettype
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
        public responseComponentresponsesVerificationresponseSearchresultCurrentaddressSuburb suburb
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
        public responseComponentresponsesVerificationresponseSearchresultCurrentaddressState state
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
        public responseComponentresponsesVerificationresponseSearchresultCurrentaddressPostcode postcode
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
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseSearchresultCurrentaddressUnitnumber
    {

        private byte searchvalueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("search-value")]
        public byte searchvalue
        {
            get
            {
                return this.searchvalueField;
            }
            set
            {
                this.searchvalueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseSearchresultCurrentaddressStreetnumber
    {

        private ushort searchvalueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("search-value")]
        public ushort searchvalue
        {
            get
            {
                return this.searchvalueField;
            }
            set
            {
                this.searchvalueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseSearchresultCurrentaddressStreetname
    {

        private string searchvalueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("search-value")]
        public string searchvalue
        {
            get
            {
                return this.searchvalueField;
            }
            set
            {
                this.searchvalueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseSearchresultCurrentaddressStreettype
    {

        private string searchvalueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("search-value")]
        public string searchvalue
        {
            get
            {
                return this.searchvalueField;
            }
            set
            {
                this.searchvalueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseSearchresultCurrentaddressSuburb
    {

        private string searchvalueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("search-value")]
        public string searchvalue
        {
            get
            {
                return this.searchvalueField;
            }
            set
            {
                this.searchvalueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseSearchresultCurrentaddressState
    {

        private string searchvalueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("search-value")]
        public string searchvalue
        {
            get
            {
                return this.searchvalueField;
            }
            set
            {
                this.searchvalueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseSearchresultCurrentaddressPostcode
    {

        private ushort searchvalueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("search-value")]
        public ushort searchvalue
        {
            get
            {
                return this.searchvalueField;
            }
            set
            {
                this.searchvalueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesVerificationresponseSearchresultPhonenumbers
    {

        private object mobilephonenumberField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("mobile-phone-number")]
        public object mobilephonenumber
        {
            get
            {
                return this.mobilephonenumberField;
            }
            set
            {
                this.mobilephonenumberField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesFraudassessmentresponse
    {

        private decimal scoreField;

        private decimal thresholdField;

        private string resultField;

        private responseComponentresponsesFraudassessmentresponseAssessmentfactor[] assessmentfactorsField;

        /// <remarks/>
        public decimal score
        {
            get
            {
                return this.scoreField;
            }
            set
            {
                this.scoreField = value;
            }
        }

        /// <remarks/>
        public decimal threshold
        {
            get
            {
                return this.thresholdField;
            }
            set
            {
                this.thresholdField = value;
            }
        }

        /// <remarks/>
        public string result
        {
            get
            {
                return this.resultField;
            }
            set
            {
                this.resultField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute("assessment-factors")]
        [System.Xml.Serialization.XmlArrayItemAttribute("assessment-factor", IsNullable = false)]
        public responseComponentresponsesFraudassessmentresponseAssessmentfactor[] assessmentfactors
        {
            get
            {
                return this.assessmentfactorsField;
            }
            set
            {
                this.assessmentfactorsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://vedaxml.com/vxml2/idmatrix-v4-0.xsd")]
    public partial class responseComponentresponsesFraudassessmentresponseAssessmentfactor
    {

        private string typeField;

        private decimal scoreField;

        private decimal weightField;

        private decimal thresholdField;

        private string resultField;

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

        /// <remarks/>
        public decimal score
        {
            get
            {
                return this.scoreField;
            }
            set
            {
                this.scoreField = value;
            }
        }

        /// <remarks/>
        public decimal weight
        {
            get
            {
                return this.weightField;
            }
            set
            {
                this.weightField = value;
            }
        }

        /// <remarks/>
        public decimal threshold
        {
            get
            {
                return this.thresholdField;
            }
            set
            {
                this.thresholdField = value;
            }
        }

        /// <remarks/>
        public string result
        {
            get
            {
                return this.resultField;
            }
            set
            {
                this.resultField = value;
            }
        }
    }


}