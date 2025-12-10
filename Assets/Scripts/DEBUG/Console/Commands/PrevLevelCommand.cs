using System;
using DEBUG.Console;
using LevelManagement;
using UnityEngine;

[CreateAssetMenu(fileName = "PrevLevel", menuName = "Debug/Console/Commands/PrevLevel", order = 0)]
public class PrevLevelCommand : ConsoleCommand
{
    public override bool Execute(string[] args)
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            Debug.LogError($"Not in Game Sequence");
            return false;
        }

        levelManager.PreviousLevel();
        Debug.Log($"PrevLevel");
        return true;
    }
}