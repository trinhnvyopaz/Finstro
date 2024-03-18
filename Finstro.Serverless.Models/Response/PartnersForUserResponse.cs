using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Response
{
   
    public class PartnerForUser
    {
        public int PartnerID { get; set; }
        public EnumPartnerType Type { get; set; }
        public Representative Director { get; set; }
    }


}
