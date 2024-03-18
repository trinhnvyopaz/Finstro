using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Request
{

    public class NotificationMessage
    {
        [JsonProperty(PropertyName = "registration_ids")]
        public string[] RegistrationIds { get; set; }

        [JsonProperty(PropertyName = "notification")]
        public Notification Notification { get; set; }

        [JsonProperty(PropertyName = "data")]
        public object Data { get; set; }

        [JsonProperty(PropertyName = "content_available")]
        public bool? ContentAvailable { get; set; }
    }
    public class Notification
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }


        
    }

}
