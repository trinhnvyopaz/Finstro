using Finstro.Serverless.Models.Request.Incc;
using Finstro.Serverless.Models.Response.Incc;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using RestSharp.Extensions;
using System.Threading.Tasks;
using Finstro.Serverless.Helper;
using Finstro.Serverless.DynamoDB;
using Finstro.Serverless.Models.Dynamo;
using Newtonsoft.Json;
using System.Net;
using System.Linq;
using Finstro.Serverless.Models;
using System.ComponentModel;
using Audit.DynamoDB.Providers;
using Amazon.DynamoDBv2;
using Amazon;
using Audit.Core;
using Finstro.Serverless.Models.Entity;
using ServiceStack.Text;

namespace FinstroServerless.Services.Incc
{
    public class InccService
    {
        private RestClient client;
        private FinstroRunningValuesDynamo dynamo;
        private CreditApplicationDynamo creditApplicationDynamo;
        private static readonly RegionEndpoint _region = RegionEndpoint.USEast2;

        public InccService()
        {
            client = new RestClient(AppSettings.InccSettings.URL);
            client.Timeout = 60000;

            dynamo = new FinstroRunningValuesDynamo();
            creditApplicationDynamo = new CreditApplicationDynamo();


            Audit.Core.Configuration.Setup().UseDynamoDB(config => config
                                    .WithClient(new AmazonDynamoDBClient(_region))
                                    .Table("FinstroAuditEvent")
                                    .SetAttribute("EventId", ev => Guid.NewGuid())
                                    );


        }

