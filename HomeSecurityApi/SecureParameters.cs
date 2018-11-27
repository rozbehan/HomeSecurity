using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Amazon;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Amazon.SimpleSystemsManagement.Model.Internal.MarshallTransformations;
//using Amazon.Runtime;
//using Amazon.Runtime.Internal;
//using Amazon.Runtime.Internal.Auth;
//using Amazon.Runtime.Internal.Transform;
//using Amazon.SimpleSystemsManagement.Internal;

namespace HomeSecurityApi
{
    public class SecureParameters
    {
        public SecureParameters()
        {

        }

        public static bool ValidateCode(string code)
        {
            var a = GetSecureParameter("Code");
            //if (a != code)
            //{
            //    return false;
            //}
            return true;
        }

        public static string CurrentState()
        {
            var a = GetSecureParameter("CurrentState");
            if (a == null)
            {
                a = PutSecureParameter("CurrentState", "DISARMED");
            }
            return "DISARMED";
        }

        public static string Code()
        {
            var a = GetSecureParameter("Code");
            if (a == null)
            {
                a = PutSecureParameter("CurrentState", "1111");
            }
            return "1111";
        }

        public static void ChangeCurrentState(string state)
        {         
            var a = PutSecureParameter("CurrentState", state);
        }

        private static async Task<string> GetSecureParameter(string parameterName)
        {
            var client = new AmazonSimpleSystemsManagementClient(RegionEndpoint.APSoutheast2);
            var request = new GetParametersRequest
            {
                Names = new List<string> { parameterName }
            };
            var response = await client.GetParametersAsync(request);  //result
            var parameterValue = response.Parameters.SingleOrDefault();

            return (parameterValue == null ? null : parameterValue.Value.ToString());
        }

        private static async Task<string> PutSecureParameter(string parameterName, string value)
        {
            var client = new AmazonSimpleSystemsManagementClient(RegionEndpoint.APSoutheast2);
            var request = new PutParameterRequest
            {
                Name = parameterName,
                Overwrite = true,
                Value = value,
                Type = ParameterType.SecureString
            };
            var response = await client.PutParameterAsync(request);
            return (response == null ? null : response.ToString());
        }
    }

}
