using System.Collections;
using _Dev.UnderRunnerTest.Scripts.Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Dev.UnderRunnerTest.Scripts.Player
{
    public class PlayerDash : MonoBehaviour
    {
        [SerializeField] private InputHandlerSO inputHandler;

        [FormerlySerializedAs("dashLength")] [Header("Dash Configuration")] [SerializeField]
        private float dashSpeed;

        [SerializeField] private float dashDuration;
        [SerializeField] private float dashCoolDown;


        private PlayerMovement _movement;
        private CharacterController _characterController;

        private bool _canDash = true;
        private Coroutine _dashCoroutine = null;

        private void Awake()
        {
            _movement = GetComponent<PlayerMovement>();
            _characterController = GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            inputHandler.onPlayerDash.AddListener(HandleDash);
        }

        private void OnDisable()
        {
            inputHandler.onPlayerDash.RemoveListener(HandleDash);
        }

        private void HandleDash()
        {
            if (!_canDash)
                return;

            if (_dashCoroutine != null)
                StopCoroutine(_dashCoroutine);

            _dashCoroutine = StartCoroutine(DashCoroutine());
        }

        private void Dash(Vector3 dir)
        {
            _characterController.Move(dir * (dashSpeed * Time.deltaTime));
        }

        private IEnumerator DashCoroutine()
        {
            float startTime = Time.time;
            float timer = 0;
            _canDash = false;

            while (timer < dashDuration)
            {
                Dash(_movement.CurrentDir);
                timer = Time.time - startTime;
                yield return null;
            }

            yield return CoolDownCoroutine();
            _canDash = true;
        }

        private IEnumerator CoolDownCoroutine()
        {
            yield return new WaitForSeconds(dashCoolDown);
        }
    }
}