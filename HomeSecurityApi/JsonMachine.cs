using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;
using System.Net;
using System.Web;

namespace HomeSecurityApi
{
    public class JsonMachine
    {
        /// <summary>
        /// private:
        /// objectTrigger is a json object form of the IoT event.
        /// SecureParameter is the calss for storing and access CurrentState and Code securly.
        /// </summary>
        private JObject objectTrigger = new JObject();
        private SecureParameter parameter = new SecureParameter();

        /// <summary>
        /// public:
        /// json object and string of the state machine.
        /// Response, Http200, and Log strings are prepared for the any of the state machine entry, exit or inside actions.
        /// CodeValid is the current validation of the Code 
        /// ApiBody.cs contains the data models for the ApiRequestBody and ApiResponseBody, ... and the string definitions 
        /// </summary>
        public JObject MachineObject { get; private set; } = new JObject();
        public string MachineString { get; private set; } = "";
        public string Response { get; private set; } = "";
        public string Http200 { get; private set; } = "";
        public string Log { get; private set; } = "";
        public bool CodeValid { get; private set; } = false;
        public ApiResponseBody ApiResponse { get; private set; } = new ApiResponseBody();

        /// <summary>
        /// jsonTrigger is a json string, IoT event request, the message body of lambda function
        /// </summary>
        /// <param name="jsonTrigger"></param>
        public JsonMachine(string jsonTrigger)
        {
            /// <summary>
            /// CurrentClass and Code are stored in three ways: AWS Parameter Storing, file, and as AWS Lambda Function Environment Variables
            /// MachineDefinition is just a static class of global strings, in ApiBody.cs
            /// </summary>
            ApiResponse.State = EnvParameter.CurrentState();
            ApiResponse.Response = MachineDefinition.Http200;
            try
            {
                objectTrigger = JObject.Parse(jsonTrigger);
                if (objectTrigger == null)
                {
                    return;
                }
                // Reset is needed in development stage, CurrentState=Disarmed and Code=1111 
                if ((string)objectTrigger[MachineDefinition.Reset] == MachineDefinition.True)
                {
                    EnvParameter.SetDefault();
                    ApiResponse.State = EnvParameter.CurrentState();
                    return;
                }
                // Connect is just for read the state of the machine before request
                if ((string)objectTrigger[MachineDefinition.Connect] == MachineDefinition.True)
                {
                    return;
                }
                
                MachineString = File.ReadAllText(MachineDefinition.MachineFile);
                MachineObject = JObject.Parse(MachineString);

                string[] actions = MachineObject.SelectToken(MachineDefinition.actions).Select(s1 => (string)s1).ToArray();
                // Search the state machine for the proper transition, the same with trigger 
                var objectCurrent = (from s2 in MachineObject[MachineDefinition.transitions]
                                     where (string)s2[MachineDefinition.Event] == (string)objectTrigger[MachineDefinition.Event]
                                     && (string)s2[MachineDefinition.From] == (string)objectTrigger[MachineDefinition.State]
                                     select s2).ToList();

                // What action must be run(or initialized) first? the action the same as entryAction in the transitition definition.
                TransiantAction(Array.FindAll(actions, s4 => s4.Equals((string)objectCurrent[0][MachineDefinition.EntryAction])), objectCurrent);                
                /// As a constraint if the transiant has a code validation action, it must be put in entryAction 
                /// because the state transfers after entryAction                
                if (CodeValid == true)
                {
                    ChangeMachineState(objectCurrent);
                }
                // and the second action is the insideAction, finally the last one is exitAction                
                TransiantAction(Array.FindAll(actions, s4 => s4.Equals((string)objectCurrent[0][MachineDefinition.InsideAction])), objectCurrent);
                TransiantAction(Array.FindAll(actions, s5 => s5.Equals((string)objectCurrent[0][MachineDefinition.ExitAction])), objectCurrent);
                // Init the Api response, current state plus Ok/Error/Http 200. (all the strings and names are defined in MachineDefinition class)
                ApiResponse.State = EnvParameter.CurrentState();
                ApiResponse.Response = (Response == "" && Http200 != "" ? Http200 : Response);
            }
            catch (Exception ex)
            {
                ApiResponse.State = EnvParameter.CurrentState();
                ApiResponse.Response = "InnerMessage: " + ex.InnerException.Message + " ;Message: " + ex.Message;
            }
        }
        /// <summary>
        /// Invoking the action by its name. Action name is Action+{CodeValidation, Response, Http200, Log , ...}
        /// </summary>
        /// <param name="action"></param>
        /// <param name="obj"></param>
        private void TransiantAction(string[] action, object obj)
        {
            if (action.Length == 1)
            {
                Type thisType = this.GetType();
                MethodInfo theMethod = thisType.GetMethod(MachineDefinition.Action + action[0], BindingFlags.NonPublic | BindingFlags.Instance);
                if (theMethod != null)
                {
                    theMethod.Invoke(this, new object[] { obj });
                }
            }
        }
        /// <summary>
        /// obj is the current transition        
        /// </summary>
        /// <param name="obj"></param>
        private void ActionCodeValidation(List<JToken> obj)
        {
            if ((string)obj[0][MachineDefinition.GuardCodeValidation] == MachineDefinition.True)
            {
                CodeValid = EnvParameter.ValidateCode((string)objectTrigger[MachineDefinition.Code]);
            }
        }

        private void ActionHttp200(List<JToken> obj)
        {
            Http200 = MachineDefinition.Http200;
        }
        /// <summary>
        /// We use AWS CloudWatch for logging, the log function is run after returning to lambda function body
        /// if the Log is not empty then is logged
        /// </summary>
        /// <param name="obj"></param>
        private void ActionLog(List<JToken> obj)
        {
            Log = @"{'" + MachineDefinition.State + @"':'" + (string)obj[0][MachineDefinition.State] + @"', 
                '" + MachineDefinition.Event + @"':'" + (string)objectTrigger[MachineDefinition.Event] + @"', 
                '" + MachineDefinition.Time + @"':'" + DateTime.Now.ToString() + @"'}";
        }
        /// <summary>
        /// If in the definition of transition the guard condition of Code is False, then the response(Ok and Error) is not passed
        /// </summary>
        /// <param name="obj"></param>
        private void ActionResponse(List<JToken> obj)
        {
            Response = (string)obj[0][MachineDefinition.GuardCodeValidation] == MachineDefinition.False ? "" : CodeValid ? MachineDefinition.Ok : MachineDefinition.Error;
        }
        /// <summary>
        /// The state moves from 'From' to 'To' if:
        /// 1. Code is valid, or
        /// 2. In the definition of transition, the guard condition of the Code validation is False
        /// Of course that the seonnd one is not usually considered in a secure system       
        /// </summary>
        /// <param name="obj"></param>
        private void ChangeMachineState(List<JToken> obj)
        {
            if ((string)obj[0][MachineDefinition.GuardCodeValidation] == MachineDefinition.False || CodeValid == true)
            {
                EnvParameter.ChangeCurrentState((string)obj[0][MachineDefinition.To]);
            }
        }
    }
}
