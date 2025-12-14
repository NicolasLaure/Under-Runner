using System;
using Minion.ScriptableObjects;
using UnityEngine;

namespace LevelManagement
{
    [Serializable]
    public class ObstacleData
    {
        public float obstaclesDuration;
        public float obstacleCooldown;
        public float timeToWarn = 4.0f;
        public float minDistance;
        public RoadData roadData;
        public AK.Wwise.State phaseState;
    }

    [Serializable]
    public class RoadData
    {
        public Vector3 roadVelocity;
    }

    [Serializable]
    public class BossData
    {
        public int hitPointsToNextPhase;
        public FallingAttackData fallingAttackData;
        public AK.Wwise.State minionsToBossState;
    }

    [Serializable]
    public class FallingAttackData
    {
        public float spawnRadiusFromPlayer;
        public float timeBetweenSpawns;
        public float spawnCooldown;
        public int spawnQuantity;
        public float acceleration;
    }

    [Serializable]
    public class MinionsData
    {
        public MinionsManagerSO managerData;
        public MinionSpawnerSO spawnerData;
        public RoadData roadData;
    }

    [Serializable]
    public class PlayerData
    {
        public float motorRtpcValue;
    }
    
    [CreateAssetMenu(menuName = "Level Loop Config", fileName = "LevelLoopConfig", order = 0)]
    public class LevelLoopSO : ScriptableObject
    {
        public ObstacleData obstacleData;
        public BossData bossData;
        public MinionsData minionsData;
        public PlayerData playerData;
    }
}
