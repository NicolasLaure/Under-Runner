using System;
using DEBUG.Console;
using UnityEngine;

[CreateAssetMenu(fileName = "NextLevel", menuName = "Debug/Console/Commands/NextLevel", order = 0)]
public class NextLevelCommand : ConsoleCommand
{
    public override bool Execute(string[] args)
    {
        Debug.Log($"NextLevel");
        return true;
    }
}