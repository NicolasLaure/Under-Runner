using System;
using DEBUG.Console;
using LevelManagement;
using UnityEngine;

[CreateAssetMenu(fileName = "NextLevel", menuName = "Debug/Console/Commands/NextLevel", order = 0)]
public class NextLevelCommand : ConsoleCommand
{
    public override bool Execute(string[] args)
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            Debug.LogError($"Not in Game Sequence");
            return false;
        }

        levelManager.NextLevel();
        Debug.Log($"NextLevel");
        return true;
    }
}