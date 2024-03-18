using System;
using System.Collections.Generic;
using Finstro.Serverless.Models.Dynamo;

namespace Finstro.Serverless.Models.Request.CreditApplication
{
    public class SaveBusinessDetailRequest
    {
        public AsicBusiness AsicBusiness { get; set; }
        public Address BusinessTradingAddress { get; set; }
        public List<Contact> Contacts { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Website { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Skype { get; set; }
        public string Linkedin { get; set; }
        public string Other { get; set; }

        public DateTime? IncorporationDate { get; set; }
        public DateTime? GstDate { get; set; }
        public DateTime? TimeTrading { get; set; }
    }
}
