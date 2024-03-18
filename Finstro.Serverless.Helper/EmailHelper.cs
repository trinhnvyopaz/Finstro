using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Finstro.Serverless.Helper
{
    public class EmailHelper
    {
        private static readonly RegionEndpoint _region = RegionEndpoint.USEast2;

        public static async Task<bool> NotifyRefundFail(string json)
        {
            try
            {

                var templateType = await GetTemplate("ADMIN_DECLINED_REVERSAL");
                Dictionary<string, string> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                StringBuilder fields = new StringBuilder();

                foreach (var item in list)
                {
                    fields.Append($"<tr><td><b>{item.Key}</b></td><td>{item.Value}</td></tr>");
                }

                string emailTemplate = templateType.Replace("${AttributeValues}", fields.ToString());

                SendEmail(AppSettings.AwsSettings.FinstroOperationsEmail, "", "Reversal Denied", emailTemplate);


                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }


        public static async Task<string> GetTemplate(string name)
        {
            using (AmazonS3Client client = new AmazonS3Client(_region))
            {

                string bucketName = AppSettings.AwsSettings.FinstroEmailTemplateBucketName;

                string keyName = $"{name}.html";


                string responseBody = "";
                try
                {
                    GetObjectRequest request = new GetObjectRequest
                    {
                        BucketName = bucketName,
                        Key = keyName
                    };
                    using (GetObjectResponse response = await client.GetObjectAsync(request))
                    using (Stream responseStream = response.ResponseStream)
                    using (StreamReader reader = new StreamReader(responseStream))
                    {

                        responseBody = reader.ReadToEnd(); // Now you process the response body.
                    }
                }
                catch (AmazonS3Exception e)
                {
                    Console.WriteLine("Error encountered ***. Message:'{0}' when writing an object", e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
                }
                return responseBody;
            }

        }

        private static string GetAccessCodeEmailTemplate(string templateId)
        {
            return "";
        }

        public static void SendEmail(string toEmail, string toName, string subject, string body, bool isBodyHtml = true)
        {

            body = body.Replace(@"${clientFirstName}' />", @"' /> " + toName);

            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress(toEmail, toName));
            msg.From = new MailAddress(AppSettings.AwsSettings.SESEmailFrom, "Finstro");
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = isBodyHtml;

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(AppSettings.SmtpSettings.SmtpUsername, AppSettings.SmtpSettings.SmtpPassword);
            client.Port = Convert.ToInt32(AppSettings.SmtpSettings.SmtpPort); // You can use Port 25 if 587 is blocked (mine is!)
            client.Host = AppSettings.SmtpSettings.SmtpHost;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            try
            {
                client.Send(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
