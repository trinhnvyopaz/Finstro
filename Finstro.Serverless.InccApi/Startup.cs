using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Finstro.Serverless.InccApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ServiceStack.Licensing.RegisterLicense(@"7592-e1JlZjo3NTkyLE5hbWU6UmljYXJkbyBCcmFuY28sVHlwZTpJbmRpZSxNZXRhOjAsSGFzaDphYjc2YkJXUVhkTndQNkRpaEwwUWVGMkVQR1FyUHZWY0IxcUxkN3hGYzExR0pkdnNUbmZZRU9ZeWFWUWZvN3FCOXFWd201dFQzc0FxQTYreklBeWVhekp5a25kYUV0aFZhTVdDWElEdVp1TFpodTVhTjNCQVJubmp5bGVsQU9jN0xaL3FNRkt4TTVDVXVMVmxaK2V6SFBKSFU5SDdHLzV2cE81RVR2dWVzRUk9LEV4cGlyeToyMDIwLTEwLTAzfQ==");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
