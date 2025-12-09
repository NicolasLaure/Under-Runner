using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events;
using MapBounds;
using Player.Controllers;
using Roads;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace ObstacleSystem
{
    public class ObstaclesSpawner : MonoBehaviour
    {
        [SerializeField] private GameObjectEventChannelSO onRoadInstantiatedEvent;
        [SerializeField] private GameObjectEventChannelSO onRoadDeletedEvent;
        [SerializeField] private VoidEventChannelSO onObstaclesDisabled;
        [SerializeField] private GameObjectEventChannelSO onObstacleDestroyed;
        [SerializeField] private MapBoundsSO mapBounds;
        [SerializeField] private RoadPoints roadPoints;
        
        private bool _shouldSpawnObject;
        private Coroutine _spawnCoroutine;

        private GameObject _lastSpawnedObstacle = null;
        private bool _shouldDisable = false;
        private bool _hasBeenDisabled = false;
        private List<GameObject> _spawnedObstacles;
        private float _minDistanceBetweenObstacles = 0.1f;

        private float _spawnCoolDown;
        private Vector3 _lastObstacleSpawnPosition;
        
        public void OnEnable()
        {
            _shouldDisable = false;
            _spawnedObstacles = new List<GameObject>();
            onRoadInstantiatedEvent?.onGameObjectEvent.AddListener(HandleNewRoadInstance);
            onRoadDeletedEvent?.onGameObjectEvent.AddListener(HandleDeleteObstacle);
            onObstacleDestroyed?.onGameObjectEvent.AddListener(DeleteObstacle);
        }

        private void Update()
        {
            if (_shouldDisable && _spawnedObstacles.Count == 0 && !_hasBeenDisabled)
            {
                onObstaclesDisabled.RaiseEvent();
                _hasBeenDisabled = true;
                if (_spawnCoroutine != null)
                   StopCoroutine(SpawnObjectCoroutine());
            }
        }

        private void OnDisable()
        {
            onRoadInstantiatedEvent?.onGameObjectEvent.RemoveListener(HandleNewRoadInstance);
            onRoadDeletedEvent?.onGameObjectEvent.RemoveListener(HandleDeleteObstacle);
            onObstacleDestroyed?.onGameObjectEvent.RemoveListener(DeleteObstacle);

            if (_spawnCoroutine != null)
                StopCoroutine(SpawnObjectCoroutine());
            
            Clear();
        }

        public void Disable()
        {
            onRoadInstantiatedEvent?.onGameObjectEvent.RemoveListener(HandleNewRoadInstance);
            if (_spawnCoroutine != null)
                StopCoroutine(SpawnObjectCoroutine());

            _shouldDisable = true;
        }

        public void StartWithCooldown(float cooldown, float minDistance)
        {
            _minDistanceBetweenObstacles = minDistance;
            _spawnCoolDown = cooldown;
            _shouldDisable = false;
            _hasBeenDisabled = false;
            StartCoroutine(SpawnObjectCoroutine());
        }
        
        private void HandleDeleteObstacle(GameObject road)
        {
            ObstaclesCollision[] obstaclesCollision = road.GetComponentsInChildren<ObstaclesCollision>();
            
            if (obstaclesCollision == null || obstaclesCollision.Length == 0) return;
            foreach (var collision in obstaclesCollision)
            {
                DeleteObstacle(collision.gameObject);
            }
        }

        private void DeleteObstacle(GameObject obstacle)
        {
            ObstaclesObjectPool.Instance?.ReturnToPool(obstacle);
            _spawnedObstacles.Remove(obstacle);
        }
        
        private void HandleNewRoadInstance(GameObject road)
        {
            if (!_shouldSpawnObject)
                return;
            
            _shouldSpawnObject = false;
            float roadWidth = mapBounds.horizontalBounds.max - mapBounds.horizontalBounds.min;
            float roadDepth = road.GetComponentInChildren<RoadDepthObtainer>().GetRoadDepth();

            float roadVelocity = road.GetComponentInChildren<Movement>().GetVelocity();
            float obstaclesToInstantiateCount = Mathf.Floor(roadDepth / roadVelocity / _spawnCoolDown);

            for (int i = 0; i < obstaclesToInstantiateCount; i++)
            {
                GameObject obstacle = ObstaclesObjectPool.Instance?.GetPooledObject();
                if (obstacle == null)
                {
                    Debug.LogWarning("Obstacle is null");
                    continue;
                }
                
                obstacle.transform.SetParent(road.transform);
                obstacle.SetActive(true);
                
                _lastSpawnedObstacle = obstacle;
                Vector3 spawnPositionInX = GetSpawnPosition(roadWidth, obstacle.transform.localPosition.y);
                _lastObstacleSpawnPosition = new Vector3(spawnPositionInX.x, spawnPositionInX.y,
                    roadDepth * (i / obstaclesToInstantiateCount));
                obstacle.transform.localPosition = _lastObstacleSpawnPosition;
                _spawnedObstacles.Add(obstacle);
            }

            if (_spawnCoroutine != null)
                StopCoroutine(SpawnObjectCoroutine());

            _spawnCoroutine = StartCoroutine(SpawnObjectCoroutine());
        }

        private Vector3 GetSpawnPosition(float width, float height)
        {
            bool hasPossibleSpawnPosition = false;
            Vector3 spawnPosition = new Vector3();
            while (!hasPossibleSpawnPosition)
            {
                spawnPosition = new Vector3(roadPoints.xPoints[Random.Range(0, roadPoints.xPoints.Length)], height, 0);

                if (_lastSpawnedObstacle == null || Mathf.Abs(spawnPosition.x - _lastObstacleSpawnPosition.x) >
                    _minDistanceBetweenObstacles)
                    hasPossibleSpawnPosition = true;
            }

            return spawnPosition;
        }
        
        private IEnumerator SpawnObjectCoroutine()
        {
            yield return new WaitForSeconds(_spawnCoolDown);
            _shouldSpawnObject = true;
        }

        public void Clear()
        {
            if (_spawnedObstacles == null) return;
            foreach (var spawnedObstacle in _spawnedObstacles.ToList())
            {
                if(spawnedObstacle == null) continue;
                DeleteObstacle(spawnedObstacle);
            }
        }
    }
}