using System.Collections;
using UnityEngine;

namespace LaserBot.Controllers
{
    public class EntryController : LaserBotController
    {
        [SerializeField] private LaserBotConfigSO minionConfig;

        private Coroutine _entryCoroutine;

        public void OnDisable()
        {
            if (_entryCoroutine != null)
                StopCoroutine(_entryCoroutine);
        }

        public void Enter()
        {
            _entryCoroutine = StartCoroutine(StartEntry());
        }

        private IEnumerator StartEntry()
        {
            float timer = 0;
            float startTime = Time.time;

            Vector3 target;
            Vector3 initialPos = transform.position;

            if (botAgent.dir == Directions.Left)
                target = minionConfig.rightMiddle;
            else if (botAgent.dir == Directions.Right)
                target = minionConfig.leftMiddle;
            else
                target = minionConfig.upperMiddle;

            while (timer < minionConfig.entryExitDuration)
            {
                timer = Time.time - startTime;
                transform.position = Vector3.Lerp(initialPos, target, timer / minionConfig.entryExitDuration);
                yield return null;
            }

            botAgent.ChangeStateToMove();
        }
    }
}