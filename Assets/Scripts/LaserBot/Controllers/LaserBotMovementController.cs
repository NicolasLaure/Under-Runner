using System.Collections;
using Events;
using UnityEngine;

namespace LaserBot.Controllers
{
    public class LaserBotMovementController : LaserBotController
    {
        [SerializeField] private LaserBotConfigSO minionConfig;
        [SerializeField] private Vector3EventChannelSO onPlayerMovedEvent;
        [SerializeField] private Vector3EventChannelSO onDashMovementEvent;

        private Vector3 _moveDir;
        private Coroutine _moveCoroutine;

        public void OnDisable()
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);
        }

        public void Enter()
        {
            _moveCoroutine = StartCoroutine(HandleChangeState());
        }

        private IEnumerator HandleChangeState()
        {
            float prevTime = Time.time;
            float delta = 0;
            bool notCloseEnough = true;

            while (notCloseEnough)
            {
                delta = Time.time - prevTime;

                if (botAgent.dir == Directions.Left || botAgent.dir == Directions.Right)
                {
                    if (botAgent.Player.transform.position.z < transform.position.z)
                        _moveDir = Vector3.back;
                    else
                        _moveDir = Vector3.forward;

                    notCloseEnough = Mathf.Abs(transform.position.z - botAgent.Player.transform.position.z) > minionConfig.minDistance;
                }
                else
                {
                    if (botAgent.Player.transform.position.x < transform.position.x)
                        _moveDir = Vector3.left;
                    else
                        _moveDir = Vector3.right;

                    notCloseEnough = Mathf.Abs(transform.position.x - botAgent.Player.transform.position.x) > minionConfig.minDistance;
                }

                _moveDir.y = 0;
                transform.Translate(_moveDir * (minionConfig.movementSpeed * delta), Space.World);
                prevTime = Time.time;
                yield return null;
            }

            botAgent.ChangeStateToAttack();
        }
    }
}