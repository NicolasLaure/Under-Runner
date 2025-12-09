using System.Collections;
using Events;
using Input;
using Managers;
using MapBounds;
using Player.Controllers;
using UnityEngine;

namespace Player
{
    public class PlayerMovementController : PlayerController
    {
        [Header("Events")] 
        [SerializeField] private VoidEventChannelSO onObstaclesStartEvent;
        [SerializeField] private VoidEventChannelSO onMinionsStartEvent;
        
        private IMovementController _actualMovementController;

        public Vector3 CurrentDir => _actualMovementController.GetCurrentDir();

        public override void OnEnable()
        {
            base.OnEnable();
            _actualMovementController ??= GetComponent<PlayerLinearMovementController>();
            _actualMovementController.OnEnable();
            onMinionsStartEvent?.onEvent.AddListener(HandleFreeMovement);
            onObstaclesStartEvent?.onEvent.AddListener(HandleRoadsMovement);
        }

        private void HandleFreeMovement()
        {
            Debug.Log("HERE?");
            SetActualMovementController(GetComponent<PlayerFreeMovementController>());
        }

        private void HandleRoadsMovement()
        {
            SetActualMovementController(GetComponent<PlayerLinearMovementController>());
        }

        private void OnDisable()
        {
            _actualMovementController.OnDisable();
            onMinionsStartEvent?.onEvent.RemoveListener(HandleFreeMovement);
            onObstaclesStartEvent?.onEvent.RemoveListener(HandleRoadsMovement);
        }

        public void OnUpdate()
        {
            _actualMovementController.OnUpdate();
        }

        public void TiltAround()
        {
            _actualMovementController.TiltAround();
        }

        public void ToggleMoveability()
        {
            _actualMovementController.ToggleMoveability();
        }

        public void ToggleMoveability(bool value)
        {
            _actualMovementController.ToggleMoveability(value);
        }

        public void SetActualMovementController(IMovementController newController)
        {
            _actualMovementController.OnDisable();
            _actualMovementController = newController;
            _actualMovementController.OnEnable();
            _actualMovementController.HandleZPosition();
        }
    }
}