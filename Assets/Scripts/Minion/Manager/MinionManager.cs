using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events;
using Events.ScriptableObjects;
using LevelManagement;
using Minion.ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Minion.Manager
{
    public class MinionManager : MonoBehaviour
    {
        [SerializeField] private GameObject player;

        [Header("Events")]
        [SerializeField] private MinionAgentEventChannelSO onMinionDeletedEvent;
        [SerializeField] private VoidEventChannelSO onAllMinionsDestroyedEvent;
        [SerializeField] private VoidEventChannelSO onGameplayEndEvent;
        [SerializeField] private MinionAgentEventChannelSO onMinionWantsToAttackEvent;
        [SerializeField] private MinionAgentEventChannelSO onMinionAttackedEvent;

        private List<MinionAgent> _minions;
        private bool _isSpawning;
        private Coroutine _spawnCoroutine;
        private MinionSpawnerSO _minionSpawnerConfig;
        private MinionsManagerSO _minionManagerConfig;

        public bool IsSpawning => _isSpawning;

        protected void OnEnable()
        {
            _spawnCoroutine = StartCoroutine(SpawnMinions());
            onMinionDeletedEvent?.onTypedEvent.AddListener(HandleDeletedEvent);
            onGameplayEndEvent?.onEvent.AddListener(RemoveAllMinions);
        }

        protected void OnDisable()
        {
            onMinionDeletedEvent?.onTypedEvent.RemoveListener(HandleDeletedEvent);
            onGameplayEndEvent?.onEvent.RemoveListener(RemoveAllMinions);
            StopSpawnCoroutine();
        }

        private void Update()
        {
            if (MinionsAttackingCount() < _minionManagerConfig.maxMinionsAttackingAtSameTime)
            {
                HandleNewMinionToAttack();
            }
        }

        private void StopSpawnCoroutine()
        {
            if (_spawnCoroutine != null)
                StopCoroutine(_spawnCoroutine);
        }

        private void HandleNewMinionToAttack()
        {
            List<MinionAgent> minionsInIdle = _minions.Where(minion => minion.IsInIdleState()).ToList();

            if (minionsInIdle.Count == 0) return;
            minionsInIdle[Random.Range(0, minionsInIdle.Count)].StartAttacking();
        }

        private int MinionsAttackingCount()
        {
            return _minions.Count(minion => minion.IsInAttackState());
        }

        private void RemoveAllMinions()
        {
            if (_minions == null) return;
            foreach (var minion in _minions.ToList())
            {
                _minions.Remove(minion);
                MinionObjectPool.Instance?.ReturnToPool(minion.gameObject);
            }
        }

        private void HandleDeletedEvent(MinionAgent deletedMinion)
        {
            _minions.Remove(deletedMinion);

            MinionObjectPool.Instance?.ReturnToPool(deletedMinion.gameObject);

            if (_minions.Count == 0 && !_isSpawning)
            {
                onAllMinionsDestroyedEvent?.RaiseEvent();
            }
        }


        private IEnumerator SpawnMinions()
        {
            _isSpawning = true;
            _minions = new List<MinionAgent>();

            int minionsSpawned = 0;
            while (minionsSpawned < _minionSpawnerConfig.minionsToSpawn)
            {
                if (minionsSpawned != 0) yield return new WaitForSeconds(_minionSpawnerConfig.timeBetweenSpawns);
                if (_minions.Count >= _minionManagerConfig.maxMinionsAtSameTime) continue;

                GameObject minion = MinionObjectPool.Instance?.GetPooledObject();
                if (minion == null)
                {
                    Debug.LogError("Minion was null");
                    break;
                }

                MinionAgent minionAgent = minion.GetComponent<MinionAgent>();
                minionAgent.SetPlayer(player);

                minion.transform.position = _minionSpawnerConfig.GetSpawnPoint();
                minion.SetActive(true);

                _minions.Add(minionAgent);
                minionsSpawned++;
            }

            _isSpawning = false;
            yield return null;
        }

        public void Clear()
        {
            RemoveAllMinions();
            StopSpawnCoroutine();
        }

        public void SetupManager(MinionsData levelConfigMinionsData)
        {
            _minionManagerConfig = levelConfigMinionsData.managerData;
            _minionSpawnerConfig = levelConfigMinionsData.spawnerData;
        }
    }
}