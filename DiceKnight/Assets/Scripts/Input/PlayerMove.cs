using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : InputAndAction
{
    public static PlayerMove Instance;

    protected void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this);
            return;
        }
        base.Awake();
        InputManager.TurnActionList.Add(Turn.PlayerMove, this);

        turnName = "PlayerMove";
    }

    protected override void InputStyle()
    {

    }

    //�ֻ����� ������

    //�̵� ��θ� ���콺�� �ѹ��� �׸� -> �밢�� �Ұ�

    //Ȯ���� ������ action �Լ� ����
}
