using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSecurityApi
{
    /// <summary>
    /// At the first oppurtunity the string defnitions must be placed in a json file 
    /// Then everything will be more dynamic without using of compile again
    /// </summary>
    public struct MachineDefinition
    {
        public const string MachineFile = "JsonMachine.json";
        public const string states = "states";
        public const string events = "events";
        public const string actions = "actions";
        public const string transitions = "transitions";

        public const string State = "State";
        public const string Action = "Action";
        public const string Event = "Event";
        public const string From = "From";
        public const string To = "To";
        public const string EntryAction = "EntryAction";
        public const string InsideAction = "InsideAction";
        public const string ExitAction = "ExitAction";
        public const string GuardCodeValidation = "GuardCodeValidation";
        public const string Code = "Code";
        public const string CurrentState = "CurrentState";
        public const string True = "True";
        public const string False = "False";
        public const string Arm = "Arm";
        public const string Disarm = "Disarm";
        public const string Ok = "Ok";
        public const string Error = "Error";
        public const string Http200 = "Http 200";
        public const string Armed = "Armed";
        public const string Disarmed = "Disarmed";
        public const string Time = "Time";
        public const string Connect = "Connect";
        public const string Reset = "Reset";
    }
    /// <summary>
    /// jspon key of Connect, 'Connet'='True', is used for just connecting to the Api and to get the CurrentState
    /// </summary>
    public class ApiConnectBody
    {
        public string Connect { get; } = MachineDefinition.True;
    }
    /// <summary>
    /// jspon key of Reset, 'Reset'='True', is used for rest the state machine 
    /// </summary>
    public class ApiDefaultBody
    {
        public string Default { get; } = MachineDefinition.True;
    }
    /// <summary>
    /// Api requence data model
    /// </summary>
    public class ApiRequestBody
    {
        public string Event { get; set; }
        public string State { get; set; }
        public string Code { get; set; }
    }
    /// <summary>
    /// Api response data model
    /// </summary>
    public class ApiResponseBody
    {
        public string State { get; set; }
        public string Response { get; set; }
    }
}
