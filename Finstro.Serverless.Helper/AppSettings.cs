using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Finstro.Serverless.Helper
{
    public sealed class AppSettings
    {

        private static IConfigurationRoot _configuration;

        public static IConfigurationRoot Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
                }
                return _configuration;
            }
        }

        public class Klebber
        {
            public static string RequestKey { get { return Configuration["AppSettings:Kleber:RequestKey"]; } }

        }
        public class Cognito
        {
            public static string PoolId { get { return Configuration["AppSettings:Cognito:PoolId"]; } }
            public static string Authority { get { return Configuration["AppSettings:Cognito:Authority"]; } }
            public static string ClientId { get { return Configuration["AppSettings:Cognito:ClientId"]; } }

        }

        public class DataBase
        {
            public static string ConnectionString { get { return Configuration["AppSettings:DataBase:ConnectionString"]; } }

        }

        public class AwsSettings
        {
            public static string LambdaUri { get { return Configuration["AppSettings:AwsSettings:LambdaUri"]; } }
            public static string ApiGatewayName { get { return Configuration["AppSettings:AwsSettings:ApiGatewayName"]; } }
            public static string SESEmailFrom { get { return Configuration["AppSettings:AwsSettings:SESEmailFrom"]; } }
            public static string DynamoUrl { get { return Configuration["AppSettings:AwsSettings:DynamoUrl"]; } }
            public static string FinstroBucketName { get { return Configuration["AppSettings:AwsSettings:FinstroBucketName"]; } }
            public static string FinstroEmailTemplateBucketName { get { return Configuration["AppSettings:AwsSettings:FinstroEmailTemplateBucketName"]; } }
            public static string ServiceStackLicense { get { return Configuration["AppSettings:AwsSettings:ServiceStackLicense"]; } }
            public static string FinstroOperationsEmail { get { return Configuration["AppSettings:AwsSettings:FinstroOperationsEmail"]; } }

        }

        public class Equifax
        {
            public static string Username { get { return Configuration["AppSettings:Equifax:Username"]; } }
            public static string Password { get { return Configuration["AppSettings:Equifax:Password"]; } }
            public static string Url { get { return Configuration["AppSettings:Equifax:Url"]; } }
            public static string FraudCheckSubscriberId { get { return Configuration["AppSettings:Equifax:FraudCheckSubscriberId"]; } }
            public static string FraudCheckSecurity { get { return Configuration["AppSettings:Equifax:FraudCheckSecurity"]; } }
            public static string FraudCheckMode { get { return Configuration["AppSettings:Equifax:FraudCheckMode"]; } }
            public static string idMatrixMaxCount { get { return Configuration["AppSettings:Equifax:idMatrixMaxCount"]; } }
        }

        public class FinstroSettings
        {
            public static string FaqUrl { get { return Configuration["AppSettings:FinstroSettings:FaqUrl"]; } }
            public static string PolicyUrl { get { return Configuration["AppSettings:FinstroSettings:PolicyUrl"]; } }
            public static string TermsUrl { get { return Configuration["AppSettings:FinstroSettings:TermsUrl"]; } }
            public static string LegalUrl { get { return Configuration["AppSettings:FinstroSettings:LegalUrl"]; } }
            public static string RequestNewAccessCodeUrl { get { return Configuration["AppSettings:FinstroSettings:RequestNewAccessCodeUrl"]; } }
            public static string LockOutUrl { get { return Configuration["AppSettings:FinstroSettings:LockOutUrl"]; } }
            public static string MerchantSuiteKey { get { return Configuration["AppSettings:FinstroSettings:MerchantSuiteKey"]; } }
            public static decimal AutoApproveCreditAmount { get { return Convert.ToDecimal(Configuration["AppSettings:FinstroSettings:AutoApproveCreditAmount"]); } }
            public static string BankStatementPrefix { get { return Configuration["AppSettings:FinstroSettings:BankStatementPrefix"]; } }
            public static string ProductNameFinstroPay { get { return Configuration["AppSettings:FinstroSettings:ProductNameFinstroPay"]; } }

        }

        public class SmtpSettings
        {
            public static string SmtpHost { get { return Configuration["AppSettings:SmtpSettings:SmtpHost"]; } }
            public static string SmtpPort { get { return Configuration["AppSettings:SmtpSettings:SmtpPort"]; } }
            public static string SmtpUsername { get { return Configuration["AppSettings:SmtpSettings:SmtpUsername"]; } }
            public static string SmtpPassword { get { return Configuration["AppSettings:SmtpSettings:SmtpPassword"]; } }

        }

        public class InccSettings
        {
            public static string CorporateId { get { return Configuration["AppSettings:INCC:CorporateId"]; } }
            public static string CxPassword { get { return Configuration["AppSettings:INCC:CxPassword"]; } }
            public static string CxReason { get { return Configuration["AppSettings:INCC:CxReason"]; } }
            public static string CxUser { get { return Configuration["AppSettings:INCC:CxUser"]; } }
            public static string InstitutionId { get { return Configuration["AppSettings:INCC:InstitutionId"]; } }
            public static string URL { get { return Configuration["AppSettings:INCC:URL"]; } }
            public static string CountryCode { get { return Configuration["AppSettings:INCC:CountryCode"]; } }
            public static int EmbossMaxSize { get { return Convert.ToInt32(Configuration["AppSettings:INCC:EmbossMaxSize"]); } }
            public static string ProductCode { get { return Configuration["AppSettings:INCC:ProductCode"]; } }
        }

        public class FccServices
        {
            public static string URL { get { return Configuration["AppSettings:FccServices:URL"]; } }
        }
        public class Firebase
        {
            public static string PushSendUrl { get { return Configuration["AppSettings:Firebase:PushSendUrl"]; } }
            public static string SecretKey { get { return Configuration["AppSettings:Firebase:SecretKey"]; } }
        }

        public static string iOSAppToken { get { return Configuration["AppSettings:iOSAppToken"]; } }

    }
}
