using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace Aws_Greeting
{
    public class Function
    {
        public APIGatewayProxyResponse FunctionHandler(
            APIGatewayProxyRequest request, 
            ILambdaContext context)
        {
            LogMessage(context, "Processing request started");

            try
            {
                string name = null;

                if (request.QueryStringParameters != null && 
                    request.QueryStringParameters.Any())
                {
                    name = request.QueryStringParameters["name"];
                }

                string responseMessage = string.IsNullOrEmpty(name)
                   ? "Pass a name in the query string for a personalized response."
                   : $"Hello, {name}.";

                LogMessage(context, responseMessage);

                LogMessage(context, "Processing request ended");

                return CreateResponse(responseMessage);
            }

            catch (Exception ex)
            {
                LogMessage(context, string.Format("Processing request failed - {0}", ex.Message));
            }

            LogMessage(context, "Processing request ended");

            return CreateResponse(null);
        }

        APIGatewayProxyResponse CreateResponse(string? result)
        {
            int statusCode = (result != null) ?
                (int)HttpStatusCode.OK :
                (int)HttpStatusCode.InternalServerError;

            APIGatewayProxyResponse response = new APIGatewayProxyResponse
            {
                StatusCode = statusCode,
                Body = result,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "text/html" },
                    { "Access-Control-Allow-Origin", "*" }
                }
            };

            return response;
        }

        private void LogMessage(
            ILambdaContext context, 
            string msg)
        {
            context.Logger.LogLine(
                string.Format("{0}:{1} - {2}",
                    context.AwsRequestId,
                    context.FunctionName,
                    msg));
        }
    }
}
