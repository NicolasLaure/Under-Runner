using DEBUG.Cheats;
using DEBUG.Console;
using UnityEngine;

[CreateAssetMenu(fileName = "InvincibilityCommand", menuName = "Debug/Console/Commands/Invincibility", order = 0)]
public class InvincibilityCommand : ConsoleCommand
{
    public override bool Execute(string[] args)
    {
        CheatsManager cheatsManager = FindObjectOfType<CheatsManager>();
        if (cheatsManager == null)
        {
            Debug.LogError($"Couldn't Find CheatManager");
            return false;
        }

        if (args.Length != 1)
        {
            Debug.LogError($"Invalids arg count, should be exactly one. (1/0; true/false)");
            return false;
        }


        args[0].ToLower();
        Debug.Log($"ToggleGod: {args[0]}");
        switch (args[0])
        {
            case "1":
            case "true":
                Debug.Log($"INVINCIBLE");
                cheatsManager.ToggleInvincibility(true);
                break;

            case "0":
            case "false":
                Debug.Log($"Vulnerable");
                cheatsManager.ToggleInvincibility(false);
                break;
        }

        return true;
    }
}