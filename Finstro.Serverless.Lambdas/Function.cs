using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Finstro.Serverless.Lambdas
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(ILambdaContext context)
        {
            
            
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var result = client.PostAsync("https://sit.finstropay.us/api/Monitoring/heartbeat", null).Result;
                    Console.WriteLine(result.Content);
                    return "OK";
                }
                catch {
                    return "Fail";
                }
                
            }
        }
    }
}
