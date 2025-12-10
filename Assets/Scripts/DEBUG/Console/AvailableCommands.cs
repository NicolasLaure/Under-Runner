using System.Collections.Generic;
using UnityEngine;

namespace DEBUG.Console
{
    [CreateAssetMenu(fileName = "AvailableCommands", menuName = "Debug/Console/AvailableCommands",order = 0)]
    public class AvailableCommands : ScriptableObject
    {
        public List<ConsoleCommand> commands = new List<ConsoleCommand>();
    }
}