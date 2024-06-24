using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : InputAndAction
{
    public static PlayerAttack Instance;

    [SerializeField] private Button okBtn;

    private Dice selectedDice;
    private (int x, int y) selectedXY;

    private List<Dice> targetDice = new List<Dice>();
    private List<TileData> targetTile = new List<TileData>();

    private (int x, int y) targetXY;

    /// <summary>
    /// ù ������ǥ�� 0���� ����, �� ��ǥ�κ��� ������ �Ÿ��� �����ϰ� ����
    /// </summary>
    private List<Vector2> selectedAttackArea = new List<Vector2>();

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
        InputManager.TurnActionList.Add(Turn.PlayerAttack, this);

        turnName = "PlayerAttack";

        okBtn.onClick.AddListener(DoAttack);
    }

    protected override void Start()
    {
        stageManager = StageManager.Instance;
        base.Start();
    }

    protected override void InputAction()
    {
        if (!okBtn.gameObject.activeSelf) okBtn.gameObject.SetActive(true);

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, LayerMask.GetMask("Dice"));

            if (hit.collider == null || hit.collider.tag != "Dice") return;

            //�ʿ��� �� �ʱ�ȭ
            if (selectedDice != null)
                InitTargets();

            selectedDice = hit.collider.GetComponent<Dice>();
            selectedDice.SetFrameBlinking();

            SetTarget();
        }
    }

    protected override void Action()
    {
        StartCoroutine(attackCo());

        actionHolder = true;
    }

    private void SetTarget()
    {
        selectedAttackArea = selectedDice.GetAttackArea();
        selectedXY = stageManager.GetXYFromPlayerDice(selectedDice);

        if (selectedXY == (-1, -1))
            return;


        for (int yPos = 0; yPos < stageManager.GridYSize; yPos++)
        {
            //selectedAttackArea�� ù ĭ�� ������ ó�� Ž���� ��ǥ�� ����Ŵ
            //attackPos.x =  ������ �ֻ���.x + ù ������ǥ.x
            (int x, int y) attackPos = (selectedXY.x + (int)selectedAttackArea[0].x, yPos);

            //�ش� ��ǥ�� �ֻ����� ������ ���� y�� Ž��
            if (stageManager.GetDiceFromXY(false, attackPos) == null) continue;

            //Ž���� ���� 1ȸ �ֻ��� ����� �ش� ��ġ���� ����
            targetDice.Add(stageManager.GetDiceFromXY(false, attackPos));
            targetXY = attackPos;
            break;
        }

        //Ÿ���� ���ٸ� Ÿ���� ���� ��ĭ�� ����
        if (targetDice.Count == 0)
        {
            targetXY = (selectedXY.x + (int)selectedAttackArea[0].x, 0);
            targetTile.Add(stageManager.GetTileDataFromXY(false, targetXY));
        }

        //���� ������ ���� ������ üũ
        int attackCount = selectedAttackArea.Count;
        for (int i = 1; i < attackCount; i++)
        {
            //ù Ÿ���� �������� attackArea�� ��ǥ�� ���Ͽ� üũ
            (int x, int y) attackPos = (targetXY.x + (int)selectedAttackArea[i].x, targetXY.y + (int)selectedAttackArea[i].y);

            if (stageManager.GetDiceFromXY(false, attackPos) == null)
                targetTile.Add(stageManager.GetTileDataFromXY(false, attackPos));
            else
                targetDice.Add(stageManager.GetDiceFromXY(false, attackPos));
        }

        //���� Ÿ�� �ð��� ǥ��
        BlinkTargets();
    }

    private void DoAttack()
    {
        //���� ��� ���� �ȵ� üũ
        if (selectedDice == null) return;

        okBtn.gameObject.SetActive(false);
        inputHolder = true;
        actionHolder = false;
    }

    private void InitTargets()
    {
        int tileCount = targetTile.Count;
        int diceCount = targetDice.Count;

        for (int i = 0; i < tileCount; i++)
        {
            targetTile[i].UnsetBlinking();
        }

        for (int i = 0; i < diceCount; i++)
        {
            if (targetDice[i] != null)
            targetDice[i].UnSetBlinking();
        }

        targetDice.Clear();
        targetTile.Clear();
        selectedDice.UnSetFrameBlinking();
        selectedDice = null;
        targetXY = (-1, -1);
    }

    private void BlinkTargets()
    {
        int tileCount = targetTile.Count;
        int diceCount = targetDice.Count;

        for (int i = 0; i < tileCount; i++)
        {
            targetTile[i].SetBlinking();
        }

        for (int i = 0; i < diceCount; i++)
        {
            targetDice[i].SetBlinking();
        }
    }

    private IEnumerator attackCo()
    {
        selectedDice.RunAttackAnimation(selectedDice.GetDiceType());
        float time = 0;
        bool enterHalf = true;

        while (true)
        {
            time += Time.deltaTime * 4.5f;

            if (time >= 0.5f && enterHalf)
            {
                //�ǰ� ������ ���
                //�÷��̾ ȥ�� ���� ��� Ÿ�� ���� ����
                if (stageManager.IsPlayerSolo())
                {
                    int tileCount = targetTile.Count;
                    //������ŭ �� ü�� ����
                }

                int diceCount = targetDice.Count;
                for (int i = 0; i < diceCount; i++)
                {
                    //������ ��� ���ݷ�*�ֻ��� �� - ����/2
                    targetDice[i].Hurt(Mathf.Clamp((selectedDice.GetDamage() * selectedDice.GetCurrentNumber().c) - (targetDice[i].GetDefense() * (7 - targetDice[i].GetCurrentNumber().c) * 0.5f), 0, 20) * DebugMode.Instance.MultiplyDMG);
                }
                enterHalf = false;
            }

            if (time >= 1f)
                break;

            yield return new WaitForEndOfFrame();
        }

        InitTargets();
        StartCoroutine(waitDestroy());
        yield break;
    }

    /// <summary>
    /// �ı��� �ֻ����� �ִ� ��찡 �����ϱ� ������ 2�ʰ��� ���� �ð��� ����
    /// </summary>
    /// <returns></returns>
    private IEnumerator waitDestroy()
    {
        //�ֻ����� �ı��Ǵ� �ð� : 0.5��
        yield return new WaitForSeconds(0.5f);
        stageManager.NextTurn();
        yield break;
    }
}
