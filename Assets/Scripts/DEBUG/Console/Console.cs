using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DEBUG.Console
{
    public class Console : MonoBehaviour
    {
        [SerializeField] private string _prefix = String.Empty;
        [SerializeField] private AvailableCommands _availableCommands;

        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI logsText;
        [SerializeField] private ScrollRect scrollRect;

        public void HandleLog(string logString, string stackTrace, LogType type)
        {
            string receivedLog = "<color=";

            switch (type)
            {
                case LogType.Error:
                case LogType.Exception:
                    receivedLog += "red>";
                    break;
                case LogType.Warning:
                    receivedLog += "yellow>";
                    break;
                case LogType.Log:
                    receivedLog += "white>";
                    break;
            }

            receivedLog += logString;
            receivedLog += "</color>\n";
            logsText.text += receivedLog;
            LayoutRebuilder.ForceRebuildLayoutImmediate(logsText.rectTransform);
            scrollRect.verticalNormalizedPosition = 0f;
        }

        public void ProcessCommand(string input)
        {
            Debug.Log(input);
            inputField.text = string.Empty;
            inputField.ActivateInputField();

            if (!input.StartsWith(_prefix))
                return;

            input = input.Remove(0, _prefix.Length);

            string[] inputWords = input.Split(' ');
            string commandInput = inputWords[0];
            List<string> argsList = new List<string>();
            argsList.AddRange(inputWords[1..]);
            string[] args = argsList.ToArray();

            foreach (IConsoleCommand command in _availableCommands.commands)
            {
                if (commandInput.Equals(command.CommandWord, StringComparison.OrdinalIgnoreCase) && command.Execute(args))
                {
                    return;
                }
            }
        }
    }
}