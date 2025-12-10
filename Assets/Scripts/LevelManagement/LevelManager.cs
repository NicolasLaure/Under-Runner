using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using Events.ScriptableObjects;
using Health;
using Input;
using LevelManagement.Sequences;
using UnityEngine;
using Utils;

namespace LevelManagement
{
    [RequireComponent(typeof(StartLevelSequence))]
    [RequireComponent(typeof(EndLevelSequence))]
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private List<LevelLoopSO> loopConfigs;
        [SerializeField] private LevelLoopManager levelLoopManager;
        [SerializeField] private HealthPoints playerHealthPoints;
        [SerializeField] private HealthPoints bossHealthPoints;
        [SerializeField] private String creditsScene = "Credits";

        [Header("Inputs")] [SerializeField] private InputHandlerSO inputHandler;

        [Header("Events")]
        [SerializeField] private IntEventChannelSO onEnemyDamageEvent;
        [SerializeField] private VoidEventChannelSO onEnemyShouldLeaveEvent;
        [SerializeField] private VoidEventChannelSO onEnemyLeftEvent;
        [SerializeField] private VoidEventChannelSO onEnemyDeathEvent;
        [SerializeField] private VoidEventChannelSO onPlayerDeathEvent;
        [SerializeField] private BoolEventChannelSO onTryAgainCanvasEvent;
        [SerializeField] private VoidEventChannelSO onResetGameplayEvent;
        [SerializeField] private StringEventChannelSo onOpenSceneEvent;

        [Header("Music")] [SerializeField] private AK.Wwise.State resettedMusicState;

        private int _loopConfigIndex;
        private LevelLoopSO _actualLoopConfig;
        private StartLevelSequence _startLevelSequence;
        private Coroutine _startCoroutine;

        private void Start()
        {
            _startLevelSequence = GetComponent<StartLevelSequence>();
            Sequence sequence = _startLevelSequence.GetStartSequence(loopConfigs[0].roadData);
            sequence.AddPostAction(HandleStartGameplay());
            _startCoroutine = StartCoroutine(sequence.Execute());
        }

        private IEnumerator HandleStartGameplay()
        {
            HandleResetGameplay();
            RemoveSkipHandler();
            yield return null;
        }

        private void OnEnable()
        {
            onEnemyDeathEvent?.onEvent.AddListener(HandleFinish);
            onResetGameplayEvent?.onEvent.AddListener(HandleResetGameplay);
            onEnemyDamageEvent?.onIntEvent.AddListener(HandleNextPhase);
            onEnemyLeftEvent?.onEvent.AddListener(NextLevel);
            onPlayerDeathEvent?.onEvent.AddListener(HandlePlayerDeath);
            inputHandler?.onPlayerAttack.AddListener(SkipCinematic);
            inputHandler?.onSkipSequence.AddListener(HandleSkipLevel);
        }

        private void OnDisable()
        {
            onEnemyDeathEvent?.onEvent.RemoveListener(HandleFinish);
            onResetGameplayEvent?.onEvent.RemoveListener(HandleResetGameplay);
            onEnemyDamageEvent?.onIntEvent.RemoveListener(HandleNextPhase);
            onEnemyLeftEvent?.onEvent.RemoveListener(NextLevel);
            onPlayerDeathEvent?.onEvent.RemoveListener(HandlePlayerDeath);
            RemoveSkipHandler();
            inputHandler?.onSkipSequence.RemoveListener(HandleSkipLevel);
        }

        private void HandleSkipLevel()
        {
            if (_actualLoopConfig == null || _startCoroutine != null)
            {
                Debug.Log("NOT SKIPPING");
                return;
            }

            levelLoopManager.StopSequence();
            NextLevel();

            if (_actualLoopConfig == null)
                HandleFinish();
        }

        private void SkipCinematic()
        {
            if (_startCoroutine != null)
                StopCoroutine(_startCoroutine);

            _startCoroutine = null;

            _startLevelSequence.SkipCinematic();
            HandleResetGameplay();

            RemoveSkipHandler();
        }

        private void RemoveSkipHandler()
        {
            inputHandler?.onPlayerAttack.RemoveListener(SkipCinematic);
        }

        private void HandlePlayerDeath()
        {
            onTryAgainCanvasEvent?.RaiseEvent(true);
            levelLoopManager.StopSequence();
        }

        private void HandleNextPhase(int hitPointsLeft)
        {
            if (hitPointsLeft < _actualLoopConfig.bossData.hitPointsToNextPhase)
            {
                onEnemyShouldLeaveEvent?.RaiseEvent();
            }
        }

        public bool TrySetLevel(int value)
        {
            if (value >= loopConfigs.Count || value < 0)
                return false;

            _loopConfigIndex = value;
            SetActualLoop();
            if (_actualLoopConfig != null)
                levelLoopManager.StartLevelSequence(_actualLoopConfig);
            else
                levelLoopManager.StopSequence();

            return true;
        }

        public void NextLevel()
        {
            TrySetLevel(_loopConfigIndex + 1);
        }

        public void PreviousLevel()
        {
            TrySetLevel(_loopConfigIndex - 1);
        }

        private void SetActualLoop()
        {
            if (_loopConfigIndex >= loopConfigs.Count || _loopConfigIndex < 0)
            {
                Debug.LogWarning("Loop index out of range");
                _actualLoopConfig = null;
                return;
            }

            _actualLoopConfig = loopConfigs[_loopConfigIndex];
        }

        private void HandleFinish()
        {
            levelLoopManager.StopSequence();

            Sequence sequence = GetComponent<EndLevelSequence>().GetEndSequence();
            StartCoroutine(sequence.Execute());
        }

        private void HandleResetGameplay()
        {
            _loopConfigIndex = 0;
            playerHealthPoints.ResetHitPoints();
            bossHealthPoints.ResetHitPoints();

            SetActualLoop();
            levelLoopManager.StartLevelSequence(_actualLoopConfig);
        }
    }
}