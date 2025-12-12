using System;
using System.Collections;
using System.Collections.Generic;
using LaserBot;
using Minion.Manager;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class LaserBotManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private MinionManager minionManager;

    [SerializeField] private GameObject laserBotPrefab;
    [SerializeField] private float minSpawnTime;
    [SerializeField] private float maxSpawnTime;

    [SerializeField] private Vector3 leftSpawnPos;
    [SerializeField] private Vector3 middleSpawnPos;
    [SerializeField] private Vector3 rightSpawnPos;

    private Coroutine _spawnCoroutine;

    protected void OnEnable()
    {
        _spawnCoroutine = StartCoroutine(SpawnMinions());
    }

    protected void OnDisable()
    {
        if (_spawnCoroutine != null)
            StopCoroutine(_spawnCoroutine);
    }

    private IEnumerator SpawnMinions()
    {
        while (minionManager.IsSpawning)
        {
            float randomWait = Mathf.Lerp(minSpawnTime, maxSpawnTime, Random.Range(0.0f, 1.0f));
            yield return new WaitForSeconds(randomWait);

            (Directions dir, Vector3 pos) = GetRandomDirAndPos();

            GameObject minion = Instantiate(laserBotPrefab, pos, quaternion.identity);

            if (dir == Directions.Left)
                minion.transform.forward = Vector3.left;
            else if (dir == Directions.Right)
                minion.transform.forward = Vector3.right;
            else
                minion.transform.forward = Vector3.back;

            LaserBotAgent minionAgent = minion.GetComponent<LaserBotAgent>();
            minionAgent.Player = player;
            minionAgent.dir = dir;
            
            minionAgent.enabled = true;
        }

        yield return null;
    }

    private (Directions dir, Vector3 pos) GetRandomDirAndPos()
    {
        int randomDir = Random.Range(0, 3);
        if (randomDir == 0)
            return (Directions.Right, leftSpawnPos);
        if (randomDir == 1)
            return (Directions.Left, rightSpawnPos);

        return (Directions.Down, middleSpawnPos);
    }
}