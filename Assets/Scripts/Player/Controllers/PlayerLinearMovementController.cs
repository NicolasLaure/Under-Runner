using System.Collections;
using Events;
using Input;
using UnityEngine;

namespace Player.Controllers
{
    public class PlayerLinearMovementController : PlayerController, IMovementController
    {
        [Header("Configuration SOs")] [SerializeField]
        private RoadPoints roadPoints;

        [SerializeField] private InputHandlerSO inputHandler;
        [SerializeField] private Vector3EventChannelSO onPlayerNewPositionEvent;
        [SerializeField] private Vector3EventChannelSO onPlayerMovementEvent;
        [SerializeField] private VoidEventChannelSO onCinematicStarted;
        [SerializeField] private VoidEventChannelSO onCinematicFinished;

        [Header("Config Values")] 
        [SerializeField] private float velocity;
        [SerializeField] private float zVelocity = 10f;
        [Range(0f, 1f)] [SerializeField] private float movementThreshold;
        [SerializeField] private float zThreshold = 0.1f;

        private bool _isInCinematic = false;
        private bool _isGoingToLeft = false;
        private bool _isGoingToRight = false;
        private bool _canMove = true;
        private bool _isMoving = false;

        private Coroutine _movingCoroutine;
        private Coroutine _zMovementCoroutine;
        private int _roadIndex = 1;

        public Vector3 GetCurrentDir()
        {
            return Vector3.zero;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            
            _isMoving = false;
            _roadIndex = 1;
            _isGoingToLeft = transform.position.x > roadPoints.xPoints[_roadIndex];
            _isGoingToRight = transform.position.x < roadPoints.xPoints[_roadIndex];
            _movingCoroutine = StartCoroutine(HandleMoveBetweenRoads(_roadIndex));
            
            onCinematicStarted?.onEvent.AddListener(HandleCinematic);
            onCinematicFinished?.onEvent.AddListener(HandleEndCinematic);
            inputHandler.onPlayerMove.AddListener(HandleMovement);
        }

        public void HandleZPosition()
        {
            if(_zMovementCoroutine != null) StopCoroutine(_zMovementCoroutine);
            _zMovementCoroutine = StartCoroutine(SetZPosition());
        }

        public IEnumerator SetZPosition()
        {
            float newZPosition = roadPoints.zPosition;
            Vector3 direction =
                (new Vector3(transform.position.x, transform.position.y, newZPosition) - transform.position).normalized;

            while(Mathf.Abs(transform.position.z - newZPosition) > zThreshold)
            {
                transform.position += direction * (zVelocity * Time.deltaTime);
                yield return null;
            }
        }

        public void OnDisable()
        {
            onCinematicStarted?.onEvent.RemoveListener(HandleCinematic);
            onCinematicFinished?.onEvent.RemoveListener(HandleEndCinematic);
            inputHandler.onPlayerMove.RemoveListener(HandleMovement);
        }

        private void HandleMovement(Vector2 movement)
        {
            if (!_canMove || _isInCinematic || _isMoving || IsInvalidMovement(movement)) return;

            _movingCoroutine = StartCoroutine(HandleMoveBetweenRoads(_isGoingToLeft ? _roadIndex - 1 : _roadIndex + 1));
        }

        private IEnumerator HandleMoveBetweenRoads(int roadIndexToUse)
        {
            _isMoving = true;
            _roadIndex = roadIndexToUse;

            float newXPosition = roadPoints.xPoints[_roadIndex];
            Vector3 direction =
                (new Vector3(newXPosition, transform.position.y, transform.position.z) - transform.position).normalized;

            while (!Mathf.Approximately(transform.position.x, newXPosition))
            {
                Vector3 previousPosition = transform.position;
                transform.position += direction * (velocity * Time.deltaTime);

                if (_isGoingToLeft && transform.position.x < newXPosition ||
                    _isGoingToRight && transform.position.x > newXPosition)
                {
                    Vector3 vector3 = transform.position;
                    vector3.x = newXPosition;
                    transform.position = vector3;
                }

                onPlayerNewPositionEvent?.RaiseEvent(transform.position);
                onPlayerMovementEvent?.RaiseEvent(transform.position - previousPosition);
                yield return null;
            }

            _isGoingToLeft = false;
            _isGoingToRight = false;
            _isMoving = false;
        }

        private bool IsInvalidMovement(Vector2 movement)
        {
            _isGoingToLeft = movement.x <= -movementThreshold;
            _isGoingToRight = movement.x >= movementThreshold;

            return (!_isGoingToLeft && !_isGoingToRight) || (_isGoingToLeft && _roadIndex == 0) ||
                   (_isGoingToRight && _roadIndex == roadPoints.xPoints.Length - 1);
        }

        public void OnUpdate()
        {
        }

        public void HandleEndCinematic()
        {
            _isInCinematic = false;
        }

        public void HandleCinematic()
        {
            _isInCinematic = true;
        }

        public void TiltAround()
        {
            animatorHandler.SetPlayerDirection(!_isMoving ? Vector3.zero :
                _isGoingToLeft ? -transform.right : transform.right);
        }

        public void ToggleMoveability()
        {
            _canMove = !_canMove;
        }

        public void ToggleMoveability(bool value)
        {
            _canMove = value;
        }
    }
}