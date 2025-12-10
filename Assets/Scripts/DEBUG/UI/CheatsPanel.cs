using DEBUG.Console;
using Events.ScriptableObjects;
using Input;
using UnityEngine;

public class CheatsPanel : MonoBehaviour
{
    [SerializeField] private InputHandlerSO _input;
    [SerializeField] private BoolEventChannelSO panelChannel;
    [SerializeField] private Console console;

    private bool isEnabled = false;

    private void OnEnable()
    {
        isEnabled = false;
        _input.onConsoleToggle.AddListener(TogglePanel);
        Application.logMessageReceived += console.HandleLog;
    }

    private void OnDisable()
    {
        _input.onConsoleToggle.RemoveListener(TogglePanel);
        Application.logMessageReceived -= console.HandleLog;
    }

    private void TogglePanel()
    {
        Debug.Log($"OpenConsole");
        isEnabled = !isEnabled;
        panelChannel.RaiseEvent(isEnabled);
    }
}