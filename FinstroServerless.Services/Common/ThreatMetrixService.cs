using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using Finstro.Serverless.DynamoDB;
using Finstro.Serverless.Helper;
using Finstro.Serverless.Models;
using Finstro.Serverless.Models.Dynamo;
using Newtonsoft.Json;

namespace FinstroServerless.Services.Common
{
    public class ThreatMetrixService
    {
        private CreditApplicationDynamo _creditApplicationDynamo;
        private ThreatMetrixDynamo _threatMetrixDynamo;

        public ThreatMetrixService()
        {
            _creditApplicationDynamo = new CreditApplicationDynamo();
            _threatMetrixDynamo = new ThreatMetrixDynamo();
        }

        public EnumThreatMetrixStatus DeviceThreatMetrix(string userId)
        {
            CreditApplication application = _creditApplicationDynamo.GetCreditApplicationForUser(userId);
            EnumThreatMetrixStatus status = EnumThreatMetrixStatus.none;

            if (application.ThreatMetrix != null)
            {
                Enum.TryParse(application.ThreatMetrix.Status.ToLower(), out status);

                return status;
            }


            try
            {

                string strUrl = GetUrl(application);

                string json = string.Empty;

                using (WebClient objWeb = new WebClient())
                {
                    json = objWeb.DownloadString(strUrl);
                }

                ThreatMetrixResult threatMetrix = new ThreatMetrixResult();

                threatMetrix = JsonConvert.DeserializeObject<ThreatMetrixResult>(json);

                Enum.TryParse(threatMetrix.review_status.ToLower(), out status);

                status = (EnumThreatMetrixStatus)Enum.Parse(typeof(EnumThreatMetrixStatus), threatMetrix.review_status.ToLower(), true);


                application.ThreatMetrix.Status = status.GetDescription();
                application.ThreatMetrix.RequestId = threatMetrix.request_id;

                application.ThreatMetrix.DeviceId = threatMetrix.device_id;
                application.ThreatMetrix.PolicyScore = threatMetrix.policy_score;

                application.ThreatMetrix.Reasons = new List<string>();

                if (threatMetrix.reason_code.Count() > 0)
                {
                    var items = new Dictionary<string, int>();

                    foreach (var item in threatMetrix.policy_details_api.policy_detail_api)
                    {
                        foreach (var line in item.customer.rules)

                            if (!items.ContainsKey(line.reason_code))
                            {
                                items.Add(line.reason_code, Convert.ToInt32(line.score));
                            }
                    }
                    application.ThreatMetrix.Reasons = items.OrderBy(i => i.Value).Select(i => i.Key).ToList();

                }



                string file = AwsHelper.UploadFiles(application.UserSubId, EnumIdFileType.ThreatMetrix, Encoding.ASCII.GetBytes(json), "json");
                application.ThreatMetrix.FileUrl = file;
                application.ThreatMetrix.CreatedDate = DateTime.UtcNow;

                application.ThreatMetrix.RequestCount++;

                _creditApplicationDynamo.Update(application);

                if (status != EnumThreatMetrixStatus.review && status != EnumThreatMetrixStatus.challenge && status != EnumThreatMetrixStatus.pass)
                {
                    throw FinstroErrorType.CreditApplication.ThreatCheckCreditFailed;
                }

                if (status == EnumThreatMetrixStatus.fail || application.ThreatMetrix.RequestCount > 2)
                {
                    throw FinstroErrorType.CreditApplication.ThreatCheckFailed;
                }

                return status;

            }
            catch (Exception ex)
            {
                if (ex is FinstroErrorType)
                    throw ex;
                else
                {
                    if (application.ThreatMetrix != null)
                    {
                        application.ThreatMetrix.Status = EnumThreatMetrixStatus.fail.GetAttributeOfType<DescriptionAttribute>().Description;

                        _creditApplicationDynamo.Update(application);
                    }
                    return EnumThreatMetrixStatus.fail;
                }
            }

        }

