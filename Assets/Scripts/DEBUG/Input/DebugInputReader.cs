using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DEBUG.Input
{
    public class DebugInputReader : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || ENABLE_CHEATS
        private DebugActions _debugInput;
        public event Action OnOpenCheatsMenu;

        private void Start()
        {
            _debugInput = new DebugActions();
            _debugInput.Enable();

            _debugInput.Cheats.OpenMenu.performed += HandleOpenMenu;
        }

        private void OnDestroy()
        {
            if (_debugInput != null)
                _debugInput.Cheats.OpenMenu.performed -= HandleOpenMenu;
        }

        private void HandleOpenMenu(InputAction.CallbackContext context)
        {
            OnOpenCheatsMenu?.Invoke();
        }
#endif
    }
}