        public Get24HourTokenResponse Get24HourToken(bool force = false)
        {

            Get24HourTokenResponse response = new Get24HourTokenResponse();

            FinstroRunningValues values = dynamo.GetFinstroRunningValues();

            if (!force && values.InccToken != null && values.InccToken.CreatedDate > DateTime.UtcNow.AddHours(-24))
            {
                response = values.InccToken;
            }
            else
            {
                var request = new RestRequest("api/getsecutoken24", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("cx_user", AppSettings.InccSettings.CxUser);

                Get24HourTokenRequest get24HourToken = new Get24HourTokenRequest()
                {
                    CorporateId = AppSettings.InccSettings.CorporateId,
                    InstitutionId = AppSettings.InccSettings.InstitutionId,
                    MacValue = string.Empty,
                    Password = AppSettings.InccSettings.CxPassword,
                    Reason = AppSettings.InccSettings.CxReason
                };


                request.AddJsonBody(JsonConvert.SerializeObject(get24HourToken));

                var data = client.Execute(request);

                if (data.StatusCode == HttpStatusCode.OK)
                {
                    response = JsonConvert.DeserializeObject<Get24HourTokenResponse>(data.Content);
                    response.CreatedDate = DateTime.UtcNow;
                    values.InccToken = response;

                    values.InccToken.Token24F1 = Finstro.Serverless.Helper.Common.GetRandomString(16);
                    values.InccToken.Token24F2 = Finstro.Serverless.Helper.Common.GetRandomString(16);


                    dynamo.Update(values);
                }
                else
                {
                    throw FinstroErrorType.INCC.CouldNotGet24HourToken;
                }
            }
            return response;

        }

        public CreateCardResponse CreateCard(string userId)
        {
            var application = creditApplicationDynamo.GetCreditApplicationForUser(userId);
            return CreateCard(application);
        }

        public CreateCardResponse CreateCard(CreditApplication application)
        {

            CreateCardResponse response = new CreateCardResponse();

            if (application.FinstroCards == null)
                application.FinstroCards = new List<FinstroCardDetail>();

            var token = Get24HourToken();

            if (token.CreatedDate > DateTime.UtcNow.AddHours(-24))
            {
                var request = new RestRequest("api/getcreatecard", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("institution_id", AppSettings.InccSettings.InstitutionId);
                request.AddHeader("corporate_id", AppSettings.InccSettings.CorporateId);
                var contact = application.Contacts.FirstOrDefault();
                FinstroCardDetail card = application.FinstroCards.FirstOrDefault();
                bool add = false;
                if (card == null)
                {
                    card = new FinstroCardDetail();
                    add = true;
                }

                if (card.PostalAddress == null)
                {
                    card.PostalAddress = application.ResidentialAddress != null ? application.ResidentialAddress : application.BusinessDetails.BusinessTradingAddress;
                }

                var address = card.PostalAddress;

                string middle = !string.IsNullOrEmpty(application.DrivingLicence?.MiddleName) ? application.DrivingLicence?.MiddleName.Substring(0, 1) : string.Empty;

                if (string.IsNullOrEmpty(middle))
                    middle = !string.IsNullOrEmpty(application.MedicareCard?.MiddleInitial) ? application.MedicareCard?.MiddleInitial.Substring(0, 1) : string.Empty;

                string emboss = $"{contact.FirstGivenName} {middle} {contact.FamilyName}".Replace("  ", " ");
                string recipientName = emboss;

                if (emboss.Length >= AppSettings.InccSettings.EmbossMaxSize)
                {
                    emboss = $"{contact.FirstGivenName} {contact.FamilyName}".Replace("  ", " ");

                    if (emboss.Length >= AppSettings.InccSettings.EmbossMaxSize)
                    {
                        emboss = $"{contact.FirstGivenName.Substring(0, 1)} {middle} {contact.FamilyName}".Replace("  ", " ");

                        if (emboss.Length >= AppSettings.InccSettings.EmbossMaxSize)
                        {
                            emboss = $"{contact.FirstGivenName.Substring(0, 1)} {contact.FamilyName}".Replace("  ", " ");
                        }
                    }
                }

                if (emboss.Length >= AppSettings.InccSettings.EmbossMaxSize)
                {
                    emboss = emboss.Substring(0, AppSettings.InccSettings.EmbossMaxSize - 1);
                }

                CreateCardRequest cardRequest = new CreateCardRequest()
                {
                    AddressLine1 = $"{address.UnitOrLevel} {address.StreetNumber} {address.StreetName} {address.StreetType}",
                    ConsumerId = application.Id.ToString(),
                    Device = AppSettings.InccSettings.CountryCode,
                    DeliveryMethod = EnumInccDeliveryMethod.StandardAustraliaPost.GetDescription(),
                    Email = contact.EmailAddress,
                    EmbossName = emboss,
                    FirstName = contact.FirstGivenName,
                    LastName = contact.FamilyName,
                    Mobile = ValidationHelper.FormatPhoneNumberNational(contact.MobilePhoneNumber, "AU").Replace(" ", ""),
                    PostCode = address.PostCode,
                    ProductCode = AppSettings.InccSettings.ProductCode,
                    RecipientName = recipientName,
                    State = address.State,
                    Suburb = address.Suburb,
                    AddressLine2 = string.Empty,
                    Token24E1 = token.Token24E1,
                    Npai = AppSettings.InccSettings.CountryCode,
                    MacValue = string.Empty
                };


                request.AddJsonBody(JsonConvert.SerializeObject(cardRequest));

                var data = client.Execute(request);

                if (data.StatusCode == HttpStatusCode.OK)
                {
                    response = JsonConvert.DeserializeObject<CreateCardResponse>(data.Content);
                    card.InccCard = response;
                    card.InccCard.OpenToBuy = application.BusinessDetails.SelectedCreditAmount;
                    card.InccCard.Status = EnumInccCardStatus.Inactive.GetDescription();
                    card.InccCard.InccStatusCode = EnumInccCardStatusCode.Inactive.GetValue();


                    if (add)
                        application.FinstroCards.Add(card);

                    application.ModifiedDate = DateTime.UtcNow;

                    creditApplicationDynamo.Update(application);
                }
                else
                {
                    throw FinstroErrorType.INCC.CouldNotCreateCard;
                }
            }
            return response;

        }



        public bool UpdateCard(CreditApplication application, FinstroCardDetail card, bool renew = false)
        {
            bool result = false;

            string sRenew = "N";

            if (renew)
                sRenew = "Y";

            if (application.FinstroCards == null)
                application.FinstroCards = new List<FinstroCardDetail>();

            var token = Get24HourToken();

            if (token.CreatedDate > DateTime.UtcNow.AddHours(-24))
            {
                var request = new RestRequest("api/getupdatecard", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("crt_card_token", card.InccCard.CardToken);
                request.AddHeader("corporate_id", AppSettings.InccSettings.CorporateId);
                var contact = application.Contacts.FirstOrDefault();

                var address = card.PostalAddress;

                string middle = !string.IsNullOrEmpty(application.DrivingLicence?.MiddleName) ? application.DrivingLicence?.MiddleName.Substring(0, 1) : string.Empty;

                if (string.IsNullOrEmpty(middle))
                    middle = !string.IsNullOrEmpty(application.MedicareCard?.MiddleInitial) ? application.MedicareCard?.MiddleInitial.Substring(0, 1) : string.Empty;

                string emboss = $"{contact.FirstGivenName} {middle} {contact.FamilyName}".Replace("  ", " ");
                string recipientName = emboss;

                if (emboss.Length >= AppSettings.InccSettings.EmbossMaxSize)
                {
                    emboss = $"{contact.FirstGivenName} {contact.FamilyName}".Replace("  ", " ");

                    if (emboss.Length >= AppSettings.InccSettings.EmbossMaxSize)
                    {
                        emboss = $"{contact.FirstGivenName.Substring(0, 1)} {middle} {contact.FamilyName}".Replace("  ", " ");

                        if (emboss.Length >= AppSettings.InccSettings.EmbossMaxSize)
                        {
                            emboss = $"{contact.FirstGivenName.Substring(0, 1)} {contact.FamilyName}".Replace("  ", " ");
                        }
                    }
                }

                if (emboss.Length >= AppSettings.InccSettings.EmbossMaxSize)
                {
                    emboss = emboss.Substring(0, AppSettings.InccSettings.EmbossMaxSize - 1);
                }

                CreateCardRequest cardRequest = new CreateCardRequest()
                {
                    CorporateId = AppSettings.InccSettings.CorporateId,
                    InstitutionId = AppSettings.InccSettings.InstitutionId,
                    Renew = sRenew,
                    CxToken24E1 = token.Token24E1,
                    AddressLine1 = $"{address.UnitOrLevel} {address.StreetNumber} {address.StreetName} {address.StreetType}",
                    PostCode = address.PostCode,
                    State = address.State,
                    Suburb = address.Suburb.PadRight(30),
                    ConsumerId = card.InccCard.ConsumerId,
                    ProductCode = AppSettings.InccSettings.ProductCode,
                    Device = AppSettings.InccSettings.CountryCode,
                    DeliveryMethod = EnumInccDeliveryMethod.StandardAustraliaPost.GetDescription(),
                    Email = contact.EmailAddress,
                    EmbossName = emboss,
                    FirstName = contact.FirstGivenName,
                    LastName = contact.FamilyName,
                    Mobile = ValidationHelper.FormatPhoneNumberNational(contact.MobilePhoneNumber, "AU").Replace(" ", ""),
                    RecipientName = recipientName,
                    AddressLine2 = string.Empty,
                    Npai = AppSettings.InccSettings.CountryCode,
                    MacValue = string.Empty
                };


                request.AddJsonBody(JsonConvert.SerializeObject(cardRequest));

                var data = client.Execute(request);

                if (data.StatusCode == HttpStatusCode.OK)
                {
                    result = true;
                    application.ModifiedDate = DateTime.UtcNow;
                    creditApplicationDynamo.Update(application);
                }
                else
                {
                    //REMOVE once INCC fix update Endpoint
                    if (data.Content.IndexOf("suburb must be equal to 30 characters") > 0)
                        return true;

                    var error = FinstroErrorType.INCC.CouldNotUpdateCard;
                    error.Description = data.Content;
                    throw error;
                }
            }
            return result;

        }


        public Get30MinTokenResponse Get30MinToken(string userId, string cardToken)
        {

            Get30MinTokenResponse response = new Get30MinTokenResponse();
            var application = creditApplicationDynamo.GetCreditApplicationForUser(userId);

            if (application.FinstroCards == null)
                throw FinstroErrorType.INCC.NoCardAvailable;

            if (application.FinstroCards.Count(c => c.InccCard.CardToken == cardToken) == 0)
                throw FinstroErrorType.INCC.NoCardAvailable;

            var token = Get24HourToken();

            if (token.CreatedDate > DateTime.UtcNow.AddHours(-24))
            {
                var request = new RestRequest("api/gettoken_30", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("crt_card_token", cardToken);


                Get30MinTokenRequest tokenRequest = new Get30MinTokenRequest()
                {
                    CardToken = cardToken,
                    CorporateId = AppSettings.InccSettings.CorporateId,
                    InstitutionId = AppSettings.InccSettings.InstitutionId,
                    MacValue = string.Empty,
                    Token24E1 = token.Token24E1
                };


                request.AddJsonBody(JsonConvert.SerializeObject(tokenRequest));

                var data = client.Execute(request);

                if (data.StatusCode == HttpStatusCode.OK)
                {
                    response = JsonConvert.DeserializeObject<Get30MinTokenResponse>(data.Content);
                }
                else
                {
                    if (data.Content.IndexOf("Invalid 24 security token") > 0)
                        Get24HourToken(true);

                    var error = FinstroErrorType.INCC.CouldNotGet30MinToken;
                    error.Description = $"Please try again. Error: {data.Content}";
                    throw error;
                }
            }
            return response;

        }


        public Shared24HourTokenResponse GetShared24hToken(string RemoteIpAddress = null)
        {
           // _ = SaveAuditAsync("Transaction:GetShared24hToken:InitialLog", new FinstroAuditEvent("incc5cc6-3ca8-4578-8af7-5ca11d42c212", RemoteIpAddress, null), null);

            var token = Get24HourToken();


            return new Shared24HourTokenResponse()
            {
                Token24F1 = token.Token24F1,
                Token24F2 = token.Token24F2,
                ExpireAt = token.CreatedDate.AddHours(24)
            };

        }


        public GetCardDataResponse GetCardData(string userId, GetCardDataRequest cardData)
        {

            GetCardDataResponse response = new GetCardDataResponse();
            var application = creditApplicationDynamo.GetCreditApplicationForUser(userId);

            if (application.FinstroCards == null)
                throw FinstroErrorType.INCC.NoCardAvailable;

            string cardToken = cardData.CardToken;

            var card = application.FinstroCards.FirstOrDefault(c => c.InccCard.CardToken == cardToken);

            if (card == null)
                throw FinstroErrorType.INCC.NoCardAvailable;

            var token = Get24HourToken();

            if (token.CreatedDate > DateTime.UtcNow.AddHours(-24))
            {
                var request = new RestRequest("api/getcarddata", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("crt_card_token", cardToken);


                request.AddJsonBody(JsonConvert.SerializeObject(cardData));

                var data = client.Execute(request);

                if (data.StatusCode == HttpStatusCode.OK)
                {
                    response = JsonConvert.DeserializeObject<GetCardDataResponse>(data.Content);

                    if (string.IsNullOrEmpty(response.Status))
                        response.Status = EnumInccCardStatusCode.PermanentBlock.GetValue();

                    EnumInccCardStatusCode inccCardStatusCod = EnumHelper.GetFromValue<EnumInccCardStatusCode>(response.Status);

                    if (card.InccCard.InccStatusCode != inccCardStatusCod.GetValue() || card.InccCard.Status != inccCardStatusCod.GetDescription())
                    {
                        card.InccCard.InccStatusCode = inccCardStatusCod.GetValue();
                        card.InccCard.Status = inccCardStatusCod.GetDescription();
                        creditApplicationDynamo.Update(application);
                    }

                    if (card.InccCard.CardMaskedNumber != response.PanNumber)
                    {

                        card.InccCard.CardMaskedNumber = response.PanNumber;
                        card.InccCard.CardEmbossName = $"{response.FirstName} {response.LastName}";
                        creditApplicationDynamo.Update(application);
                    }

                }
                else
                {
                    var error = FinstroErrorType.INCC.CouldNotGetCard;
                    error.Description = data.Content;
                    throw error;
                }
            }
            return response;

        }


        public void ActivateCard(string userId, GetCardDataRequest cardData)
        {

            GetCardDataResponse response = new GetCardDataResponse();
            var application = creditApplicationDynamo.GetCreditApplicationForUser(userId);

            if (application.FinstroCards == null)
                throw FinstroErrorType.INCC.NoCardAvailable;

            string cardToken = cardData.CardToken;

            var card = application.FinstroCards.FirstOrDefault(c => c.InccCard.CardToken == cardToken);

            if (card == null)
                throw FinstroErrorType.INCC.NoCardAvailable;

            if (card.InccCard.Status != EnumInccCardStatus.Active.GetDescription())
            {
                card.InccCard.Status = EnumInccCardStatus.Active.GetDescription();
                creditApplicationDynamo.Update(application);
            }
        }

        public void LockUnlockCard(string userId, GetCardDataRequest cardData, EnumInccCardAction action)
        {

            GetCardDataResponse response = new GetCardDataResponse();
            var application = creditApplicationDynamo.GetCreditApplicationForUser(userId);

            if (application.FinstroCards == null)
                throw FinstroErrorType.INCC.NoCardAvailable;

            string cardToken = cardData.CardToken;

            var card = application.FinstroCards.FirstOrDefault(c => c.InccCard.CardToken == cardToken);

            if (card == null)
                throw FinstroErrorType.INCC.NoCardAvailable;

            if (action == EnumInccCardAction.TemporaryLockCard && card.InccCard.Status != EnumInccCardStatus.Active.GetDescription())
                throw FinstroErrorType.INCC.CardNotActive;

            if (action == EnumInccCardAction.UnlockCard && card.InccCard.Status == EnumInccCardStatus.PermLocked.GetDescription())
                throw FinstroErrorType.INCC.CardPermLocked;

            if (action == EnumInccCardAction.UnlockCard && card.InccCard.Status != EnumInccCardStatus.Locked.GetDescription())
                throw FinstroErrorType.INCC.CardNotLocked;


            string reason = string.Empty;
            EnumInccCardStatus statusResult = EnumInccCardStatus.Active;

            switch (action)
            {
                case EnumInccCardAction.TemporaryLockCard:
                    reason = EnumInccReason.Reason_41.GetDescription();
                    statusResult = EnumInccCardStatus.Locked;
                    break;
                case EnumInccCardAction.PermanentLock:
                    reason = EnumInccReason.Reason_04.GetDescription();
                    statusResult = EnumInccCardStatus.PermLocked;
                    break;
                case EnumInccCardAction.UnlockCard:
                    reason = EnumInccReason.Reason_00.GetDescription();
                    statusResult = EnumInccCardStatus.Active;
                    break;
                default:
                    reason = "";
                    break;
            }

            var token = Get24HourToken();

            if (token.CreatedDate > DateTime.UtcNow.AddHours(-24))
            {
                var request = new RestRequest("api/getlockunlockcard", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("crt_card_token", cardToken);

                dynamic lockUnlockRequest = new
                {
                    corporate_id = AppSettings.InccSettings.CorporateId,
                    institution_id = AppSettings.InccSettings.InstitutionId,
                    crt_securtoken_24_e1 = token.Token24E1,
                    crt_card_token = cardData.CardToken,
                    crt_action = action.GetDescription(),
                    crt_reason = reason,
                    mac_value = ""
                };



                request.AddJsonBody(JsonConvert.SerializeObject(lockUnlockRequest));

                var data = client.Execute(request);

                if (data.StatusCode == HttpStatusCode.OK)
                {

                    var options = new AuditScopeOptions()
                    {
                        EventType = "InccCard:LockUnlockCard",
                        CreationPolicy = EventCreationPolicy.Manual,
                        AuditEvent = new FinstroAuditEvent(userId),
                        TargetGetter = () => card.InccCard
                    };


                    using (var scope = AuditScope.Create(options))
                    {

                        response = JsonConvert.DeserializeObject<GetCardDataResponse>(data.Content);
                        card.InccCard.Status = statusResult.GetDescription();
                        //card.InccCard.Status = EnumInccCardStatus.Locked.GetDescription();
                        creditApplicationDynamo.Update(application);
                    }
                }
                else
                {
                    //if (data.Content.IndexOf("Card is Blocked permanently") > 0)
                    //    return;
                    var error = FinstroErrorType.INCC.CouldNotGetCard;
                    error.Description = data.Content;
                    throw error;
                }
            }
        }

        public void LogCall(string name, string ip)
        {
            AuditScope.CreateAndSave(name, new { ExtraField = ip , UserSubId  = "incc5cc6-3ca8-4578-8af7-5ca11d42c212" });
        }

        public GetAuthorizationResponse GetAuthorization(GetAuthorizationRequest request)
        {
            //_ = SaveAuditAsync("Transaction:GetAuthorization:InitialLog", new FinstroAuditEvent("incc5cc6-3ca8-4578-8af7-5ca11d42c212", request.crt_numauto, request), null);


            GetAuthorizationResponse response = new GetAuthorizationResponse()
            {
                ErrorCode = EnumInccErrorMessage.Approved.GetValue(),
                ErrorMessage = EnumInccErrorMessage.Approved.GetName(),
                CardToken = request.trx_card_token,
                InstitutionId = request.institution_id,
                CorporateId = request.corporate_id,
                Stan = request.trx_stan,
                ReferenceNumber = request.trs_rrn,
                TerminalId = request.trx_terminal_id,
                LocalDate = request.local_date,
                LocalTime = request.local_time,
                Amount = request.trx_amount,
                TransmissionDateTime = request.transm_dat_time

            };

            try
            {
                EnumInccResponseMessageType inccResponseMessageType = EnumInccResponseMessageType.Type_0110;
                EnumInccMessageType messageType = EnumHelper.GetValueFromDescription<EnumInccMessageType>(request.msg_type);

                switch (messageType)
                {
                    case EnumInccMessageType.Type_0100:
                        inccResponseMessageType = EnumInccResponseMessageType.Type_0110;
                        break;
                    case EnumInccMessageType.Type_0200:
                        inccResponseMessageType = EnumInccResponseMessageType.Type_0210;
                        break;
                    case EnumInccMessageType.Type_0420:
                        inccResponseMessageType = EnumInccResponseMessageType.Type_0430;
                        break;
                    case EnumInccMessageType.Type_0421:
                        inccResponseMessageType = EnumInccResponseMessageType.Type_0230;
                        break;

                    default:
                        break;
                }

                response.MessageType = inccResponseMessageType.GetDescription();

                EnumInccPurpose inccPurpose = EnumInccPurpose.Unknown;

                inccPurpose = ExtractPurpose(request, inccPurpose);
                if (inccPurpose == EnumInccPurpose.Unknown)
                {
                    response.ErrorCode = EnumInccErrorMessage.UnknownTransactionPurpose.GetValue();
                    response.ErrorMessage = EnumInccErrorMessage.UnknownTransactionPurpose.GetName();
                    return response;
                }

                if (inccPurpose == EnumInccPurpose.PurchaseReversalWithCashOut || inccPurpose == EnumInccPurpose.PurchaseWithCashOut)
                {
                    response.ErrorCode = EnumInccErrorMessage.Declined.GetValue();
                    response.ErrorMessage = EnumInccErrorMessage.Declined.GetName();
                    return response;
                }



                var token24 = Get24HourToken();
                response.Token24E1 = token24.Token24E1;

                if (request.crt_securtoken_24_f1 != token24.Token24F1)
                {
                    response.ErrorCode = EnumInccErrorMessage.InvalidToken24F1Token.GetValue();
                    response.ErrorMessage = EnumInccErrorMessage.InvalidToken24F1Token.GetName();
                    return response;
                }


                request.transaction_purpose = inccPurpose.ToString();

                var application = creditApplicationDynamo.GetCardByToken(request.trx_card_token);

                if (application.FinstroCards == null)
                    throw FinstroErrorType.INCC.NoCardAvailable;

                string cardToken = request.trx_card_token;

                var card = application.FinstroCards.FirstOrDefault(c => c.InccCard.CardToken == cardToken);

                if (card == null)
                    throw FinstroErrorType.INCC.NoCardAvailable;


                //REMOVE COMMENT
                //if (card.InccCard.Status != EnumInccCardStatus.Active.GetDescription())
                //    throw FinstroErrorType.INCC.CardNotActive;
                AuditEventsDynamo eventDynamo = new AuditEventsDynamo();

                decimal amount = Int32.Parse(request.trx_amount) / 100.0m;

                if (inccPurpose == EnumInccPurpose.Refund)
                {


                    var log = eventDynamo.GetByTransactionNumber(application.UserSubId, request.crt_numauto);




                    if (log != null)
                    {
                        //add logic to cross match refund with original tx
                    }

                    card.InccCard.OpenToBuy += amount;

                    creditApplicationDynamo.Update(application);


                    _ = SaveAuditAsync("Transaction:GetAuthorization:Refund", new FinstroAuditEvent(application.UserSubId, request.crt_numauto, request), null);


                    return response;
                }
                else if (inccPurpose == EnumInccPurpose.PurchaseReversal || inccPurpose == EnumInccPurpose.RefundReversal)
                {
                    var logs = eventDynamo.GetByLatestByMinutes(application.UserSubId, -30);
                    if (logs.Count > 0)
                    {
                        //var test = logs[0].AuthorizationRequest.ChangeType(typeof(GetAuthorizationRequest));

                        var tx = logs.Where(l => l.AuthorizationRequest != null &&
                                                l.AuthorizationRequest.trx_stan == request.trs_rrn &&
                                                l.AuthorizationRequest.trx_accep_id == request.trx_accep_id &&
                                                l.AuthorizationRequest.trx_amount == request.trx_amount).ToList();

                        if (tx.Count(t => t.AuthorizationRequest.transaction_purpose == inccPurpose.ToString()) > 0)
                        {
                            response.ErrorCode = EnumInccErrorMessage.Declined.GetValue();
                            response.ErrorMessage = EnumInccErrorMessage.Declined.GetName();
                            return response;
                        }

                        if (tx != null)
                        {
                            var reverseTx = tx.FirstOrDefault();

                            if (reverseTx.AuthorizationRequest.transaction_purpose == EnumInccPurpose.Purchase.ToString())
                                card.InccCard.OpenToBuy += amount;

                            if (reverseTx.AuthorizationRequest.transaction_purpose == EnumInccPurpose.Refund.ToString())
                                card.InccCard.OpenToBuy -= amount;

                            creditApplicationDynamo.Update(application);

                            _ = SaveAuditAsync("Transaction:GetAuthorization:Reversal",
                                                new FinstroAuditEvent(application.UserSubId, request.crt_numauto, request), null);

                            return response;
                        }
                        else
                        {
                            response.ErrorCode = EnumInccErrorMessage.Declined.GetValue();
                            response.ErrorMessage = EnumInccErrorMessage.Declined.GetName();
                            return response;
                        }


                    }
                    else
                    {
                        _ = EmailHelper.NotifyRefundFail(JsonConvert.SerializeObject(request));
                        response.ErrorCode = EnumInccErrorMessage.Declined.GetValue();
                        response.ErrorMessage = EnumInccErrorMessage.Declined.GetName();
                        return response;
                    }

                }



                var numberAuto = Finstro.Serverless.Helper.Common.GetTransactionAutoNumber(6);
                response.NumberAuto = numberAuto;

                //var log = creditApplicationDynamo.GetLog(application.UserSubId);



                if (card.InccCard.Status != EnumInccCardStatus.Active.GetDescription())
                {
                    response.ErrorCode = EnumInccErrorMessage.CardNotActive.GetValue();
                    response.ErrorMessage = EnumInccErrorMessage.CardNotActive.GetName();
                    return response;
                }




                if (inccPurpose == EnumInccPurpose.Purchase)
                {
                    if (amount > card.InccCard.OpenToBuy)
                    {
                        response.ErrorCode = EnumInccErrorMessage.RequestedAmountExceedsLimit.GetValue();
                        response.ErrorMessage = EnumInccErrorMessage.RequestedAmountExceedsLimit.GetName();
                        return response;
                    }

                }




                if (response.ErrorCode == EnumInccErrorMessage.Approved.GetValue())
                {
                    if (inccPurpose == EnumInccPurpose.Purchase)
                    {

                        request.crt_numauto = numberAuto;

                        card.InccCard.OpenToBuy -= amount;

                        creditApplicationDynamo.Update(application);

                        _ = SaveAuditAsync("Transaction:GetAuthorization:Purchase", new FinstroAuditEvent(application.UserSubId, request.crt_numauto, request), null);

                    }
                    else if (inccPurpose == EnumInccPurpose.ATMBalanceInquiry)
                    {
                        string sAmount = String.Format("{0:F2}", card.InccCard.OpenToBuy).Replace(".", "").Replace(",", "").PadLeft(12, '0');

                        response.Amount = sAmount;
                    }
                }


                return response;
            }
            catch (Exception ex)
            {
                response.ErrorCode = EnumInccErrorMessage.Declined.GetValue();
                response.ErrorMessage = EnumInccErrorMessage.Declined.GetName();
                return response;
            }

        }

        private static EnumInccPurpose ExtractPurpose(GetAuthorizationRequest authorizationRequest, EnumInccPurpose inccPurpose)
        {
            EnumInccMessageType messageType = EnumHelper.GetValueFromDescription<EnumInccMessageType>(authorizationRequest.msg_type);
            EnumInccProcessCode processCode = EnumHelper.GetValueFromDescription<EnumInccProcessCode>(authorizationRequest.trx_process_code);

            switch (messageType)
            {
                case EnumInccMessageType.Type_0100:
                    switch (processCode)
                    {
                        case EnumInccProcessCode.Code_00:
                            inccPurpose = EnumInccPurpose.Purchase;
                            break;
                        case EnumInccProcessCode.Code_09:
                            inccPurpose = EnumInccPurpose.PurchaseWithCashOut;
                            break;
                        case EnumInccProcessCode.Code_30:
                            inccPurpose = EnumInccPurpose.ATMBalanceInquiry;
                            break;
                        case EnumInccProcessCode.Code_31:
                            inccPurpose = EnumInccPurpose.ATMBalanceInquiry;
                            break;
                        case EnumInccProcessCode.Code_20:
                            inccPurpose = EnumInccPurpose.Refund;
                            break;
                        default:
                            break;
                    }
                    break;
                case EnumInccMessageType.Type_0200:
                    switch (processCode)
                    {
                        case EnumInccProcessCode.Code_30:
                            inccPurpose = EnumInccPurpose.ATMBalanceInquiry;
                            break;
                        case EnumInccProcessCode.Code_31:
                            inccPurpose = EnumInccPurpose.ATMBalanceInquiry;
                            break;
                        default:
                            break;
                    }
                    break;
                case EnumInccMessageType.Type_0420:
                    switch (processCode)
                    {
                        case EnumInccProcessCode.Code_00:
                            inccPurpose = EnumInccPurpose.PurchaseReversal;
                            break;
                        case EnumInccProcessCode.Code_09:
                            inccPurpose = EnumInccPurpose.PurchaseReversalWithCashOut;
                            break;
                        case EnumInccProcessCode.Code_20:
                            inccPurpose = EnumInccPurpose.RefundReversal;
                            break;
                        default:
                            break;
                    }
                    break;
                case EnumInccMessageType.Type_0421:
                    switch (processCode)
                    {
                        case EnumInccProcessCode.Code_00:
                            inccPurpose = EnumInccPurpose.PurchaseReversal;
                            break;
                        case EnumInccProcessCode.Code_09:
                            inccPurpose = EnumInccPurpose.PurchaseReversalWithCashOut;
                            break;
                        case EnumInccProcessCode.Code_20:
                            inccPurpose = EnumInccPurpose.RefundReversal;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }

            return inccPurpose;
        }


        public void CancelCard(CardActionRequest cardData)
        {
            string userId = cardData.UserId;

            GetCardDataResponse response = new GetCardDataResponse();
            var application = creditApplicationDynamo.GetCreditApplicationForUser(userId);

            if (application.FinstroCards == null)
                throw FinstroErrorType.INCC.NoCardAvailable;

            string cardToken = cardData.CardToken;

            var card = application.FinstroCards.FirstOrDefault(c => c.InccCard.CardToken == cardToken);

            if (card == null)
                throw FinstroErrorType.INCC.NoCardAvailable;

            if (!CheckIfCardCanBeCancelled(application, card))
                throw FinstroErrorType.INCC.CannotCancelCard;

            var token = Get24HourToken();

            if (token.CreatedDate > DateTime.UtcNow.AddHours(-24))
            {
                var request = new RestRequest("api/getcancelcard", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("crt_card_token", cardToken);

                dynamic cancelCard = new
                {
                    corporate_id = AppSettings.InccSettings.CorporateId,
                    institution_id = AppSettings.InccSettings.InstitutionId,
                    crt_securtoken_24_e1 = token.Token24E1,
                    crt_card_token = cardData.CardToken,
                    consumer_id = card.InccCard.ConsumerId,
                    crt_reason = EnumInccReason.CancelCard.GetValue(),
                    mac_value = ""
                };



                request.AddJsonBody(JsonConvert.SerializeObject(cancelCard));

                var data = client.Execute(request);

                if (data.StatusCode == HttpStatusCode.OK)
                {
                    var options = new AuditScopeOptions()
                    {
                        EventType = "InccCard:CancelCard",
                        CreationPolicy = EventCreationPolicy.Manual,
                        AuditEvent = new FinstroAuditEvent(userId),
                        TargetGetter = () => card.InccCard
                    };

                    using (var scope = AuditScope.Create(options))
                    {
                        response = JsonConvert.DeserializeObject<GetCardDataResponse>(data.Content);
                        card.InccCard.Status = EnumInccCardStatus.Cancelled.GetDescription();
                        creditApplicationDynamo.Update(application);
                    }
                }
                else
                {
                    var error = FinstroErrorType.INCC.CouldNotGetCard;
                    error.Description = data.Content;
                    throw error;
                }
            }
        }

        public void UpdateCard(CardActionRequest cardData, bool renew)
        {
            string userId = cardData.UserId;
            string cardToken = cardData.CardToken;

            var application = creditApplicationDynamo.GetCreditApplicationForUser(userId);

            if (application.FinstroCards == null)
                throw FinstroErrorType.INCC.NoCardAvailable;


            var card = application.FinstroCards.FirstOrDefault(c => c.InccCard.CardToken == cardToken);

            if (card == null)
                throw FinstroErrorType.INCC.NoCardAvailable;

            card.PostalAddress = cardData.PostalAddress;


            bool updated = UpdateCard(application, card, renew);
        }

        public void ReplaceCard(CardActionRequest cardData)
        {
            string userId = cardData.UserId;
            string cardToken = cardData.CardToken;

            var application = creditApplicationDynamo.GetCreditApplicationForUser(userId);

            if (application.FinstroCards == null)
                throw FinstroErrorType.INCC.NoCardAvailable;


            var card = application.FinstroCards.FirstOrDefault(c => c.InccCard.CardToken == cardToken);

            if (card == null)
                throw FinstroErrorType.INCC.NoCardAvailable;


            //REACTIVATE
            if (card.InccCard.Status != EnumInccCardStatus.Active.GetDescription())
                throw FinstroErrorType.INCC.CardNotActive;

            if (cardData.PostalAddress == null)
                throw FinstroErrorType.Schema.InvalidAddress;

            var options = new AuditScopeOptions()
            {
                EventType = "InccCard:ReplaceCard",
                CreationPolicy = EventCreationPolicy.Manual,
                AuditEvent = new FinstroAuditEvent(userId),
                TargetGetter = () => card.InccCard
            };


            using (var scope = AuditScope.Create(options))
            {

                card.PostalAddress = cardData.PostalAddress;



                bool updated = UpdateCard(application, card);

                FinstroCardDetail newCard = new FinstroCardDetail()
                {
                    PostalAddress = card.PostalAddress,
                    InccCard = new CreateCardResponse()
                    {
                        CardEmbossName = card.InccCard.CardEmbossName,
                        CardMaskedNumber = card.InccCard.CardMaskedNumber,
                        CardToken = card.InccCard.CardToken,
                        ConsumerId = card.InccCard.ConsumerId,
                        ErrorCode = card.InccCard.ErrorCode,
                        ErrorMessage = card.InccCard.ErrorMessage,
                        InccStatusCode = card.InccCard.InccStatusCode,
                        MacValue = card.InccCard.MacValue,
                        OpenToBuy = card.InccCard.OpenToBuy,
                        ProductCode = card.InccCard.ProductCode,
                        Status = card.InccCard.Status,
                        Token24F2 = card.InccCard.Token24F2
                    }
                };

                if (updated)
                {
                    if (application.FinstroInactiveCards == null)
                        application.FinstroInactiveCards = new List<FinstroCardDetail>();

                    application.FinstroInactiveCards.Add(card);

                    application.FinstroCards = new List<FinstroCardDetail>();

                }

                var token30Min = Get30MinToken(userId, cardToken);


                var lockCardRequest = new GetCardDataRequest()
                {
                    Token30min = token30Min.Token30min,
                    CardToken = cardToken,
                    CallType = EnumInccCardAction.PermanentLock.GetDescription()
                };


                LockUnlockCard(userId, lockCardRequest, EnumInccCardAction.PermanentLock);
                card.InccCard.Status = EnumInccCardStatus.PermLocked.GetDescription();
                card.InccCard.InccStatusCode = EnumInccCardStatusCode.PermanentBlock.GetValue();


                var cardDataRequest = new GetCardDataRequest()
                {
                    Token30min = token30Min.Token30min,
                    CardToken = cardToken,
                    CallType = EnumInccCardCallType.MaskedData.GetDescription()
                };

                var oldCardData = GetCardData(userId, cardDataRequest);

                if (oldCardData.Status != EnumInccCardStatusCode.PermanentBlock.GetValue()
                    && oldCardData.Status != EnumInccCardStatusCode.TemporaryBlockLost.GetValue()
                    && oldCardData.Status != EnumInccCardStatusCode.TemporaryBlockStolen.GetValue())
                    throw FinstroErrorType.INCC.CardNotLocked;


                var token = Get24HourToken();

                if (token.CreatedDate > DateTime.UtcNow.AddHours(-24))
                {
                    var request = new RestRequest("api/getcardrepl", Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("crt_card_token", cardToken);

                    dynamic replaceCard = new
                    {
                        corporate_id = AppSettings.InccSettings.CorporateId,
                        institution_id = AppSettings.InccSettings.InstitutionId,
                        crt_securtoken_24_e1 = token.Token24E1,
                        crt_card_token = cardData.CardToken,
                        consumer_id = card.InccCard.ConsumerId,
                        code_product = AppSettings.InccSettings.ProductCode,
                        crt_reason = EnumInccReason.Reason_41.GetValue(),
                        mac_value = ""
                    };



                    request.AddJsonBody(JsonConvert.SerializeObject(replaceCard));

                    var data = client.Execute(request);

                    if (data.StatusCode == HttpStatusCode.OK)
                    {
                        dynamic response = JsonConvert.DeserializeObject(data.Content);
                        newCard.InccCard.CardToken = response.crt_new_token;
                        newCard.InccCard.Status = EnumInccCardStatus.Inactive.GetDescription();
                        newCard.InccCard.InccStatusCode = EnumInccCardStatusCode.Inactive.GetValue();

                        application.FinstroCards.Add(newCard);

                        creditApplicationDynamo.Update(application);



                        token30Min = Get30MinToken(userId, newCard.InccCard.CardToken);
                        cardDataRequest = new GetCardDataRequest()
                        {
                            Token30min = token30Min.Token30min,
                            CardToken = newCard.InccCard.CardToken,
                            CallType = EnumInccCardCallType.MaskedData.GetDescription()
                        };

                        oldCardData = GetCardData(userId, cardDataRequest);

                    }
                    else
                    {
                        var error = FinstroErrorType.INCC.CannotRenewCard;
                        error.Description = data.Content;
                        throw error;
                    }
                }
            }
        }

        public bool CheckIfCardCanBeCancelled(CreditApplication application, FinstroCardDetail card)
        {
            bool result = true;

            if (card.InccCard.OpenToBuy < 0)
                throw FinstroErrorType.INCC.CannotCancelCard;



            return result;

        }


        private async Task SaveAuditAsync(string eventType, AuditEvent auditEvent, object extraFields)
        {

            var options = new AuditScopeOptions()
            {
                EventType = eventType,
                CreationPolicy = EventCreationPolicy.Manual,
                AuditEvent = auditEvent,
                ExtraFields = extraFields,
            };


            AuditScope auditScope = null;
            try
            {
                // async scope creation
                auditScope = await AuditScope.CreateAsync(options);

                await auditScope.SaveAsync();
            }
            finally
            {
                // async disposal
                await auditScope.DisposeAsync();
            }
        }

    }




}
