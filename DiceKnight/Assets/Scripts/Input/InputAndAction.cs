using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputAndAction : MonoBehaviour
{
    protected string turnName = "";
    private bool turnEnter;

    protected virtual void Awake()
    {
        turnEnter = false;
    }

    protected virtual void Start()
    {
        enabled = false;
        turnEnter = true;
    }

    protected virtual void Update()
    {
        InputStyle();
    }

    //��Ʈ��Ʈ�� enable �Ǵ°��� ��ȣ�� ���� �簳��
    protected virtual void OnEnable()
    {
        if (turnEnter)
        {
            ShowTurnName(StageManager.Instance.TurnNamePanel, StageManager.Instance.TurnNameText);
        }
    }


    //�� ���� ���۵Ǹ� �� �̸��� ǥ���ϰ� �������� �Ѿ
    protected virtual void ShowTurnName(GameObject _namePanel, TMP_Text _turnText)
    {
        _turnText.text = turnName;
        _namePanel.GetComponent<Animator>().SetTrigger("ShowTurn"); //todo
    }

    //�� �̸� ǥ�� �� PreAction�� ȣ���Ͽ� ���� ������ ������ (�����-ȭ�� �� ���� ������ �ൿ)
    protected virtual void PreAction()
    {

    }

    //�� �ϸ��� �Է� ����� ������
    protected virtual void InputStyle()
    {

    }

    //�� ���� �Է��� ������ Action�� ȣ���Ͽ� ������ ������
    protected virtual void Action()
    {

    }

    //�ൿ�� �ֻ����� ������
    protected virtual void SelectDice()
    {

    }

    public void SetEnable(bool _enabled)
    {
        enabled = _enabled;
    }
}
