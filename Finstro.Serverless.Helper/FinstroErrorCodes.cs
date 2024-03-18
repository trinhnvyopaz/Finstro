using System;
using System.Collections.Generic;
using System.Net;

namespace Finstro.Serverless.Helper
{

    public static class FinstroError
    {
        public static List<FinstroErrorType> GetErrors(FinstroErrorType error)
        {
            List<FinstroErrorType> errors = new List<FinstroErrorType>();
            errors.Add(error);
            return errors;
        }

        public static object ToFinstroError(this Exception ex)
        {
            List<object> errors = new List<object>();
            try
            {
                var error = (FinstroErrorType)ex;

                errors.Add(new
                {
                    error.Code,
                    error.Group,
                    error.Message,
                    error.Description

                });
            }
            catch
            {
                var error = FinstroErrorType.Schema.UnexpectedError;
                error.Description = ex.Message;
                errors.Add(new
                {
                    error.Code,
                    error.Group,
                    error.Message,
                    error.Description

                });
            }
            return errors;

        }

    }



    public class FinstroErrorType : Exception
    {
        public string Group { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }



        public FinstroErrorType(FinstroErrorGroup group, string errorCode, string message, string description)
        {
            Code = errorCode;
            Group = group.ToString();
            Message = message;
            Description = description;

        }

        public FinstroErrorType(FinstroErrorGroup group, string errorCode, string message)
        {
            Code = errorCode;
            Group = group.ToString();
            Message = message;
            Description = message;

        }
        public class User
        {
            public static readonly FinstroErrorType UserExist = new FinstroErrorType(FinstroErrorGroup.User, "USER_001", "Email already in use", "An account with the given email already exists.");
            public static readonly FinstroErrorType UserNotFound = new FinstroErrorType(FinstroErrorGroup.User, "USER_002", "User not found.", "User does not exist or is not activated.");
            public static readonly FinstroErrorType UserAlreadyActivated = new FinstroErrorType(FinstroErrorGroup.User, "USER_003", "User not found.", "User does not exist or is not activated.");
            public static readonly FinstroErrorType UserNotConfirmed = new FinstroErrorType(FinstroErrorGroup.User, "USER_004", "User is not confirmed.", "The user exists but is not confirmed.");
            public static readonly FinstroErrorType UserTermsNotAccepted = new FinstroErrorType(FinstroErrorGroup.User, "USER_005", "You must accept our terms and conditions.", "Please agree to our Privacy Policy and Terms and Conditions.");
            public static readonly FinstroErrorType PhoneExist = new FinstroErrorType(FinstroErrorGroup.User, "USER_006", "Phone number already in use", "An account with the given phone number already exists.");
        }

        public class Schema
        {
            public static readonly FinstroErrorType UnexpectedError = new FinstroErrorType(FinstroErrorGroup.Schema, "SCHEMA_001", "Unexpected error", "Unexpected error");
            public static readonly FinstroErrorType InvalidPhoneNumber = new FinstroErrorType(FinstroErrorGroup.Schema, "SCHEMA_002", "Invalid phone number", "Invalid phone number");
            public static readonly FinstroErrorType InvalidEmail = new FinstroErrorType(FinstroErrorGroup.Schema, "SCHEMA_003", "Invalid email address", "Invalid email address");
            public static readonly FinstroErrorType FileNotFound = new FinstroErrorType(FinstroErrorGroup.Schema, "SCHEMA_004", "File not found.", "The specified file does not exist.");

