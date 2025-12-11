using UnityEngine;

[CreateAssetMenu(fileName = "LaserBotConfig", menuName = "Minions/LaserConfig")]
public class LaserBotConfigSO : ScriptableObject
{
    public float movementSpeed;
    public float minDistance;


    [Header("Attack")]
    public float chargeDuration;
    public float attackDuration;
    public int damage;

    [Header("Sequence")]
    public float entryExitDuration;
    public Vector3 upperMiddle;
    public Vector3 leftMiddle;
    public Vector3 rightMiddle;
}