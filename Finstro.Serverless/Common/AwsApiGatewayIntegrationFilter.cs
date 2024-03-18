using System.Collections.Generic;
using Finstro.Serverless.Helper;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Finstro.Serverless.API.Common
{

    public class AwsApiGatewayIntegrationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext contex)
        {
            operation.Extensions.Add("x-amazon-apigateway-integration", new OpenApiObject
            {
                ["type"] = new OpenApiString("aws_proxy"),
                ["uri"] = new OpenApiString(AppSettings.AwsSettings.LambdaUri), //arn:aws:apigateway:us-east-2:lambda:path/2015-03-31/functions/arn:aws:lambda:us-east-2:404658609106:function:FinstroAdminApiStack-DEV-AspNetCoreFunction-EZD4WZ6AOMOM/invocations"),
                ["httpMethod"] = new OpenApiString(contex.ApiDescription.HttpMethod),
                ["passthroughBehavior"] = new OpenApiString("when_no_match"),
                ["contentHandling"] = new OpenApiString("CONVERT_TO_TEXT"),
            });

            System.Reflection.MethodInfo m1;
            IEnumerable<object> m2;

            contex.ApiDescription.GetAdditionalMetadata(out m1, out m2);

            bool addApiKeyRequest = false;

            foreach (var item in m1.CustomAttributes)
            {
                if (item.AttributeType == typeof(FisntroServiceCallAttribute))
                {
                    addApiKeyRequest = true;
                    break;
                }
            }
            if (addApiKeyRequest)
            {
                var oAuthScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "api_key" }
                };

                operation.Security = new List<OpenApiSecurityRequirement>
                    {
                    new OpenApiSecurityRequirement
                        {
                           [ oAuthScheme] = new List<string>()
                        }
                    };
            }
        }
    }


}

/*


{
  "swagger": "2.0",
  "info": {
    "version": "1.0",
    "title": "FinstroAdminApiStack-DEV"
  },
  "host": "dev.finstropay.us",
  "schemes": [
    "https"
  ],
  "paths": {
    "/": {
      "x-amazon-apigateway-any-method": {
        "responses": {},
        "x-amazon-apigateway-integration": {
          "uri": "arn:aws:apigateway:us-east-2:lambda:path/2015-03-31/functions/arn:aws:lambda:us-east-2:404658609106:function:FinstroAdminApiStack-DEV-AspNetCoreFunction-EZD4WZ6AOMOM/invocations",
          "passthroughBehavior": "when_no_match",
          "httpMethod": "POST",
          "type": "aws_proxy"
        }
      }
    },
    "/{proxy+}": {
      "x-amazon-apigateway-any-method": {
        "responses": {},
        "x-amazon-apigateway-integration": {
          "uri": "arn:aws:apigateway:us-east-2:lambda:path/2015-03-31/functions/arn:aws:lambda:us-east-2:404658609106:function:FinstroAdminApiStack-DEV-AspNetCoreFunction-EZD4WZ6AOMOM/invocations",
          "passthroughBehavior": "when_no_match",
          "httpMethod": "POST",
          "type": "aws_proxy"
        }
      }
    }
  }
}


{
  "swagger": "2.0",
  "info": {
    "version": "1.0",
    "title": "FinstroAdminApiStack-SIT"
  },
  "host": "sit.finstropay.us",
  "schemes": [
    "https"
  ],
  "paths": {
    "/": {
      "x-amazon-apigateway-any-method": {
        "responses": {},
        "x-amazon-apigateway-integration": {
          "uri": "arn:aws:apigateway:us-east-2:lambda:path/2015-03-31/functions/arn:aws:lambda:us-east-2:404658609106:function:FinstroAdminApiStack-SIT-AspNetCoreFunction-1EXQT55F2ZYKM/invocations",
          "passthroughBehavior": "when_no_match",
          "httpMethod": "POST",
          "type": "aws_proxy"
        }
      }
    },
    "/{proxy+}": {
      "x-amazon-apigateway-any-method": {
        "responses": {},
        "x-amazon-apigateway-integration": {
          "uri": "arn:aws:apigateway:us-east-2:lambda:path/2015-03-31/functions/arn:aws:lambda:us-east-2:404658609106:function:FinstroAdminApiStack-SIT-AspNetCoreFunction-1EXQT55F2ZYKM/invocations",
          "passthroughBehavior": "when_no_match",
          "httpMethod": "POST",
          "type": "aws_proxy"
        }
      }
    }
  }
}



     */