            public static readonly FinstroErrorType MedicareNumber = new FinstroErrorType(FinstroErrorGroup.Schema, "SCHEMA_005", "Invalid Medicare Number.", "Invalid Medicare Number.");
            public static readonly FinstroErrorType MedicareReference = new FinstroErrorType(FinstroErrorGroup.Schema, "SCHEMA_006", "Invalid Medicare Reference.", "Invalid Medicare Reference.");
            public static readonly FinstroErrorType MedicareColour = new FinstroErrorType(FinstroErrorGroup.Schema, "SCHEMA_007", "Invalid Medicare Colour.", "Invalid Medicare Colour.");
            public static readonly FinstroErrorType MedicareValidTo = new FinstroErrorType(FinstroErrorGroup.Schema, "SCHEMA_008", "Invalid Medicare Expire Date.", "Invalid Medicare Expire Date.");
            public static readonly FinstroErrorType MedicareDOB = new FinstroErrorType(FinstroErrorGroup.Schema, "SCHEMA_009", "Invalid Medicare Date of Birth.", "Invalid Medicare Date of Birth.");
            public static readonly FinstroErrorType MedicareName = new FinstroErrorType(FinstroErrorGroup.Schema, "SCHEMA_010", "Invalid Medicare First or Last name.", "Invalid Medicare First or Last name.");

            public static readonly FinstroErrorType SelectedCreditAmount = new FinstroErrorType(FinstroErrorGroup.Schema, "SCHEMA_011", "Selected credit amount must have value");
            public static readonly FinstroErrorType NotEmpty = new FinstroErrorType(FinstroErrorGroup.Schema, "SCHEMA_012", "{0} must not be blank.");
            public static readonly FinstroErrorType EmailAddress = new FinstroErrorType(FinstroErrorGroup.Schema, "SCHEMA_013", "Email must be a well-formed email address.");
            public static readonly FinstroErrorType Accepted = new FinstroErrorType(FinstroErrorGroup.Schema, "SCHEMA_014", "Accepted must match true");
            public static readonly FinstroErrorType BusinesssType = new FinstroErrorType(FinstroErrorGroup.Schema, "SCHEMA_015", "Business type must have value( one of: COMPANY, SOLE_TRADER, BUSINESS, CORPORATE_TRUSTEE, PARTNERSHIP)");

            public static readonly FinstroErrorType InvalidAddress = new FinstroErrorType(FinstroErrorGroup.Schema, "SCHEMA_016", "Invalid Address.");


            public static FinstroErrorType ErrorNotEmpty(string field)
            {
                var result = new FinstroErrorType(FinstroErrorGroup.Schema, "SCHEMA_012", "{0} must not be blank.");

                result.Message = string.Format(result.Message, field);
                result.Description = string.Format(result.Description, field);

                return result;
            }
        }
        public class Auth
        {
            public static readonly FinstroErrorType InvalidCode = new FinstroErrorType(FinstroErrorGroup.Authentication, "AUTH_001", "Invalid verification.", "Invalid verification code provided, please try again.");
            public static readonly FinstroErrorType InvalidRefreshToken = new FinstroErrorType(FinstroErrorGroup.Authentication, "AUTH_002", "Invalid Refresh Token.", "Please provide an valid Refresh Token.");
        }

        public class CreditApplication
        {
            public static readonly FinstroErrorType AttemptsExceeded = new FinstroErrorType(FinstroErrorGroup.CreditApplication, "CRED_001", "Maximum number of attempts exceeded.", "Maximum number of attempts exceeded.");
            public static readonly FinstroErrorType IdentityCheckFailed = new FinstroErrorType(FinstroErrorGroup.CreditApplication, "CRED_002", "ID Check failed, please try again.", "ID Check failed, please try again.");
            public static readonly FinstroErrorType ThreatCheckFailed = new FinstroErrorType(FinstroErrorGroup.CreditApplication, "CRED_003", "Threat Check failed, please try again.", "Threat failed, please try again.");
            public static readonly FinstroErrorType ThreatCheckCreditFailed = new FinstroErrorType(FinstroErrorGroup.CreditApplication, "CRED_004", "Threat Check failed, please try again.", "Unfortunately, we are not able to approve your transaction at this stage. Please contact Finstro support team at credit@fccapital.com.au for further details.");
            public static readonly FinstroErrorType ApplicationNotFound = new FinstroErrorType(FinstroErrorGroup.CreditApplication, "CRED_005", "Application not found.", "Application not found.");
        }

