using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Finstro.Serverless.Helper
{
    public static class Extensions
    {
        public static Stream ConvertToBase64(this Stream stream)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            string base64 = Convert.ToBase64String(bytes);
            return new MemoryStream(Encoding.UTF8.GetBytes(base64));
        }

        public static string ConvertToBase64String(this Stream stream)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            return Convert.ToBase64String(bytes);

        }

        public static byte[] ReadFullyBytes(this Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static string GetElementValue(this XmlDocument doc, string fieldName)
        {
            try
            {
                return doc.GetElementsByTagName(fieldName)[0].InnerText;
            }
            catch
            {
                return "";
            }

        }

        public static string GetElementValue(this XmlDocument doc, string fieldName, string attribute)
        {
            try
            {
                return doc.GetElementsByTagName(fieldName)[0].Attributes[attribute].Value;
            }
            catch
            {
                return "";
            }

        }

        public static string GetCharacteristicValue(this XElement xDoc, XNamespace nameSpace, string parent, string child, string field = "value")
        {
            try
            {
                var node = xDoc.Descendants(nameSpace + parent).Where(e => e.Value == child).FirstOrDefault().Parent;
                
                return node.Descendants(nameSpace + field).FirstOrDefault().Value;
            }
            catch
            {
                return "";
            }

        }

    }


}
