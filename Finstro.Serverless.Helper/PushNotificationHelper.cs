using Finstro.Serverless.Models.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Finstro.Serverless.Helper
{
    public static class PushNotificationHelper
    {
        public static async Task<bool> SendPushNotification(string[] deviceTokens, string title, string body, object data)
        {
            var messageInformation = new NotificationMessage()
            {
                Notification = new Notification()
                {
                    Title = title,
                    Text = body
                },
                Data = data,
                RegistrationIds = deviceTokens
            };

            string jsonMessage = JsonConvert.SerializeObject(messageInformation);

            HttpResponseMessage result;
            using (var client = new HttpClient())
            {
                result = await client.SendAsync(GetRequest(jsonMessage));
            }
            return true;
        }

        public static async Task<bool> SendSilentNotification(string[] deviceTokens, object data)
        {
            var messageInformation = new NotificationMessage()
            {
                Data = data,
                RegistrationIds = deviceTokens,
                ContentAvailable = true
            };

            string jsonMessage = JsonConvert.SerializeObject(messageInformation);

            HttpResponseMessage result;
            using (var client = new HttpClient())
            {
                result = await client.SendAsync(GetRequest(jsonMessage));
            }
            return true;
        }

        private static HttpRequestMessage GetRequest(string json)
        {
            string ServerKey = AppSettings.Firebase.SecretKey;
            string FireBasePushNotificationsURL = AppSettings.Firebase.PushSendUrl;

            // Create request to Firebase API
            var request = new HttpRequestMessage(HttpMethod.Post, FireBasePushNotificationsURL);
            request.Headers.TryAddWithoutValidation("Authorization", "key=" + ServerKey);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return request;
        }

    }
}
