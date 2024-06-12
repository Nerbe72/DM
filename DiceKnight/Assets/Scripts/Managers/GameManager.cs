using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int clearedStage = 0;

    //������ �ֻ��� ���
    private Dictionary<bool, DiceType> ownDice = new Dictionary<bool, DiceType>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this);


    }

    private void Start()
    {
        AddDice();
    }


    //�׽�Ʈ�� �ֻ��� �߰�
    private void AddDice()
    {
        ownDice.Add(true, DiceType.Normal);
    }

    public void SetStage(Difficulty _diff)
    {
        //���̵��� �� �ֻ����� ��ġ�� cost�� �ҷ����� ������ ���� ������ ������
        //�����;��� ����
        /*
         * ���� ��� ������ �ڽ�Ʈ�� ��
         * ���� ��ġ ������ ���̽� ��
         * ���� ������ �ֻ���
         * ���� ������ ��ų ���(�ļ��� �߰�)
         * 
         * ���� ���̽� ����
         * �� ���̽��� ��ġ
         * �� ���̽��� ���� ����(����)
         * ���� �����ϴ� �ð�(����)�� �ּҰ�
         * 
         * �� �˰���� ���õ� ����
         * 
         */
    }
}
