using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Finstro.Serverless.Models
{
    public enum EnumPartnerType
    {
        [Description("Referral Partner")]
        ReferralPartner,
        [Description("Trade Partner")]
        TradePartner
    }


    public enum EnumCreditApplicationStatus
    {
        Incomplete,
        Complete
    }

    public enum EnumCreditAssessmentStatus
    {
        [Description("PASS")]
        Pass,
        [Description("REFER")]
        Refer,
        [Description("DECLINE")]
        Decline,
        [Description("NONE")]
        None
    }

    public enum EnumInccDeliveryMethod
    {
        [Description("01")]
        StandardAustraliaPost,
        [Description("02")]
        Courier,
    }
    public enum EnumInccCardCallType
    {
        [Description("01")]
        MaskedData,
        [Description("02")]
        FullData,
    }

    public enum EnumThreatMetrixStatus
    {
        [Description("FAIL")]
        fail,
        [Description("PASS")]
        pass,
        [Description("REFER")]
        refer,
        [Description("NONE")]
        none,
        [Description("REVIEW")]
        review,
        [Description("REVIEW")]
        challenge,
        [Description("REJECT")]
        reject,


    }

    public enum EnumIDMetrixType
    {
        [Description("IDENTITY")]
        Identity,
        [Description("BUSINESS")]
        Business,
    }

    public enum EnumIdFileType
    {
        IdFront,
        IdBack,
        Face,
        IdMetrix,
        BankStatement,
        OrgId,
        BusinessCreditCheck,
        IndividualCreditCheck,
        ThreatMetrix,
        SupportingDocuments,

    }



    public enum SearchFields
    {
        [Description("b.abn")]
        Abn,
        [Description("ab.company_name")]
        CompanyName,
        [Description("ab.company_legal_name")]
        CompanyLegalName,
        [Description("pe.first_name")]
        FirstName,
        [Description("pe.last_name")]
        LastName,
        [Description("pe.email")]
        Email,
        [Description("pe.mobile")]
        Mobile

    }

    public enum EmailTemplateType
    {
        [Description("CUSTOMER_ACKNOWLEDGEMENT")]
        CustomerAcknowledgement,

        [Description("CUSTOMER_REJECTION")]
        CustomerRejection,

        [Description("EMAIL_VERIFICATION")]
        EmailVerification,

        [Description("EMAIL_VERIFIED_FAIL")]
        EmailVerifiedFail,

        [Description("EMAIL_VERIFIED_SUCCESS")]
        EmailVerifiedSuccess,

        [Description("EN01")]
        En01,

        [Description("EN02")]
        En02,

        [Description("EN03")]
        En03,
        
        [Description("EN04")]
        En04,

        [Description("FINSTRO_PAY_EMAIL_ACCESS_CODE")]
        FinstroPayEmailAccessCode,

        [Description("MAX_DISHONORS_EXCEEDED_CLIENT")]
        MaxDishonorsExceededClient,

        [Description("MAX_DISHONORS_EXCEEDED_CUSTOMER")]
        MaxDishonorsExceededCustomer,

        [Description("MIGRATION_FAILS")]
        MigrationFails,

        [Description("PAYMENT_DISHONOURED_CLIENT")]
        PaymentDishonouredClient,

        [Description("PAYMENT_DISHONOURED_CUSTOMER")]
        PaymentDishonouredCustomer,

        [Description("PAYMENT_REJECTED_CLIENT")]
        PaymentRejectedClient,

        [Description("PAYMENT_REJECTED_CUSTOMER")]
        PaymentRejectedCustomer,

        [Description("PUSH_NOTIFICATION_ONBOARD_DECLINED_CREDIT")]
        PushNotificationOnboardDeclinedCredit,

        [Description("PUSH_NOTIFICATION_ONBOARD_DECLINED_ID")]
        PushNotificationOnboardDeclinedId,

        [Description("PUSH_NOTIFICATION_ONBOARD_PEND_DOCSIGN")]
        PushNotificationOnboardPendDocsign,

        [Description("PUSH_NOTIFICATION_ONBOARD_SUCCESS_WITH_SMALL_TERM_CONFIRM")]
        PushNotificationOnboardSuccessWithSmallTermConfirm,

        [Description("PUSH_NOTIFICATION_ONBOARD_SUCCESS_WITHOUT_SMALL_TERM_CONFIRM")]
        PushNotificationOnboardSuccessWithoutSmallTermConfirm,

        [Description("SILENT_NOTIFICATION")]
        SilentNotification,

    }

    public enum FinstroCognitoUserGroups
    {
        AdminPortalBackOffice,
        AdminPortalSuperUser,
        FinstroAppUser,
    }

    #region INCC

    public enum EnumInccCardStatus
    {
        [Description("ACTIVE")]
        Active,
        [Description("REPLACED")]
        Replaced,
        [Description("CANCELLED")]
        Cancelled,
        [Description("LOCKED")]
        Locked,
        [Description("PERMLOCKED")]
        PermLocked,
        [Description("TEMPLOCKED")]
        TempLocked,
        [Description("CARDCREATED")]
        CardCreated,
        [Description("INACTIVE")]
        Inactive,
    }



    public enum EnumInccCardAction
    {
        [Description("L")]
        TemporaryLockCard,
        [Description("P")]
        PermanentLock,
        [Description("U")]
        UnlockCard
    }
    public enum EnumInccMessageType
    {
        [Description("0100")]
        Type_0100,
        [Description("0200")]
        Type_0200,
        [Description("0420")]
        Type_0420,
        [Description("0421")]
        Type_0421,
    }

    public enum EnumInccResponseMessageType
    {
        [Description("0110")]
        Type_0110,
        [Description("0210")]
        Type_0210,
        [Description("0230")]
        Type_0230,
        [Description("0430")]
        Type_0430

    }
    public enum EnumInccProcessCode
    {
        [Description("00")]
        Code_00,
        [Description("09")]
        Code_09,
        [Description("30")]
        Code_30,
        [Description("31")]
        Code_31,
        [Description("20")]
        Code_20
    }

    public enum EnumInccReason
    {
        [NameValue("CancelCardReplace", "41")]
        [Description("41")]
        Reason_41,
        [Description("04")]
        Reason_04,
        [Description("00")]
        Reason_00,
        [Description("43")]
        Reason_43,
        [NameValue("CancelCard", "05")]
        CancelCard,
    }

    public enum EnumInccPurpose
    {
        Purchase,
        PurchaseWithCashOut,
        ATMBalanceInquiry,
        Refund,
        PurchaseReversal,
        PurchaseReversalWithCashOut,
        RefundReversal,
        Unknown
    }

    public enum EnumInccErrorMessage
    {
        [NameValue("Approved", "000")]
        Approved,
        [NameValue("Declined", "001")]
        Declined,
        [NameValue("Card not Active", "002")]
        CardNotActive,
        [NameValue("Requested amount exceeds limit", "003")]
        RequestedAmountExceedsLimit,
        [NameValue("Unknown Transaction Purpose", "004")]
        UnknownTransactionPurpose,
        [NameValue("Invalid Token 24 hours F1 token", "005")]
        InvalidToken24F1Token,
    }

    public enum EnumInccCardStatusCode
    {

        [NameValue("PermanentBlock", "04")]
        [Description("PERMLOCKED")]
        PermanentBlock,

        [NameValue("TemporaryBlockLost)", "41")]
        [Description("LOCKED")]
        TemporaryBlockLost,

        [NameValue("TemporaryBlockStolen)", "43")]
        [Description("LOCKED")]
        TemporaryBlockStolen,

        [NameValue("Inactive", "10")]
        [Description("INACTIVE")]
        Inactive,

        [NameValue("CardSentForPersonalization", "11")]
        [Description("CARDCREATED")]
        CardSentForPersonalization,

        [NameValue("CardPersonalizationCompleted", "12")]
        [Description("CARDCREATED")]
        CardPersonalizationCompleted,

        [NameValue("CardSentToBranch", "14")]
        [Description("CARDCREATED")]
        CardSentToBranch,

        [NameValue("CardReceivedByBranch", "28")]
        [Description("CARDCREATED")]
        CardReceivedByBranch,

        [NameValue("CardActive", "17")]
        [Description("ACTIVE")]
        CardActive,

        [NameValue("CardActive", "00")]
        [Description("ACTIVE")]
        CardIsActive,

        [NameValue("CardCancelled", "88")]
        [Description("CANCELLED")]
        CardCancelled,

        [NameValue("CardDeleted", "99")]
        [Description("DELETED")]
        CardDeleted,
    }


    #endregion

    public class NameValueAttribute : Attribute
    {
        internal NameValueAttribute(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
        public string Name { get; private set; }
        public string Value { get; private set; }
    }

}
