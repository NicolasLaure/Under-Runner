using DEBUG.Console;
using UnityEngine;

[CreateAssetMenu(fileName = "SetLevel", menuName = "Debug/Console/Commands/SetLevel", order = 0)]
public class SetLevelCommand : ConsoleCommand
{
    public override bool Execute(string[] args)
    {
        Debug.Log($"Set Level to: {args[0]}");
        return true;
    }
}