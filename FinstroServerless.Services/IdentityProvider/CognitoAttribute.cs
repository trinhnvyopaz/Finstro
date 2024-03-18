using System;
using System.Collections.Generic;
using System.Text;

namespace FinstroServerless.Services.IdentityProvider
{
    public class CognitoAttribute
    {
        // This list of default attributes follows the OpenID Connect specification.
        // Source: https://docs.aws.amazon.com/cognito/latest/developerguide/user-pool-settings-attributes.html
        // Some default attributes might be required when registering a user depending on the user pool configuration.

        public static readonly CognitoAttribute Address = new CognitoAttribute("address");
        public static readonly CognitoAttribute BirthDate = new CognitoAttribute("birthdate");
        public static readonly CognitoAttribute Email = new CognitoAttribute("email");
        public static readonly CognitoAttribute EmailVerified = new CognitoAttribute("email_verified");
        public static readonly CognitoAttribute Enabled = new CognitoAttribute("status");
        public static readonly CognitoAttribute FamilyName = new CognitoAttribute("family_name");
        public static readonly CognitoAttribute Gender = new CognitoAttribute("gender");
        public static readonly CognitoAttribute GivenName = new CognitoAttribute("given_name");
        public static readonly CognitoAttribute Locale = new CognitoAttribute("locale");
        public static readonly CognitoAttribute MiddleName = new CognitoAttribute("middle_name");
        public static readonly CognitoAttribute Name = new CognitoAttribute("name");
        public static readonly CognitoAttribute NickName = new CognitoAttribute("nickname");
        public static readonly CognitoAttribute PhoneNumber = new CognitoAttribute("phone_number");
        public static readonly CognitoAttribute PhoneNumberVerified = new CognitoAttribute("phone_number_verified");
        public static readonly CognitoAttribute Picture = new CognitoAttribute("picture");
        public static readonly CognitoAttribute PreferredUsername = new CognitoAttribute("preferred_username");
        public static readonly CognitoAttribute Profile = new CognitoAttribute("profile");
        public static readonly CognitoAttribute Sub = new CognitoAttribute("sub");
        public static readonly CognitoAttribute UpdatedAt = new CognitoAttribute("updated_at");
        public static readonly CognitoAttribute UserName = new CognitoAttribute("username");
        public static readonly CognitoAttribute UserStatus = new CognitoAttribute("cognito:user_status");
        public static readonly CognitoAttribute Website = new CognitoAttribute("website");
        public static readonly CognitoAttribute ZoneInfo = new CognitoAttribute("zoneinfo");
        public static readonly CognitoAttribute TermsAccepted = new CognitoAttribute("custom:terms_accepted");
        public static readonly CognitoAttribute UserSub = new CognitoAttribute("cognito:username");

        //


        // List of attributes filterable through the ListUsers API.
        public static readonly string[] FilterableAttributes = { Email.AttributeName, Enabled.AttributeName, GivenName.AttributeName,
            FamilyName.AttributeName, PhoneNumber.AttributeName, PreferredUsername.AttributeName, Name.AttributeName, Sub.AttributeName, UserName.AttributeName, UserStatus.AttributeName };

        public string AttributeName { get; }

        public CognitoAttribute(string attributeName)
        {
            AttributeName = attributeName;
        }

        public override string ToString()
        {
            return AttributeName;
        }
    }
}
