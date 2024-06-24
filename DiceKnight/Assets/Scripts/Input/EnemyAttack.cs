using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : InputAndAction
{
    public static EnemyAttack Instance;

    protected override void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this);
            return;
        }

        base.Awake();
        InputManager.TurnActionList.Add(Turn.EnemyAttack, this);

        turnName = "EnemyAttack";
    }

    protected override void PreAction()
    {

    }

    protected override void InputAction()
    {
        //Ÿ���� ������ ���� ���ڰ� ���� ���� �ֻ����� ����

        //Ÿ���� ������ ���� ���ڰ� ���� ���� �ֻ����� ����

        //�����ϸ� ǥ���ϰ� 1�� ��� �� Action�Լ��� �̵�
        inputHolder = true;
    }

    protected override void Action()
    {

    }
}
