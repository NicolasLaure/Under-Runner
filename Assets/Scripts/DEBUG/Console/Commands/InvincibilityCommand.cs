using DEBUG.Console;
using UnityEngine;

[CreateAssetMenu(fileName = "InvincibilityCommand", menuName = "Debug/Console/Commands/Invincibility", order = 0)]
public class InvincibilityCommand : ConsoleCommand
{
    public override bool Execute(string[] args)
    {
        Debug.Log($"ToggleGod: {args[0]}");
        return true;
    }
}