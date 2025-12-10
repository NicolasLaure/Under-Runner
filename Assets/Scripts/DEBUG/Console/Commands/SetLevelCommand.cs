using DEBUG.Console;
using LevelManagement;
using UnityEngine;

[CreateAssetMenu(fileName = "SetLevel", menuName = "Debug/Console/Commands/SetLevel", order = 0)]
public class SetLevelCommand : ConsoleCommand
{
    public override bool Execute(string[] args)
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            Debug.LogError($"Not in Game Sequence");
            return false;
        }

        if (args.Length != 1)
        {
            Debug.LogError($"Invalids arg count, should be exactly one. (1/0; true/false)");
            return false;
        }

        if (!levelManager.TrySetLevel(int.Parse(args[0]) - 1))
        {
            Debug.Log($"Failed To Set Level");
            return false;
        }

        Debug.Log($"Set Level to: {int.Parse(args[0])}");
        return true;
    }
}