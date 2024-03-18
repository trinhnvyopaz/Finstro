using System;
using Finstro.Serverless.Dapper.Repository;
using Finstro.Serverless.Models;
using Finstro.Serverless.Models.Entity;
using Finstro.Serverless.Helper;
using System.ComponentModel;
using System.Reflection;
using System.Net.Mail;

namespace FinstroServerless.Services.Email
{


    public class EmailService
    {
       // private EmailTemplateRepository _emailTemplateRepository;

        public EmailService()
        {
            //_emailTemplateRepository = new EmailTemplateRepository();
        }

        public void SendEmail(string toEmail, string toName, string subject, string body, bool isBodyHtml = true)
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


        public string GetAccessCodeEmailTemplate()
        {
            try
            {
                string templateType = EmailTemplateType.FinstroPayEmailAccessCode.GetDescription();


                //var template = _emailTemplateRepository.GetTemplateByType(templateType);
                var template = EmailHelper.GetTemplate(templateType).Result;

                // Get the type of this instance 


                AppSettings.FinstroSettings settings = new AppSettings.FinstroSettings();


                Type type = settings.GetType();

                string fieldName;
                object propertyValue;

                template = template.Replace(@"""", "'");

                // Use each property of the business object passed in 
                foreach (PropertyInfo pi in type.GetProperties())
                {
                    // Get the name and value of the property 
                    fieldName = pi.Name;

                    // Get the value of the property 
                    propertyValue = pi.GetValue(settings, null);

                    string value = propertyValue.ToString();

                    template = template.Replace("@{${" + fieldName + "}}", value);
                }

                return template;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
    }
}
