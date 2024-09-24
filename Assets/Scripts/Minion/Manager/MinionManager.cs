using System.Collections;
using System.Collections.Generic;
using Events;
using Minion.ScriptableObjects;
using UnityEngine;

namespace Minion.Manager
{
    public class MinionManager : MonoBehaviour
    {
        [SerializeField] private MinionSpawnerSO minionSpawnerConfig;
        [SerializeField] private GameObject player;
        
        [Header("Events")]
        [SerializeField] private GameObjectEventChannelSO onMinionDeletedEvent;
        [SerializeField] private VoidEventChannelSO onAllMinionsDestroyedEvent;
        
        private List<GameObject> _minions;
        private bool _isSpawning;
        private Coroutine _spawnCoroutine;
    
        protected void OnEnable()
        {
            _spawnCoroutine = StartCoroutine(SpawnMinions());
            onMinionDeletedEvent?.onGameObjectEvent.AddListener(HandleDeletedEvent);
        }

        protected void OnDisable()
        {
            onMinionDeletedEvent?.onGameObjectEvent.RemoveListener(HandleDeletedEvent);
            StopCoroutine(_spawnCoroutine);
        }
        
        private void HandleDeletedEvent(GameObject deletedMinion)
        {
            MinionObjectPool.Instance?.ReturnToPool(deletedMinion);
            
            _minions.Remove(deletedMinion);

            if (_minions.Count == 0 && !_isSpawning)
            {
                onAllMinionsDestroyedEvent?.RaiseEvent();
            }
        }

        private IEnumerator SpawnMinions()
        {
            _isSpawning = true;
            _minions = new List<GameObject>();

            for (int i = 0; i < minionSpawnerConfig.minionsToSpawn; i++)
            {
                GameObject minion = MinionObjectPool.Instance?.GetPooledObject();
                if (minion == null)
                {
                    Debug.LogError("Minion was null");
                    break;
                }

                MinionAgent minionAgent = minion.GetComponent<MinionAgent>();
                minionAgent.SetPlayer(player);
                
                minion.transform.position = minionSpawnerConfig.GetSpawnPoint();
                minion.SetActive(true);

                _minions.Add(minion);
                yield return new WaitForSeconds(minionSpawnerConfig.timeBetweenSpawns);
            }

            _isSpawning = false;
            yield return null;
        }
    }
}
