using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public int GridXSize = 5;
    public int GridYSize = 4;

    private bool enterStage;
    private bool changeTurn;
    private Turn currentTurn;

    private List<PlayerDiceCointroller> playerDices;
    private List<EnemyDiceController> enemyDices;

    private GameObject[,] playerGrid;
    private GameObject[,] enemyGrid;

    private List<(int, int)> nextMovePosition = new List<(int, int)>();

    [Header("Sprite")]
    [Tooltip("�ֻ��� Ÿ�Ժ� ���� ����")][SerializeField] private List<Sprite> attackAreaSprites;

    [Header("���� ǥ��")]
    [SerializeField] private GameObject statIndicator;
    [SerializeField] private List<TMP_Text> statTexts;
    [SerializeField] private Image attackAreaImage;

    [Header("����UI")]
    [SerializeField] private List<GameObject> tilePrefabs;
    [SerializeField] private GameObject bottomDicePullDowner;
    [SerializeField] private GameObject bottomDiceGroup;
    [SerializeField] private Button bottomDicePrefab;
    public GameObject TurnNamePanel;
    public TMP_Text TurnNameText;

    [Header("�׸���")]
    [Tooltip("�׸��� �ý��� ����(�÷��̾�)")][SerializeField] private GameObject playerGirdParent;
    [Tooltip("�׸��� �ý��� ����(��)")][SerializeField] private GameObject enemyGridParent;

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //private static void asdf()
    //{
       
    //}

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            Destroy(this);
        }

        Init();
        CreateGrid();

    }

    private void Init()
    {
        playerDices = new List<PlayerDiceCointroller>();
        enemyDices = new List<EnemyDiceController>();

        playerGrid = new GameObject[GridXSize, GridYSize];
        enemyGrid = new GameObject[GridXSize, GridYSize];

        enterStage = true;
        changeTurn = false;
        currentTurn = Turn.PlayerSet;

        //���� ���� �ֻ��� ������ ���� ���� �� �ֻ��� ����Ʈ ����
        for (int i = 0; i < 1; i++)
        {
            Button btn = Instantiate(bottomDicePrefab);
            btn.transform.parent = bottomDiceGroup.transform;
        }
        bottomDiceGroup.GetComponent<RectTransform>().offsetMax = new Vector2(-1900 + (bottomDiceGroup.transform.childCount * 100), 0);
        bottomDiceGroup.GetComponent<RectTransform>().offsetMin = new Vector2(20, -40);
        bottomDiceGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(bottomDiceGroup.GetComponent<RectTransform>().anchoredPosition.x, 40);


        CloseStatUI();
    }

    public (int, int) ClampedPos((int x, int y) _pos)
    {
        return (Math.Clamp(_pos.x, 0, GridXSize), Math.Clamp(_pos.y, 0, GridYSize));
    }

    public void CreateGrid()
    {
        for (int yPos = 0; yPos < GridYSize; yPos++)
        {
            for (int xPos = 0; xPos < GridXSize; xPos++)
            {
                GameObject obj = Instantiate(tilePrefabs[0]);
                obj.transform.parent = playerGirdParent.transform;
                obj.transform.localPosition = PositionFromXY((xPos, yPos));
            }
        }

        for (int yPos = 0; yPos < GridYSize; yPos++)
        {
            for (int xPos = 0; xPos < GridXSize; xPos++)
            {
                GameObject obj = Instantiate(tilePrefabs[0]);
                obj.transform.parent = enemyGridParent.transform;
                obj.transform.localPosition = PositionFromXY((xPos, yPos));
            }
        }
    }

    public Vector3 PositionFromXY((int x, int y) _pos)
    {
        return new Vector3(0.5f * (_pos.y - _pos.x), 0.215f * (_pos.x + _pos.y), 0);
    }

    public void AddDiceOnGrid((int x, int y) _pos)
    {
        
    }

    public void OpenStatUI(Dice _selectedDice)
    {
        //0: ü�� 1: �⺻���� 2: �⺻ ���
        //
        statTexts[0].text = _selectedDice.GetDamage().ToString();
        statTexts[1].text = _selectedDice.GetDefense().ToString();
        attackAreaImage.sprite = attackAreaSprites[(int)_selectedDice.GetID()];
        statIndicator.transform.position = _selectedDice.transform.position + new Vector3();
        statIndicator.SetActive(true);
    }

    public void CloseStatUI()
    {
        statIndicator.SetActive(false);
    }

    public void CloseDiceList()
    {
        bottomDicePullDowner.GetComponent<Animator>().SetTrigger("EndSelect");
    }

    public Turn GetTurn()
    {
        return currentTurn;
    }

    public void NextTurn()
    {
        //���� 1ȸ �÷��̾� ���� �� ���� �� �÷��̾� �̵����� �� ���ݱ��� �ݺ�
        currentTurn = (Turn)(((int)currentTurn + (((currentTurn == Turn.EnemyAttack) ? 2 : 1))) % (int)Turn.Count);
        IsChangingTurn(true);
    }

    public bool CheckEnterStage()
    {
        return enterStage;
    }

    public void SetStageEntered()
    {
        enterStage = false;
    }

    public void IsChangingTurn(bool _changed)
    {
        changeTurn = _changed;
    }

    public bool IsChangeTurn()
    {
        return changeTurn;
    }
}
