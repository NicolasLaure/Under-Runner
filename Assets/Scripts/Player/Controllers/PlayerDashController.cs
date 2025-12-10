using System.Collections;
using Events;
using Health;
using Input;
using Managers;
using MapBounds;
using Player.Controllers;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    public class PlayerDashController : PlayerController
    {
        [SerializeField] private PauseSO pauseData;

        [Header("Input")]
        [SerializeField] private InputHandlerSO inputHandler;

        [Header("MapBounds")]
        [SerializeField] private MapBoundsSO boundsConfig;

        [Header("Dash Configuration")]
        [SerializeField] private float dashSpeed;

        [SerializeField] private float dashDuration;
        [SerializeField] private float dashCoolDown;
        [SerializeField] private float phantomDuration;
        [SerializeField] private AnimationCurve speedCurve;

        [Header("Bullet Time Dash")]
        [SerializeField] private DashPredictionLine dashPredictionLine;

        [SerializeField] private AnimationCurve bulletTimeVariationCurve;
        [SerializeField] private float bulletTimeDuration;

        [Header("Events")]
        [SerializeField] private FloatEventChannelSO onDashRechargeEvent;
        [SerializeField] private VoidEventChannelSO onDashRechargedEvent;
        [SerializeField] private VoidEventChannelSO onDashUsedEvent;
        [SerializeField] private Vector3EventChannelSO onDashMovementEvent;
        [SerializeField] private VoidEventChannelSO onDamageAvoidedEvent;
        [SerializeField] private VoidEventChannelSO onCinematicFinished;
        [SerializeField] private VoidEventChannelSO onCinematicStarted;

        [Header("Internal events")]
        [SerializeField] private UnityEvent onDash;

        private PlayerMovementController _movementController;
        private HealthPoints _healthPoints;

        private bool _canDash = true;
        private Coroutine _dashCoroutine = null;
        private Coroutine _bulletTimeCoroutine = null;

        private Vector3 _dashDir;
        private float _currentDashSpeed;

        private Bounds _playerColliderBounds;
        private bool _hasAvoidedSomething;
        private bool _hasActivatedFastCooldown;
        private bool _isPlayerInCinematic;
        private Coroutine _cooldownCoroutine;

        private void Awake()
        {
            _movementController = GetComponent<PlayerMovementController>();
            _healthPoints ??= GetComponent<HealthPoints>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _hasAvoidedSomething = false;
            _playerColliderBounds = playerCollider.bounds;

            onCinematicStarted?.onEvent.AddListener(HandlePlayerInCinematic);
            onCinematicFinished?.onEvent.AddListener(HandlePlayerOutOfCinematic);
            onDamageAvoidedEvent?.onEvent.AddListener(HandleDamageAvoided);
            inputHandler.onPlayerDashStarted.AddListener(HandleDash);
        }

        private void OnDisable()
        {
            onCinematicStarted?.onEvent.RemoveListener(HandlePlayerInCinematic);
            onCinematicFinished?.onEvent.RemoveListener(HandlePlayerOutOfCinematic);
            onDamageAvoidedEvent?.onEvent.AddListener(HandleDamageAvoided);
            inputHandler.onPlayerDashStarted.RemoveListener(HandleDash);
        }

        private void HandlePlayerInCinematic()
        {
            _isPlayerInCinematic = true;
        }

        private void HandlePlayerOutOfCinematic()
        {
            _isPlayerInCinematic = false;
        }

        private void HandleDamageAvoided()
        {
            _hasAvoidedSomething = true;
        }

        private bool CanDash()
        {
            if (_movementController.CurrentDir == Vector3.zero)
                return false;

            return _canDash;
        }

        private void HandleDash()
        {
            if (!CanDash() || _isPlayerInCinematic || pauseData.isPaused)
                return;

            if (_dashCoroutine != null)
                StopCoroutine(_dashCoroutine);

            _healthPoints.SetIsInvincible(true);
            _dashCoroutine = StartCoroutine(DashCoroutine());
        }

        public void HandlePhantomCoroutine()
        {
            if (!_healthPoints.CanTakeDamage)
                return;

            StartCoroutine(PhantomCoroutine());
        }

        private IEnumerator DashCoroutine()
        {
            float startTime = Time.time;
            float timer = 0;
            _canDash = false;

            _dashDir = _movementController.CurrentDir;
            playerAgent.ChangeStateToDash();
            onDashUsedEvent.RaiseEvent();
            onDash?.Invoke();
            while (timer < dashDuration)
            {
                float dashTime = Mathf.Lerp(0, 1, timer / dashDuration);

                _currentDashSpeed = dashSpeed * speedCurve.Evaluate(dashTime);

                Vector3 dashMovement = _dashDir * (_currentDashSpeed * Time.deltaTime);
                Vector3 previousPosition = transform.position;
                Vector3 newPosition = transform.position + dashMovement;

                transform.position = boundsConfig.ClampPosition(newPosition, _playerColliderBounds.size);
                onDashMovementEvent?.RaiseEvent(transform.position - previousPosition);

                timer = Time.time - startTime;
                if (_hasAvoidedSomething)
                {
                    if (_cooldownCoroutine != null)
                        StopCoroutine(_cooldownCoroutine);

                    _cooldownCoroutine = StartCoroutine(CoolDownCoroutine());

                    _hasAvoidedSomething = false;
                    _hasActivatedFastCooldown = true;
                }

                yield return null;
            }

            if (_movementController.CurrentDir == Vector3.zero)
                playerAgent.ChangeStateToIdle();
            else
                playerAgent.ChangeStateToMove();

            if (!_hasAvoidedSomething && !_hasActivatedFastCooldown)
            {
                if (_cooldownCoroutine != null)
                    StopCoroutine(_cooldownCoroutine);

                _cooldownCoroutine = StartCoroutine(CoolDownCoroutine());
            }

            _hasAvoidedSomething = false;
            _hasActivatedFastCooldown = false;
        }

        private IEnumerator CoolDownCoroutine()
        {
            float timeInCooldown = 0f;
            while (timeInCooldown <= dashCoolDown)
            {
                onDashRechargeEvent.RaiseEvent(timeInCooldown / dashCoolDown);
                yield return new WaitWhile(() => pauseData.isPaused);
                timeInCooldown += Time.unscaledDeltaTime;
            }

            onDashRechargedEvent.RaiseEvent();
            _canDash = true;
        }

        private IEnumerator PhantomCoroutine()
        {
            _healthPoints.SetCanTakeDamage(false);
            yield return new WaitForSeconds(phantomDuration);
            _healthPoints.SetCanTakeDamage(true);
        }

        public void ResetDash()
        {
            _canDash = true;
            onDashRechargedEvent?.RaiseEvent();
        }
    }
}