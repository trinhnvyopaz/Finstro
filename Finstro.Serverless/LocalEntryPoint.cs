using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;

namespace Finstro.Serverless.API
{
    /// <summary>
    /// The Main function can be used to run the ASP.NET Core application locally using the Kestrel webserver.
    /// </summary>
    public class LocalEntryPoint
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                //.UseKestrel(options =>
                //{
                //    options.Limits.MaxRequestBodySize = null;
                //})
                .Build();

            


            /*var sw = (ISwaggerProvider)host.Services.GetService(typeof(ISwaggerProvider));

            OpenApiDocument swaggerDoc = sw.GetSwagger("v1", null, "/");


            using (var outputString = new StringWriter())
            {
                var writer = new OpenApiJsonWriter(outputString);
                swaggerDoc.SerializeAsV3(writer);
                string json = outputString.ToString();
                File.WriteAllText("finstro_swagger.json", outputString.ToString());
            }*/

            return host;
        }
    }
}
