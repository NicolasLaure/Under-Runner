using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Input
{
    [CreateAssetMenu(menuName = "Input/InputHandlerSO", fileName = "InputHandler", order = 0)]
    public class InputHandlerSO : ScriptableObject
    {
        public UnityEvent<Vector2> onPlayerMove;
        public UnityEvent onPlayerAttack;
        public UnityEvent onPauseToggle;
        public UnityEvent onPlayerDashStarted;
        public UnityEvent onPlayerDashFinished;
        public UnityEvent onSkipSequence;
        public UnityEvent onNavigation;
        public UnityEvent onConsoleToggle;

        public void HandleNavigation(InputAction.CallbackContext context)
        {
            onNavigation?.Invoke();
        }

        public void HandleMovement(InputAction.CallbackContext context)
        {
            Vector2 dir = context.ReadValue<Vector2>();

            onPlayerMove?.Invoke(dir);
        }

        public void HandleDash(InputAction.CallbackContext context)
        {
            if (context.started)
                onPlayerDashStarted?.Invoke();

            if (context.canceled)
                onPlayerDashFinished?.Invoke();
        }

        public void HandleAttack(InputAction.CallbackContext context)
        {
            if (context.started)
                onPlayerAttack?.Invoke();
        }

        public void HandlePause(InputAction.CallbackContext context)
        {
            if (context.started)
                onPauseToggle?.Invoke();
        }

        public void HandleSkipSequence(InputAction.CallbackContext context)
        {
            if (context.started)
                onSkipSequence?.Invoke();
        }

        public void HandleConsoleToggle(InputAction.CallbackContext context)
        {
            Debug.Log($"CTRL F12");
            if (context.started)
                onConsoleToggle?.Invoke();
        }
    }
}