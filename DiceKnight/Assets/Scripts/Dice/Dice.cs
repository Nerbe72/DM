using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;


public class Dice : MonoBehaviour
{
    [SerializeField] private TMP_Text currentNumber;
    [SerializeField] private GameObject dice;
    [SerializeField] private Animator attackAnimator;

    protected DiceStats stats;
    //���� �ֻ��� ��, ��, �����ʿ� ��ġ�� �ֻ����� ���� �����մϴ�
    private (int current, int front, int right) diceNumber;


    public void MoveTo(MoveDirection _direction)
    {

    }

    

    public DiceType GetID()
    {
        return stats.DiceType;
    }

    public int GetDamage()
    {
        return stats.Damage;
    }

    public int GetDefense()
    {
        return stats.Defense;
    }

    public int GetCost()
    {
        return stats.Cost;
    }

    public int GetMovement()
    {
        return stats.Movement;
    }
}
