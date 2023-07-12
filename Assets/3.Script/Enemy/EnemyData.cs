using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/EnemyData", fileName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    public float hp = 1000f;
    public float atk = 30f;
    public float def = 0.1f;
    public float attackTime = 1f;
    public float moveSpeed = 1f;
    public float attackRange = 2f;
    public float attackDelay = 3f;
    public Weather weather;
    public string monsterName = "basicBoss";
}

