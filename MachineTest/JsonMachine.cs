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

namespace MachineTest
{
    internal struct MachineDefinition
    {
        public const string MachineFile = "JsonMachine.json"; 
        public const string State = "State";
        public const string Action = "Action";
        public const string Event = "Event";
        public const string state = "state";
        public const string _event = "event";
        public const string events = "events";
        public const string actions = "actions";
        public const string CurrentState = "CurrentState";
        public const string guardCodeCondition = "guardCodeCondition";
        public const string transitions = "transitions";
        public const string entryAction = "entryAction";
        public const string insideAction = "insideAction";
        public const string exitAction = "exitAction";
        public const string Code = "Code";
        public const string code = "code";
        public const string guardCodeValidation = "guardCodeValidation";
        public const string _true = "True";
        public const string _false = "False";
        public const string OK = "Ok";
        public const string ERROR = "Error";
        public const string to = "to";
        public const string _from = "from";
        public const string time = "time";
        public const string Http200 = "Http 200";
        public const string ARMED = "Armed";
        public const string DISARMED = "Disarmed";
    }

    public class JsonMachine
    {
        private JObject objectTrigger = new JObject();
        private JArray arrayMachine = new JArray();
        private SecureParameter parameter = new SecureParameter();

        public JObject MachineObject { get; private set; } = new JObject();
        public string MachineString { get; private set; } = "";
        public string Response { get; private set; } = "";
        public string Http200 { get; private set; } = "";
        public string Log { get; private set; } = "";
        public bool CodeValid { get; private set; } = false;
        public ApiResponseBody ApiResponse { get; private set; } = new ApiResponseBody();

        public JsonMachine()
        {
            MachineString = File.ReadAllText(MachineDefinition.MachineFile);
            MachineObject = JObject.Parse(MachineString);
        }

        public JsonMachine(string jsonTrigger)
        {
            try
            {
                MachineString = File.ReadAllText(MachineDefinition.MachineFile);
                MachineObject = JObject.Parse(MachineString);

                var Def = (from s2 in MachineObject["Definitions"] select s2).ToList();

                objectTrigger = JObject.Parse(jsonTrigger);
                if (objectTrigger == null)
                {
                    return;
                }

                string[] actions = MachineObject.SelectToken(MachineDefinition.actions).Select(s1 => (string)s1).ToArray();

                var objectCurrent = (from s2 in MachineObject[MachineDefinition.transitions]
                                     where (string)s2[MachineDefinition._event] == (string)objectTrigger[MachineDefinition._event]
                                     && (string)s2[MachineDefinition._from] == (string)objectTrigger[MachineDefinition.state]
                                     select s2).ToList();

                TransiantAction(Array.FindAll(actions, s4 => s4.Equals((string)objectCurrent[0][MachineDefinition.entryAction])), objectCurrent);
                /// 
                /// As a constraint if the transiant has a code validation action, it must put in entryAction 
                /// because the state transfers after entryAction
                /// 
                if (CodeValid == true)
                {
                    ChangeMachineState(objectCurrent);
                }
                TransiantAction(Array.FindAll(actions, s4 => s4.Equals((string)objectCurrent[0][MachineDefinition.insideAction])), objectCurrent);
                TransiantAction(Array.FindAll(actions, s5 => s5.Equals((string)objectCurrent[0][MachineDefinition.exitAction])), objectCurrent);

                ApiResponse.State = EnvParameter.CurrentState();
                ApiResponse.Response = (Response == "" && Http200 != "" ? Http200 : Response);
            }
            catch (Exception ex)
            {
                ApiResponse.State = EnvParameter.CurrentState();
                ApiResponse.Response = "InnerMessage: " + ex.InnerException.Message + "    ,Message: " + ex.Message + "    ,Data: " + ex.Data.ToString();
            }
        }

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

        private void ActionCodeValidation(List<JToken> obj)
        {
            if ((string)obj[0][MachineDefinition.guardCodeValidation] == MachineDefinition._true)
            {
                CodeValid = EnvParameter.ValidateCode((string)objectTrigger[MachineDefinition.code]);
            }
        }

        private void ActionHttp200(List<JToken> obj)
        {
            Http200 = MachineDefinition.Http200;
        }

        private void ActionLog(List<JToken> obj)
        {
            Log = @"{'" + MachineDefinition.state + @"':'" + (string)obj[0][MachineDefinition.state] + @"', 
                '" + MachineDefinition._event + @"':'" + (string)objectTrigger[MachineDefinition._event] + @"', 
                '" + MachineDefinition.time + @"':'" + DateTime.Now.ToString() + @"'}";
        }

        private void ActionResponse(List<JToken> obj)
        {
            Response = (string)obj[0][MachineDefinition.guardCodeValidation] == MachineDefinition._false ? "" : CodeValid ? MachineDefinition.OK : MachineDefinition.ERROR;
        }

        private void ChangeMachineState(List<JToken> obj)
        {
            if ((string)obj[0][MachineDefinition.guardCodeValidation] == MachineDefinition._false || CodeValid == true)
            {
                EnvParameter.ChangeCurrentState((string)obj[0][MachineDefinition.to]);
            }
        }
    }
}
