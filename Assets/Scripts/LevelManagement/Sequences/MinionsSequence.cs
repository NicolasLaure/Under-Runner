using System.Collections;
using Events;
using Events.ScriptableObjects;
using Minion.Manager;
using UnityEngine;
using Utils;
using VFX;

namespace LevelManagement.Sequences
{
    public class MinionsSequence : MonoBehaviour
    {
        [SerializeField] private MinionManager minionManager;
        [SerializeField] private MotorFlamesData motorData;
        
        [Header("Events")] [SerializeField] private VoidEventChannelSO onAllMinionsDestroyedEvent;
        [SerializeField] private VoidEventChannelSO onMinionsSequenceStart;
        [SerializeField] private Vector3EventChannelSO onNewRoadsVelocity;
        [SerializeField] private MotorFlamesChannelSO onNewMotorData;
        
        private bool _areAllMinionsDestroyed;
        private RoadData _actualMinionsRoadData;
        private IEnumerator _postAction;
        
        private void OnEnable()
        {
            onAllMinionsDestroyedEvent?.onEvent.AddListener(HandleAllMinionsDestroyed);
        }

        private void OnDisable()
        {
            onAllMinionsDestroyedEvent?.onEvent.RemoveListener(HandleAllMinionsDestroyed);
        }

        public void SetPostAction(IEnumerator postAction)
        {
            _postAction = postAction;
        }

        public void SetupSequence(MinionsData levelConfigMinionsData)
        {
            minionManager.SetupManager(levelConfigMinionsData);
            _actualMinionsRoadData = levelConfigMinionsData.roadData;
            minionManager.gameObject.SetActive(false);
        }

        private IEnumerator MinionSequencePreActions()
        {
            onMinionsSequenceStart.RaiseEvent();
            _areAllMinionsDestroyed = false;

            onNewRoadsVelocity?.RaiseEvent(_actualMinionsRoadData.roadVelocity);
            onNewMotorData?.RaiseEvent(motorData);
            yield return null;
        }
        
        private void HandleAllMinionsDestroyed()
        {
            _areAllMinionsDestroyed = true;
        }

        private IEnumerator SetMinionManager(bool value)
        {
            minionManager.gameObject.SetActive(value);

            yield return new WaitUntil(() => _areAllMinionsDestroyed);
        }

        public IEnumerator StartMinionPhase()
        {
            Sequence minionSequence = new Sequence();

            minionSequence.AddPreAction(MinionSequencePreActions());
            minionSequence.SetAction(SetMinionManager(true));
            minionSequence.AddPostAction(SetMinionManager(false));
            minionSequence.AddPostAction(_postAction);

            return minionSequence.Execute();
        }


        public void ClearSequence()
        {
            minionManager.Clear();
            HandleAllMinionsDestroyed();
        }
    }
}
