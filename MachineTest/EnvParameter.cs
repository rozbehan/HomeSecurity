using System;
using System.Collections.Generic;
using System.Linq;

namespace MachineTest
{
    public static class EnvParameter
    {
        public static void SetDefault()
        {
            Environment.SetEnvironmentVariable(MachineDefinition.CurrentState, Default.CurrentState);
            Environment.SetEnvironmentVariable(MachineDefinition.Code, Default.Code);
        }

        public static bool ValidateCode(string code)
        {
            return (code == Environment.GetEnvironmentVariable(MachineDefinition.Code));
        }

        public static string CurrentState()
        {
            return Environment.GetEnvironmentVariable(MachineDefinition.CurrentState);
        }

        public static string Code()
        {
            return Environment.GetEnvironmentVariable(MachineDefinition.Code);
        }

        public static void ChangeCurrentState(string state)
        {
            Environment.SetEnvironmentVariable(MachineDefinition.CurrentState, state);
        }
    }
}