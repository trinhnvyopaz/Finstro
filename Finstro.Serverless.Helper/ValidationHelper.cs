using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using PhoneNumbers;
using ServiceStack;

namespace Finstro.Serverless.Helper
{
    public class ValidationHelper
    {
        public static string FormatPhoneNumber(string phoneNumber, string countrCode = "AU")
        {
            try
            {
                var phoneUtil = PhoneNumberUtil.GetInstance();

                var number = phoneUtil.Parse(phoneNumber, countrCode);

                if (!phoneUtil.IsValidNumber(number))
                    throw FinstroErrorType.Schema.InvalidPhoneNumber;

                var possible = phoneUtil.IsPossibleNumberWithReason(number);

                var formattedPhone = phoneUtil.Format(number, PhoneNumberFormat.E164);

                return formattedPhone.Trim();
            }
            catch
            {
                throw FinstroErrorType.Schema.InvalidPhoneNumber;
            }
        }

        public static string FormatPhoneNumberNational(string phoneNumber, string countrCode = "AU")
        {
            try
            {
                var phoneUtil = PhoneNumberUtil.GetInstance();

                var number = phoneUtil.Parse(phoneNumber, countrCode);

                if (!phoneUtil.IsValidNumber(number))
                    throw FinstroErrorType.Schema.InvalidPhoneNumber;

                var possible = phoneUtil.IsPossibleNumberWithReason(number);

                var formattedPhone = phoneUtil.Format(number, PhoneNumberFormat.NATIONAL);

                return formattedPhone.Trim();
            }
            catch
            {
                throw FinstroErrorType.Schema.InvalidPhoneNumber;
            }
        }


        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }


        public static void CheckRequired(object myObject)
        {
            foreach (PropertyInfo pi in myObject.GetType().GetProperties())
            {
                bool isRequired = pi.AllAttributes().Where(a => a.ToString().Contains("Required")).Count() > 0;
                object value = pi.GetValue(myObject);
                if (value == null && isRequired)
                    throw FinstroErrorType.Schema.ErrorNotEmpty(pi.Name);
                else
                {
                    if (pi.PropertyType == typeof(string))
                    {
                        string sValue = (string)pi.GetValue(myObject);
                        if (string.IsNullOrEmpty(sValue) && isRequired)
                        {
                            throw FinstroErrorType.Schema.ErrorNotEmpty(pi.Name);
                        }
                    }
                        
                }
            }

        }
    }
}
