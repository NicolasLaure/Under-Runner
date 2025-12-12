using System.Collections;
using UnityEngine;

namespace LaserBot.Controllers
{
    public class ExitController : LaserBotController
    {
        [SerializeField] private LaserBotConfigSO minionConfig;
        private Coroutine _exitCoroutine;

        public void OnDisable()
        {
            if (_exitCoroutine != null)
                StopCoroutine(_exitCoroutine);
        }

        public void Enter()
        {
            _exitCoroutine = StartCoroutine(StartExit());
        }

        private IEnumerator StartExit()
        {
            float timer = 0;
            float prevTime = Time.time;
            float startTime = Time.time;
            float delta = 0;
            Vector3 dir;
            while (timer < minionConfig.entryExitDuration)
            {
                delta = Time.time - prevTime;
                timer = Time.time - startTime;
                if (botAgent.dir == Directions.Left || botAgent.dir == Directions.Right)
                    dir = Vector3.back;
                else
                    dir = Vector3.forward;

                transform.Translate(dir * (minionConfig.movementSpeed * 2 * delta), Space.World);
                prevTime = Time.time;
                yield return null;
            }

            Destroy(botAgent.gameObject);
        }
    }
}