using System.Collections;
using Events;
using Events.ScriptableObjects;
using ObstacleSystem;
using UI;
using UnityEngine;
using Utils;

namespace LevelManagement.Sequences
{
    public class ObstacleSequence : MonoBehaviour
    {
        [SerializeField] private float progressBarMaxValue = 1f;
        [SerializeField] private WarningTextSO warningData;
        
        [Header("Spawners")]
        [SerializeField] private ObstaclesSpawner obstaclesSpawner;

        [Header("Events")] 
        [SerializeField] private Vector3EventChannelSO onNewRoadManagerVelocity;
        [SerializeField] private VoidEventChannelSO onObstacleStartEvent;
        
        [Header("UI Events")] 
        [SerializeField] private FloatEventChannelSO onProgressBarChangeEvent;
        [SerializeField] private BoolEventChannelSO onProgressBarActiveEvent;
        [SerializeField] private EventChannelSO<WarningTextSO> onWarning;
        
        private bool _isObstacleSystemDisabled;
        private LevelLoopSO _levelConfig;
        private IEnumerator _postAction;
        
        [Header("Events")]
        [SerializeField] private VoidEventChannelSO onObstaclesSystemDisabled;
        
        private void OnEnable()
        {
            onObstaclesSystemDisabled.onEvent.AddListener(HandleObstacleSystemDisabled);
        }

        private void OnDisable()
        {
            if (obstaclesSpawner != null)
                onObstaclesSystemDisabled.onEvent.RemoveListener(HandleObstacleSystemDisabled);
        }

        public void SetupSequence(RoadData roadData)
        {
            obstaclesSpawner.gameObject.SetActive(false);
            onNewRoadManagerVelocity?.RaiseEvent(roadData.roadVelocity);
        }

        public void ClearSequence()
        {
            obstaclesSpawner.Clear();
            obstaclesSpawner.gameObject.SetActive(false);
        }
        
        private void HandleObstacleSystemDisabled()
        {
            _isObstacleSystemDisabled = true;
        }
        
        public IEnumerator Execute()
        {
            return GetObstacleSequence().Execute();
        }

        public void SetLevelConfig(LevelLoopSO levelConfig)
        {
            _levelConfig = levelConfig;
        }

        public void SetPostAction(IEnumerator postAction)
        {
            _postAction = postAction;
        }

        private Sequence GetObstacleSequence()
        {
            Sequence obstacleSequence = new Sequence();

            obstacleSequence.AddPreAction(ObstacleSequencePreActions());
            obstacleSequence.SetAction(ObstaclesAction());
            obstacleSequence.AddPostAction(ObstaclesPostActions());
            obstacleSequence.AddPostAction(_postAction);
            
            return obstacleSequence;
        }

    
        private IEnumerator ObstacleSequencePreActions()
        {
            onObstacleStartEvent.RaiseEvent();
            
            if (_levelConfig.obstacleData.phaseState != null)
            {
                AkSoundEngine.SetState(_levelConfig.obstacleData.phaseState.GroupId,
                    _levelConfig.obstacleData.phaseState.Id);
            }
            obstaclesSpawner.gameObject.SetActive(true);
            onProgressBarActiveEvent?.RaiseEvent(true);
            onProgressBarChangeEvent?.RaiseEvent(0);
            _isObstacleSystemDisabled = false;

            yield return null;
        }

        private IEnumerator ObstaclesAction()
        {
            float timer = 0;
            float obstaclesDuration = _levelConfig.obstacleData.obstaclesDuration;
            float obstacleCooldown = _levelConfig.obstacleData.obstacleCooldown;
            float startTime = Time.time;

            obstaclesSpawner.StartWithCooldown(obstacleCooldown, _levelConfig.obstacleData.minDistance);

            bool hasWarned = false;
            while (timer < obstaclesDuration)
            {
                timer = Time.time - startTime;
                onProgressBarChangeEvent?.RaiseEvent(Mathf.Lerp(0, progressBarMaxValue, timer / obstaclesDuration));

                if (!hasWarned && _levelConfig.obstacleData.timeToWarn > obstaclesDuration - timer)
                {
                    onWarning?.RaiseEvent(warningData);
                    hasWarned = true;
                }
                yield return null;
            }

            onProgressBarActiveEvent?.RaiseEvent(false);
            obstaclesSpawner.Disable();
        }

        private IEnumerator ObstaclesPostActions()
        {
            yield return new WaitUntil(() => _isObstacleSystemDisabled);
        }
    }
}
