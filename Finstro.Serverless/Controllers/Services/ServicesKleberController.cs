
using Finstro.Serverless.API.Models;
using Finstro.Serverless.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Finstro.Serverless.API.Controllers
{
    // [Authorize]
    public class ServicesKleberController : Controller
    {
        [Route("api/Services/Kleber")]
        [HttpPost]
        [FisntroServiceCall]
        public IActionResult GetAddresses(string search)
        {
         
            //Prepare DtRequest XML Writer
            XmlWriterSettings XmlWriterSettings = new XmlWriterSettings();
            XmlWriterSettings.Indent = true;
            XmlWriterSettings.OmitXmlDeclaration = true;
            StringBuilder XmlStringBuilder = new StringBuilder();
            XmlWriter XmlWriter = XmlWriter.Create(XmlStringBuilder, XmlWriterSettings);

            // Create DtRequest Query XML
            XmlWriter.WriteStartElement("DtRequest");
            XmlWriter.WriteAttributeString("Method", "DataTools.Capture.Address.Predictive.AuNzPaf.SearchAddress");
            XmlWriter.WriteAttributeString("AddressLine", search);
            XmlWriter.WriteAttributeString("ResultLimit", "30");
            XmlWriter.WriteAttributeString("DisplayOnlyCountryCode", "AU");
            XmlWriter.WriteAttributeString("RequestKey", AppSettings.Klebber.RequestKey);
            XmlWriter.WriteAttributeString("DepartmentCode", "Finstro");
            XmlWriter.WriteAttributeString("OutputFormat", "json");

            try
            {

                XmlWriter.WriteEndElement();
                XmlWriter.Close();
                string DtRequestXml = XmlStringBuilder.ToString();
                XmlWriter.Dispose();


                //---------------------------------------------------------------------------------------------
                //Send DtRequest to Kleber Server for processing
                DataTools.DtKleberServiceClient KleberServer = new DataTools.DtKleberServiceClient(DataTools.DtKleberServiceClient.EndpointConfiguration.BasicHttpBinding_IDtKleberService);
                //string DtResponseXml = KleberServer.ProcessXmlRequestAsync(DtRequestXml).Result;
                string json = KleberServer.ProcessXmlRequestAsync(DtRequestXml).Result;
                //---------------------------------------------------------------------------------------------
                //XmlSerializer serializer = new XmlSerializer(typeof(AddressListResponse), new XmlRootAttribute("DtResponse"));

                //XmlDocument doc = new XmlDocument();
                //doc.LoadXml(DtResponseXml);
                //string json = JsonConvert.SerializeXmlNode(doc).Replace("@", "");

                AddressListResponse response = JsonConvert.DeserializeObject<AddressListResponse>(json);
                if (response.DtResponse.Result != null)
                    return Ok(response);
                else
                    return NoContent();

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }



        }

        [Route("api/Services/Kleber/{addressid}")]
        [HttpPost]
        [FisntroServiceCall]
        public IActionResult GetAddress(string addressid)
        {

            //Prepare DtRequest XML Writer
            XmlWriterSettings XmlWriterSettings = new XmlWriterSettings();
            XmlWriterSettings.Indent = true;
            XmlWriterSettings.OmitXmlDeclaration = true;
            StringBuilder XmlStringBuilder = new StringBuilder();
            XmlWriter XmlWriter = XmlWriter.Create(XmlStringBuilder, XmlWriterSettings);

            // Create DtRequest Query XML
            XmlWriter.WriteStartElement("DtRequest");
            XmlWriter.WriteAttributeString("Method", "DataTools.Capture.Address.Predictive.AuPaf.RetrieveAddress");
            XmlWriter.WriteAttributeString("RecordId", addressid);
            XmlWriter.WriteAttributeString("RequestKey", AppSettings.Klebber.RequestKey);
            XmlWriter.WriteAttributeString("DepartmentCode", "Finstro");
            XmlWriter.WriteAttributeString("OutputFormat", "json");

            XmlWriter.WriteEndElement();
            XmlWriter.Close();
            string DtRequestXml = XmlStringBuilder.ToString();
            XmlWriter.Dispose();

            //---------------------------------------------------------------------------------------------
            //Send DtRequest to Kleber Server for processing
            DataTools.DtKleberServiceClient KleberServer = new DataTools.DtKleberServiceClient(DataTools.DtKleberServiceClient.EndpointConfiguration.BasicHttpBinding_IDtKleberService);
            //string DtResponseXml = KleberServer.ProcessXmlRequestAsync(DtRequestXml).Result;
            string json = KleberServer.ProcessXmlRequestAsync(DtRequestXml).Result;
            //---------------------------------------------------------------------------------------------


            //XmlSerializer serializer = new XmlSerializer(typeof(AddressResponse), new XmlRootAttribute("DtResponse"));

            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml(DtResponseXml);
            //string json = JsonConvert.SerializeXmlNode(doc).Replace("@", "");

            AddressResponse response = JsonConvert.DeserializeObject<AddressResponse>(json);

            return Ok(response);

        }

        [HttpPost]
        [Route("api/Services/Test/Test2")]
        public ActionResult Get()
        {
            return Ok("CodePipeline with Lambda Working! version 001.");
        }

    }
}
