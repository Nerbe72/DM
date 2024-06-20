using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct StageData
{
    public Difficulty StageDifficulty;
    public SerializedDictionary<Vector2, DiceType> EnemyDiceSet;
    public float PlayerHp;
    public float EnemyHp;
    public int CostLimit;
    public int DiceLimit;
    [Tooltip("���� �����ϴ� �ð�(��)\n�������� ���۽� (1 ~ ���� �ð�)�� ���� ����")][Range(10, 60)]public float EnemyThinkingTime;

}