using System.Collections;
using Events;
using LevelManagement.Sequences;
using ObstacleSystem;
using UnityEngine;

namespace LevelManagement
{
    public class LevelLoopManager : MonoBehaviour
    {
        [Header("Spawners")]
        [SerializeField] private ObstaclesSpawner obstaclesSpawner;

        [Header("Sequences")]
        [SerializeField] private ObstacleSequence obstacleSequence;
        [SerializeField] private MinionsSequence minionsSequence;
        [SerializeField] private BossSequence bossSequence;
        
        [SerializeField] private FloatEventChannelSO onPlayerNewMotorValue;
        
        private LevelLoopSO _levelConfig;
        private Coroutine _actualLoopSequence;

        public void StartLevelSequence(LevelLoopSO loopConfig)
        {
            SetupLevelLoop(loopConfig);
            _actualLoopSequence = StartCoroutine(StartLoopWithConfig());
        }

        public void SetupLevelLoop(LevelLoopSO loopConfig)
        {
            _levelConfig = loopConfig;
            onPlayerNewMotorValue?.RaiseEvent(_levelConfig.playerData.motorRtpcValue);

            obstacleSequence.SetupSequence(_levelConfig.obstacleData.roadData);
            minionsSequence.SetupSequence(_levelConfig.minionsData);
            bossSequence.SetupSequence(_levelConfig.bossData);
            
            obstacleSequence.SetLevelConfig(_levelConfig);
            minionsSequence.SetPostAction(bossSequence.StartBossBattle());
            obstacleSequence.SetPostAction(minionsSequence.StartMinionPhase());
        }

        private IEnumerator StartLoopWithConfig()
        {
            return obstacleSequence.Execute();
        }

        public void StopSequence()
        {
            if (_actualLoopSequence != null)
            {
                StopCoroutine(_actualLoopSequence);
                obstacleSequence.ClearSequence();
                minionsSequence.ClearSequence();
                bossSequence.ClearSequence();
            }
        }
    }
}