        public class INCC
        {
            public static readonly FinstroErrorType CouldNotGet24HourToken = new FinstroErrorType(FinstroErrorGroup.Incc, "INCC_001", "Unable to create 24 hour token", "Unable to create 24 hour token");
            public static readonly FinstroErrorType NoCardAvailable = new FinstroErrorType(FinstroErrorGroup.Incc, "INCC_002", "Could not find a card.", "Could not find a card.");
            public static readonly FinstroErrorType CouldNotGetCard = new FinstroErrorType(FinstroErrorGroup.Incc, "INCC_003", "Unable to get card data.", "Unable to get card data.");
            public static readonly FinstroErrorType CouldNotGet30MinToken = new FinstroErrorType(FinstroErrorGroup.Incc, "INCC_004", "Unable to create 30 minutes token.", "Unable to create 30 minutes token.");
            public static readonly FinstroErrorType CouldNotCreateCard = new FinstroErrorType(FinstroErrorGroup.Incc, "INCC_004", "Unable to create new card.", "Unable to create new card.");
            public static readonly FinstroErrorType CardNotActive = new FinstroErrorType(FinstroErrorGroup.Incc, "INCC_005", "Card not Active.", "Card not Active.");
            public static readonly FinstroErrorType CardNotLocked = new FinstroErrorType(FinstroErrorGroup.Incc, "INCC_006", "Card not Locked.", "Card not Locked.");
            public static readonly FinstroErrorType CardPermLocked = new FinstroErrorType(FinstroErrorGroup.Incc, "INCC_007", "Card permanently Locked.", "Card permanently Locked.");
            public static readonly FinstroErrorType CardLocked = new FinstroErrorType(FinstroErrorGroup.Incc, "INCC_008", "Card Locked.", "Card Locked.");


            public static readonly FinstroErrorType UnknownTransactionPurpose = new FinstroErrorType(FinstroErrorGroup.Incc, "INCC_009", "Unknown Transaction Purpose.");
            public static readonly FinstroErrorType AmountExeedLimit = new FinstroErrorType(FinstroErrorGroup.Incc, "INCC_010", "The requested amount exceeds your limit.");
            public static readonly FinstroErrorType InvalidToken24F1 = new FinstroErrorType(FinstroErrorGroup.Incc, "INCC_011", "Invalid Token 24 hours F1 token.");

            public static readonly FinstroErrorType CannotCancelCard = new FinstroErrorType(FinstroErrorGroup.Incc, "INCC_012", "Card cannot be cancelled.");
            public static readonly FinstroErrorType CannotRenewCard = new FinstroErrorType(FinstroErrorGroup.Incc, "INCC_013", "Unable to renew card.");
            public static readonly FinstroErrorType CouldNotUpdateCard = new FinstroErrorType(FinstroErrorGroup.Incc, "INCC_014", "Unable to update card.");


        }

        public class FccServices
        {
            public static readonly FinstroErrorType FccServiceFail = new FinstroErrorType(FinstroErrorGroup.FccServices, "FCCS_001", "Could not comunicate with Fcc Services.", "Could not comunicate with Fcc Services.");
            public static readonly FinstroErrorType InvalidDriverLicense = new FinstroErrorType(FinstroErrorGroup.FccServices, "FCCS_002", "Invalid Driver License.", "Invalid Driver License.");
        }


        public class Transaction
        {
            public static readonly FinstroErrorType FccServiceFail = new FinstroErrorType(FinstroErrorGroup.FccServices, "FCCS_001", "Could not comunicate with Fcc Services.", "Could not comunicate with Fcc Services.");
            public static readonly FinstroErrorType InvalidDriverLicense = new FinstroErrorType(FinstroErrorGroup.FccServices, "FCCS_002", "Invalid Driver License.", "Invalid Driver License.");
        }

    }


    public enum FinstroErrorGroup
    {
        Event,
        Schema,
        Item,
        Authentication,
        Activation,
        User,
        CreditApplication,
        Incc,
        FccServices,
        Transaction
    }
}
