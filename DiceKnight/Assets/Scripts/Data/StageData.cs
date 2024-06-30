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
    public Background BackgroundMusic;
    public SerializedDictionary<Vector2, DiceType> EnemyDiceSet;
    public float PlayerHp;
    public float EnemyHp;
    public int CostLimit;
    public int DiceLimit;
    [Tooltip("���� ����ϴ� �ð�(��)\n�������� ���۽� (1 ~ ���� �ð�)�� ���� ����")][Range(1, 10)]public float EnemyThinkingTime;

}
