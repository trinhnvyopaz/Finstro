using Finstro.Serverless.API.Common;
using Finstro.Serverless.Helper;
using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace Finstro.Serverless.API
{
    public class Startup
    {
        public const string AppS3BucketKey = "AppS3Bucket";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        readonly string MyAllowSpecificOrigins = "FinstroPolicy";

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {

            ServiceStack.Licensing.RegisterLicense(AppSettings.AwsSettings.ServiceStackLicense);


            string _audience = AppSettings.Cognito.ClientId;
            string _authority = AppSettings.Cognito.Authority;

            services.AddAuthentication("Bearer")
               .AddJwtBearer(options =>
               {
                   //options.Audience = "5h3f64kdfe2s1qocvvmsf2370k";
                   //options.Authority = "https://cognito-idp.us-east-2.amazonaws.com/us-east-2_UDhHeDytj";
                   //options.Audience = "1roek95128k8kef1p6iq9sdsu";
                   //options.Authority = "https://cognito-idp.us-east-2.amazonaws.com/us-east-2_aNz7VMCpX";
                   //options.Audience = AppSettings.Cognito.ClientId;
                   //options.Authority = AppSettings.Cognito.Authority;
                   options.Audience = _audience;
                   options.Authority = _authority;


               });

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins("*")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                });
            });

            // add our Cognito group authorization requirement, specifying CalendarWriter as the group



            services.AddAuthorization(options =>
            {
                options.AddPolicy(FinstroCustomPolicy.AdminUserPolicy.PolicyName, policy =>
                {
                    policy.Requirements.Add(new CognitoGroupAuthorizationRequirement(FinstroCustomPolicy.AdminUserPolicy.PolicyRoles));

                });
                options.AddPolicy(FinstroCustomPolicy.AdminSuperUserPolicy.PolicyName, policy =>
                {
                    policy.Requirements.Add(new CognitoGroupAuthorizationRequirement(FinstroCustomPolicy.AdminSuperUserPolicy.PolicyRoles));
                });
                options.AddPolicy(FinstroCustomPolicy.FinstroAppPolicy.PolicyName, policy =>
                {
                    policy.Requirements.Add(new CognitoGroupAuthorizationRequirement(FinstroCustomPolicy.FinstroAppPolicy.PolicyRoles));
                });
            });


            // add a singleton of our cognito authorization handler
            services.AddSingleton<IAuthorizationHandler, CognitoGroupAuthorizationHandler>();



            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

            // Add S3 to the ASP.NET Core dependency injection framework.
            services.AddAWSService<Amazon.S3.IAmazonS3>();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = AppSettings.AwsSettings.ApiGatewayName, Version = "v1" });
                c.OperationFilter<AwsApiGatewayIntegrationFilter>();
                c.AddSecurityDefinition("api_key", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Name = "x-api-key",
                    In = ParameterLocation.Header
                });

            });

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            loggerFactory.AddLambdaLogger(Configuration.GetLambdaLoggerOptions());

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./swagger/v1/swagger.json", "Serverless Finstro");
                c.RoutePrefix = string.Empty;

            });


            app.UseMiddleware<CustomExceptionMiddleware>();

            app.UseAuthentication();

            // Make sure you call this before calling app.UseMvc()
            app.UseCors(MyAllowSpecificOrigins);


            app.UseMvc();
        }
    }
}