        private string GetUrl(CreditApplication application)
        {

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            var contact = application.Contacts.FirstOrDefault();


            parameters.Add("api_key", "a3d4hsvej3mt4hpx");
            parameters.Add("session_id", application.ThreatMetrix.SessionId);
            parameters.Add("service_type", "All");
            parameters.Add("event_type", "ACCOUNT_CREATION");
            parameters.Add("customer_event_type", "facility_activation");
            parameters.Add("output_format", "json");
            parameters.Add("policy", "default");
            parameters.Add("account_login", contact.EmailAddress);
            parameters.Add("account_first_name", contact.FirstGivenName);
            parameters.Add("account_last_name", contact.FamilyName);
            parameters.Add("account_address_country", "AU");
            parameters.Add("account_address_postcode", application.ResidentialAddress.PostCode);

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("https://h-api.online-metrix.net/api/session-query?org_id=axi6nxue");


            foreach (var parameter in parameters)
            {
                if (!String.IsNullOrEmpty(parameter.Value))
                {
                    stringBuilder.AppendFormat("&{0}={1}", parameter.Key, parameter.Value);
                }
            }

            return stringBuilder.ToString();
        }


    }
    /*
public string FindThreatMetrixResult(string userId)
    {
        String result = THREAT_METRIX_FAIL;

        var application = _creditApplicationDynamo.GetCreditApplicationForUser(userId);


        List<BusinessPerson> businessPersons = this.businessPersonRepository.findByBusinessPartyId(clientId);
        if (CollectionUtils.isEmpty(businessPersons))
        {
            this.logger.info("Director does not exist with client id {}", clientId);
            throw new FccBusinessLogicException(("Director does not exist with client id " + clientId));
        }

        Optional<Person> person = this.personRepository.findById(businessPersons.get(0).getPerson().getPartyId());
        Optional<Business> business = this.businessRepository.findById(clientId);
        List<AssessmentInput> assessmentInputs = this.assessmentInputRepository.findByPartyAndTypeOrderByCreatedOnDesc(business.get(), AssessmentInputType.THREAT_METRIX);
        if (CollectionUtils.isEmpty(assessmentInputs))
        {
            this.logger.info("Threat metrix does not exist with client id {}", person.get().getPartyId());
            throw new FccBusinessLogicException(("Threat metrix does not exist with client id " + person.get().getPartyId()));
        }

        // get session id with party
        AssessmentInput currentAssessmentInput = assessmentInputs.get(0);
        AssessmentInputAttribute sessionAttribute = null;
        foreach (AssessmentInputAttribute attribute in currentAssessmentInput.getAssessmentInputAttributes())
        {
            if ("SESSION_ID".equals(attribute.getAttribute()))
            {
                sessionAttribute = attribute;
                break;
            }

        }

        if (Objects.isNull(sessionAttribute))
        {
            throw new FccBusinessLogicException(("Session id does not exist with client id " + clientId));
        }

        ResponseEntity<String> response = this.prepareSendRequest(person.get(), sessionAttribute);
        if ((response.getStatusCode() == HttpStatus.OK))
        {
            result = this.extractThreatMetrixResultFromJson(response.getBody(), currentAssessmentInput, sessionAttribute);
            this.saveThreatMetrixResultFile(response.getBody(), currentAssessmentInput);
        }
        else
        {
            currentAssessmentInput.setCreatedOn(LocalDate.now());
            currentAssessmentInput.setResult(AssessmentInputResultType.FAIL);
            result = THREAT_METRIX_FAIL;
            this.assessmentInputRepository.save(currentAssessmentInput);
            this.logger.error("Request threat metrix is failed with client id = {}", clientId);
        }

        return result;
    }

    public ResponseEntity<String> prepareSendRequest(Person person, AssessmentInputAttribute session)
    {
        Address address = person.getAddress();
        if (Objects.isNull(address))
        {
            this.logger.info("Address does not exist with party id {}", person.getPartyId());
            throw new FccBusinessLogicException(("Address does not exist with party id " + person.getPartyId()));
        }

        HttpHeaders requestHeaders = new HttpHeaders();
        requestHeaders.add("Accept", MediaType.APPLICATION_JSON_VALUE);
        HttpEntity<String> requestEntity = new HttpEntity(requestHeaders);
        UriComponentsBuilder uriBuilder = UriComponentsBuilder.fromHttpUrl(this.env.getRequiredProperty("threat.metrix.baseUrl")).queryParam("org_id", this.env.getRequiredProperty("threat.metrix.org_id")).queryParam("api_key", this.env.getRequiredProperty("threat.metrix.api_key")).queryParam("session_id", session.getValue()).queryParam("service_type", THREAT_SERVICE_TYPE_ALL).queryParam("event_type", THREAT_EVENT_TYPE_ACCOUNT_CREATION).queryParam("customer_event_type", THREAT_CUSTOMER_EVENT_TYPE).queryParam("output_format", THREAT_OUTPUT_JSON_FORMAT).queryParam("policy", THREAT_POLICY).queryParam("account_login", person.getEmail()).queryParam("account_first_name", person.getFirstName()).queryParam("account_last_name", person.getLastName()).queryParam("account_date_of_birth", dateFormatter.format(person.getDateOfBirth())).queryParam("account_address_street1", String.format("%s %s %s", address.getUnitOrLevel(), address.getStreetNumber(), address.getStreetName())).queryParam("account_address_city", address.getSuburb()).queryParam("account_address_country", address.getCountry()).queryParam("account_address_postcode", address.getPostCode());
        return this.restTemplate.exchange(uriBuilder.toUriString(), HttpMethod.GET, requestEntity, String.class);
}

[Transactional(rollbackFor = Exception.class)]
public String extractThreatMetrixResultFromJson(String jsonData, AssessmentInput assessmentInput, AssessmentInputAttribute sessionAttribute)
{
    String result = THREAT_METRIX_FAIL;
    JSONObject data = new JSONObject(jsonData);
    assessmentInput.getAssessmentInputAttributes().clear();
    assessmentInput.setResult(AssessmentInputResultType.PASS);
    assessmentInput.setCreatedOn(LocalDate.now());
    List<AssessmentInputAttribute> assessmentInputAttributes = new ArrayList();
    AssessmentInputAttribute assessmentInputAttribute = new AssessmentInputAttribute();
    assessmentInputAttribute.setAttribute(sessionAttribute.getAttribute());
    assessmentInputAttribute.setValue(sessionAttribute.getValue());
    assessmentInputAttributes.add(assessmentInputAttribute);
    if (data.has("device_id"))
    {
        assessmentInputAttribute = new AssessmentInputAttribute();
        assessmentInputAttribute.setAttribute("DEVICE_ID");
        assessmentInputAttribute.setValue(data.get("device_id").toString());
        assessmentInputAttributes.add(assessmentInputAttribute);
    }

    if (data.has("policy_score"))
    {
        assessmentInputAttribute = new AssessmentInputAttribute();
        assessmentInputAttribute.setAttribute("POLICY_SCORE");
        assessmentInputAttribute.setValue(data.get("policy_score").toString());
        assessmentInputAttributes.add(assessmentInputAttribute);
    }

    if (data.has("reason_code"))
    {
        JSONArray reasonCodes = data.getJSONArray("reason_code");
        int count = 0;
        for (int i = (reasonCodes.length() - 1); (i >= 0); i--)
        {
            assessmentInputAttribute = new AssessmentInputAttribute();
            assessmentInputAttribute.setAttribute("REASON_CODE");
            assessmentInputAttribute.setValue(((String)(reasonCodes.get(i))));
            assessmentInputAttributes.add(assessmentInputAttribute);
            count++;
            if ((count == 4))
            {
                break;
            }

        }

    }

    if (data.has(JSON_FIELD_REVIEW_STATUS))
    {
        assessmentInputAttribute = new AssessmentInputAttribute();
        assessmentInputAttribute.setAttribute("REVIEW_STATUS");
        if ("Accept".equals(data.get(JSON_FIELD_REVIEW_STATUS).toString()))
        {
            assessmentInputAttribute.setValue(THREAT_METRIX_PASS);
        }
        else if ("Review".equals(data.get(JSON_FIELD_REVIEW_STATUS).toString()))
        {
            assessmentInputAttribute.setValue(THREAT_METRIX_REFER);
        }
        else if ("Reject".equals(data.get(JSON_FIELD_REVIEW_STATUS).toString()))
        {
            assessmentInputAttribute.setValue(THREAT_METRIX_FAIL);
        }
        else
        {
            assessmentInputAttribute.setValue(THREAT_METRIX_FAIL);
        }

        result = assessmentInputAttribute.getValue();
        assessmentInputAttributes.add(assessmentInputAttribute);
    }

    assessmentInput.getAssessmentInputAttributes().addAll(assessmentInputAttributes);
    this.assessmentInputRepository.save(assessmentInput);
    return result;
}

public void saveThreatMetrixResultFile(String data, AssessmentInput assessmentInput)
{
    try
    {
        String folderName = String.format("%s%sTM_%s", assessmentInput.getParty().getPartyId(), File.separator, System.currentTimeMillis());
        String fileName = String.format("%s.json", System.currentTimeMillis());
        this.fileStorageService.writeToFile(folderName, fileName, data);
        String pathFile = String.format("%s/%s", folderName, fileName);
        AssessmentInputAttribute assessmentInputAttribute = new AssessmentInputAttribute();
        assessmentInputAttribute.setAttribute("FOLDER_PATH");
        assessmentInputAttribute.setValue(pathFile);
        foreach (AssessmentInputAttribute attribute in assessmentInput.getAssessmentInputAttributes())
        {
            if ("FOLDER_PATH".equals(attribute.getAttribute()))
            {
                assessmentInputAttribute.setAssessmentInputAttributeId(attribute.getAssessmentInputAttributeId());
                this.assessmentInputRepository.save(assessmentInput);
                return;
            }

        }

        assessmentInput.getAssessmentInputAttributes().add(assessmentInputAttribute);
        this.assessmentInputRepository.save(assessmentInput);
    }
    catch (IOException e)
    {
        this.logger.error("Can\'t save file with name {}", e.getMessage());
        throw new FccBusinessLogicException("Can\'t save file");
    }

}
}*/
}
