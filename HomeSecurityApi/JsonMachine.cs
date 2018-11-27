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
        private JObject objectTrigger = new JObject();
        private JArray arrayMachine = new JArray();

        public JObject MachineObject { get; private set; } = new JObject();
        public string MachineString { get; private set; } = "";
        public string Response { get; private set; }  = "";
        public string Http200 { get; private set; } = "";
        public string Log { get; private set; } = "";
        public bool CodeValid { get; private set; } = false;
        public ApiResponseBody ApiResponse { get; private set; } = new ApiResponseBody();

        public JsonMachine()
        {
            MachineString = File.ReadAllText(@"JsonMachine.json");
            MachineObject = JObject.Parse(MachineString);
        }

        public JsonMachine(string jsonTrigger)
        {
            MachineString = File.ReadAllText(@"JsonMachine.json");
            MachineObject = JObject.Parse(MachineString);

            //objectTrigger = JObject.Parse(@"{'event':'Arm' , 'state':'DISARMED', 'code':'35432556464654'}");
            objectTrigger = JObject.Parse(jsonTrigger);
            if (objectTrigger == null)
            {
                return;
            }

            string[] actions = MachineObject.SelectToken("actions").Select(s1 => (string)s1).ToArray();

            var objectCurrent = (from s2 in MachineObject["transitions"]
                      where (string)s2["event"] == (string)objectTrigger["event"]
                      && (string)s2["from"] == (string)objectTrigger["state"]
                      select s2).ToList();

            //var myActions = new List<string>();
            TransiantAction(Array.FindAll(actions, s4 => s4.Equals((string)objectCurrent[0]["entryAction"])), objectCurrent);
            /// 
            /// As a constraint if the transiant has a code validation action, it must put in entryAction 
            /// because the state transfers after entryAction
            /// 
            if (CodeValid == true)
            {
                ChangeMachineState(objectCurrent);
            }
            TransiantAction(Array.FindAll(actions, s4 => s4.Equals((string)objectCurrent[0]["insideAction"])), objectCurrent);
            TransiantAction(Array.FindAll(actions, s5 => s5.Equals((string)objectCurrent[0]["exitAction"])), objectCurrent);

            ApiResponse.state = SecureParameters.CurrentState();
            ApiResponse.response = (Response == "" && Http200 != "" ? Http200 : Response);
        }

        private void TransiantAction(string[] action, object obj)
        {
            if (action.Length == 1)
            {
                Type thisType = this.GetType();
                MethodInfo theMethod = thisType.GetMethod("Action" + action[0], BindingFlags.NonPublic | BindingFlags.Instance);
                if (theMethod != null)
                {
                    theMethod.Invoke(this, new object[] { obj });
                }
            }
        }

        private void ActionCodeValidation(List<JToken> obj)
        {
            if ((string)obj[0]["guardCodeValidation"] == "true")
            {
                CodeValid = SecureParameters.ValidateCode((string)objectTrigger["code"]);
            }
        }

        private void ActionHttp200(List<JToken> obj)
        {
            Http200 = "HTTP 200";
        }

        private void ActionLog(List<JToken> obj)
        {
            Log = @"{'state':'" + (string)obj[0]["state"] + @"', 
                'event':'" + (string)objectTrigger["event"] + @"', 
                'time':'" + DateTime.Now.ToString() + @"'}";
        }

        private void ActionResponse(List<JToken> obj)
        {
            Response = (string)obj[0]["guardCodeValidation"] == "false" ? "" : CodeValid ? "OK" : "ERROR";
        }

        private void ChangeMachineState(List<JToken> obj)
        {
            if ((string)obj[0]["guardCodeValidation"] == "false" || CodeValid == true)
            {
                SecureParameters.ChangeCurrentState((string)obj[0]["to"]);
            }
        }
    }
}
