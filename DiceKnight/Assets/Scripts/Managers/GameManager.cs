using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int clearedStage;

    [SerializeField] private List<GameObject> everyPlayerDices;
    [SerializeField] private List<GameObject> everyEnemyDices;
    private StageData selectedStageData;

    //������ �ֻ��� ���
    private Dictionary<bool, List<GameObject>> ownDice = new Dictionary<bool, List<GameObject>>();

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

        LoadOwnDiceList();

        Application.targetFrameRate = 60;
    }
    
    private void AddDice()
    {
        //�׽�Ʈ�� �ֻ��� �߰�
        ownDice[true].Add(everyPlayerDices[0]);
        ownDice[true].Add(everyPlayerDices[1]);
    }

    public void LoadOwnDiceList()
    {
        ownDice.Add(true, new List<GameObject>());
        ownDice.Add(false, new List<GameObject>());

        //������ �ֻ��� ��� �ҷ�����
        //�ҷ��� ������ ������� (��������, �ֻ���prefab����Ʈ)�� ������ ��ųʸ��� ����
        AddDice();
    }

    public void LoadStageDataFromJson(Difficulty _diff)
    {
        //���̵��� �� �ֻ����� ��ġ�� cost�� �ҷ����� ������ ���� ������ ������
        //�����;��� ����
        /*
         * ��� ������ �ڽ�Ʈ�� ��
         * ��ġ ������ ���̽� ��
         * ���� ������ �ֻ���
         * 
         * ���� ���̽� ����
         * �� ���̽��� ��ġ
         * ���� �����ϴ� �ð�(����)�� �ּҰ�
         */

        string stagePath = Path.Combine(Application.dataPath + "/StageDatas", _diff + ".json");

        try
        {
            string stageJson = File.ReadAllText(stagePath);
            selectedStageData = JsonUtility.FromJson<StageData>(stageJson);
        }
        catch
        {
            return;
        }
    }

    public StageData GetStageData()
    {
        return selectedStageData;
    }

    public (int, int) XYFromVec2(Vector2 _vecPos)
    {
        return (Mathf.RoundToInt(_vecPos.x), Mathf.RoundToInt(_vecPos.y));
    }

    public List<GameObject> GetOwnDiceList()
    {
        return ownDice.GetValueOrDefault(true);
    }

    public GameObject GetEnemyDiceAtIndex(int _index)
    {
        return everyEnemyDices[_index];
    }

    public GameObject GetEnemyDiceAtType(DiceType _type)
    {
        for (int i = 0; i < everyEnemyDices.Count; i++)
        {
            if (everyEnemyDices[i].GetComponent<Dice>().GetDiceType() == _type)
            {
                return everyEnemyDices[i];
            }
        }
        return null;
    }
}
