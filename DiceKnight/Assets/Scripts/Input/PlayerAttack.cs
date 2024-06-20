using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : InputAndAction
{
    public static PlayerAttack Instance;

    private StageManager stageManager;

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

    protected override void InputStyle()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, LayerMask.GetMask("Dice"));

            if (hit.collider == null || hit.collider.tag != "Dice")
            {
                if (selectedDice != null)
                    selectedDice.UnSetFrameBlinking();
                InitTargets();
                return;
            }

            selectedDice = hit.collider.GetComponent<Dice>();
            selectedDice.SetFrameBlinking();

            SetTarget();
        }
    }

    protected override void Action()
    {
        //�÷��̾ ȥ�� ���� ��� Ÿ�� ���� ����
        if (stageManager.GetPlayerIsSolo())
        {

        }
        //run Animation (�ڷ�ƾ)
        //�ִϸ��̼��� ���� �� �������� �ְ� ������ ǥ��
        //
        //�ǰݵ� �� �ֻ��� ü�� ����
        //�ǰݵ� ��ü ü�� ����
        //nexturn

        //������ �ִ� �ݺ���
        //int diceCount = targetDice.Count;
        //for (int i = 0; i < diceCount; i++)
        //{

        //}

        //int tileCount = targetTile.Count;
        //for (int i = 0; i < tileCount; i++)
        //{

        //}


    }

    private void SetTarget()
    {
        //�ʿ��� �� �ʱ�ȭ
        InitTargets();

        selectedAttackArea = selectedDice.GetAttackArea();
        selectedXY = stageManager.GetXYFromDice(selectedDice);

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
        if (targetXY == (-1, -1))
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

        //���õ� ���뿡 ���� ���� ���� �� ��� ����
        //setblinkenemy - ���� ��� ��ġ�� �ִ� enemyDices[(x, y)]�� �̹����� ������ �����ϵ��� ����
    }

    private void DoAttack()
    {
        //���� ��� ���� �ȵ� üũ

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
            targetDice[i].UnSetBlinking();
        }

        targetDice.Clear();
        targetTile.Clear();
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

    private void AttackTile()
    {

    }

    private void AttackEnemy()
    {

    }
}
