using System;
using DEBUG.Console;
using UnityEngine;

[CreateAssetMenu(fileName = "PrevLevel", menuName = "Debug/Console/Commands/PrevLevel", order = 0)]
public class PrevLevelCommand : ConsoleCommand
{
    public override bool Execute(string[] args)
    {
        Debug.Log($"PrevLevel");
        return true;
    }
}