using System;
using System.Collections.Generic;

namespace Finstro.Serverless.Models.Request.CreditApplication
{
    public class SavePhotoRequest
    {
        public List<SavePhotoData> Photos { get; set; }
    }
    public class SavePhotoData
    {

        public EnumIdFileType Type { get; set; }
        public string Base64Image { get; set; }

    }
}

