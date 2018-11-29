using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Amazon;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HomeSecurityApi
{
    public class Functions
    {
        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public Functions()
        {
        }
        /// <summary>
        /// A Lambda function to respond to HTTP Post method from API Gateway from IoT
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The response from the state machine: CurretState and Response</returns>
        public APIGatewayProxyResponse IoTEvent(APIGatewayProxyRequest request, ILambdaContext context)
        {
            // Using AWS CloudWatch to log
            context.Logger.LogLine("IoT Event\n");
            // JsonMachine is the state machine class
            JsonMachine jsonMachine = new JsonMachine(request?.Body);
            if (jsonMachine.Log != "")
            {
                context.Logger.LogLine(jsonMachine.Log);
            }

            // ApiResponse is a class defined in ApiBody.cs, a response data model toward IoT
            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(jsonMachine.ApiResponse), 
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };

            return response;
        }

        private GetParameterResponse GetParameterAsync(GetParameterRequest a)
        {
            throw new NotImplementedException();
        }
    }
}
