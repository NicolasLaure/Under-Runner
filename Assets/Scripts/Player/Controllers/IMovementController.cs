using UnityEngine;

namespace Player.Controllers
{
    public interface IMovementController
    {
        public Vector3 GetCurrentDir();
        public void HandleZPosition();
        public void OnEnable();
        public void OnDisable();
        public void OnUpdate();
        public void HandleEndCinematic();
        public void HandleCinematic();
        public void TiltAround();
        public void ToggleMoveability();
        public void ToggleMoveability(bool value);
    }
}