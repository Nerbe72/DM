using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class EnemyMove : InputAndAction
{
    public static EnemyMove Instance;

    #region SELECT
    private Dictionary<(int x, int y), Dice> dices;
    private Dictionary<(int x, int y), Dice> players;
    /// <summary>
    /// Ÿ���� ������ ���� ��츦 ������ (EnemyAttack�� �Ͱ� �ݴ�)
    /// </summary>
    private List<Dice> tempSelectedDices = new List<Dice>();
    private Dice selectedDice = null;

    private bool[,] visited;
    #endregion

    #region MOVE
    private List<MoveDirection> movingTo = new List<MoveDirection>();
    private List<(int c, int r, int b)> nextMoveNumber = new List<(int c, int r, int b)>();
    private List<bool> movingChecker = new List<bool>();
    private int movePointer;
    private PathData selectedPath;

    private Coroutine checkMovingCo;
    #endregion
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

        if (!InputManager.TurnActionList.ContainsKey(Turn.EnemyMove))
            InputManager.TurnActionList.Add(Turn.EnemyMove, this);

        turnName = "EnemyMove";
    }

    protected override void PreAction()
    {
        StartCoroutine(waitBeforeSearch(StageManager.Instance.GetStageData().EnemyThinkingTime));
        Init();
        preActionHolder = true;
    }

    //�Է� ��� ���� �ֻ����� ��θ� �����ϴ� �Լ��� ���
    protected override void InputAction()
    {
        //��� ����
        Searching();

        //���� ��� ����
        if (selectedDice == null)
            selectedDice = dices.Values.ToList<Dice>()[Random.Range(0, dices.Count)];

        selectedPath.num = selectedDice.GetNumbers();
        selectedPath.path.Add(stageManager.GetXYFromEnemyDice(selectedDice));

        ////��� ����
        //selectedPath = SearchBestPath(selectedPath, MoveDirection.Stay, selectedDice.GetMovement(), true);
        selectedPath = SearchBestPathBFS(selectedPath, selectedDice.GetMovement());


        //��θ� ���� �̵� ���� �� ���� ����
        //nextmoveNumber, path�� ���κ���, movingTo�� ���� �̵�������� ����
        nextMoveNumber.Add(selectedDice.GetNumbers());
        int pathCount = selectedPath.path.Count;
        for (int i = 0; i < pathCount - 1; i++)
        {
            movingTo.Add(stageManager.GetDirectionFromXY(selectedPath.path[i], selectedPath.path[i + 1]));
            movingChecker.Add(false);
            nextMoveNumber.Add(stageManager.MoveTo(movingTo[i], nextMoveNumber[i]));
        }

        selectedDice.SetFrameBlinking();
        StartCoroutine(waitBeforeMove(stageManager.GetStageData().EnemyThinkingTime));
        inputHolder = true;
    }

    protected override void Action()
    {
        if (selectedPath.path.Count - 1 <= movePointer || movingChecker.Count == 0)
        {
            //�� ����
            if (selectedDice != null)
                stageManager.AddEnemyDiceOnBoard(stageManager.GetTileDataFromXY(false, selectedPath.path[selectedPath.path.Count - 1]), selectedDice);
            PlayerDiceManager.Instance.UnSelectDice();
            StageManager.Instance.NextTurn();
            return;
        }

        if (!movingChecker[movePointer] && checkMovingCo == null)
        {
            movingChecker[movePointer] = true;
            checkMovingCo = StartCoroutine(MovingCo());
        }
    }

    private void Init()
    {
        dices = StageManager.Instance.GetEnemiesOnBoard();
        players = StageManager.Instance.GetPlayersOnBoard();

        visited = new bool[stageManager.GridXSize, stageManager.GridYSize];
        selectedPath = new PathData((0, 0, 0));
        tempSelectedDices.Clear();
        nextMoveNumber.Clear();
        movingChecker.Clear();
        movingTo.Clear();
        selectedDice = null;
        movePointer = 0;

        for (int i = 0; i < stageManager.GridXSize; i++)
        {
            for (int j = 0; j < stageManager.GridYSize; j++)
            {
                visited[i, j] = false;
            }
        }

        System.GC.Collect();
    }

    private void Searching()
    {
        int enemyCount = dices.Count;
        int playerCount = players.Count;
        List<(int x, int y)> diceXs = dices.Keys.ToList<(int x, int y)>();
        List<(int x, int y)> playerXs = players.Keys.ToList<(int x, int y)>();

        for (int e = 0; e < enemyCount; e++)
        {
            for(int p = 0; p < playerCount; p++)
            {
                //Ÿ���� ���� ��츦 ����
                if (diceXs[e].x != playerXs[p].x)
                    tempSelectedDices.Add(dices[diceXs[e]]);
            }
        }

        
        
        if (tempSelectedDices.Count == 0)
            TargetedAll(); //��� �ֻ����� Ÿ���� ���� ���
        else
            TargetedSome(); //�� ��
    }

    //���� ���� ���� ����� ������ ����
    private void TargetedAll()
    {
        List<Dice> tempDices = dices.Values.ToList<Dice>();
        selectedDice = tempDices[0];
        
        for (int i = 1; i < tempDices.Count; i++)
        {
            if (selectedDice.GetCurrentNumber().c > tempDices[i].GetCurrentNumber().c)
            {
                selectedDice = tempDices[i];
            }
        }
    }

    //�÷��̾� �ֻ����� ���� ����� ����� ������ ����
    private void TargetedSome()
    {
        selectedDice = tempSelectedDices[0];

        for (int i = 0; i < tempSelectedDices.Count; i++)
        {
            (int x, int y) tempPos = stageManager.GetXYFromEnemyDice(tempSelectedDices[i]);
            int movement = tempSelectedDices[i].GetMovement();

            for (int j = 0; j < movement; j++)
            {
                if (stageManager.HavePlayerInX(tempPos.x + j))
                {
                    selectedDice = tempSelectedDices[i];
                    return;
                }    

                if (stageManager.HavePlayerInX(tempPos.x - j))
                {
                    selectedDice = tempSelectedDices[i];
                    return;
                }
            }
        }
    }

    private PathData SearchBestPathBFS(PathData _start, int _movement)
    {
        Queue<PathData> queue = new Queue<PathData>();
        queue.Enqueue(_start);

        //������ bestPath�� ����
        PathData bestPath = _start;

        while (queue.Count > 0)
        {
            PathData currentPath = queue.Dequeue();
            (int x, int y) latestPos = currentPath.LatestPath();

            //�ֻ����� �̹� �ִ� ��ġ�� �̵��� ��� ��ŵ
            if (currentPath.path.Count > 1 && stageManager.IsHaveDice(false, latestPos)) continue;

            //�̹� ����ִ��� üũ
            if (currentPath.path.Count > 1)
            {
                bool alreadySearched = false;
                for (int i = 0; i < currentPath.path.Count - 2; i++)
                {
                    if (currentPath.path[i] == latestPos)
                    {
                        alreadySearched = true;
                        break;
                    }
                }
                if (alreadySearched) continue;
            }

            // x, y���� ������ ��� ��� ��ŵ
            if (latestPos.x < 0 || latestPos.x >= stageManager.GridXSize || latestPos.y < 0 || latestPos.y >= stageManager.GridYSize) continue;

            // �̹� �湮�� ��� ��ŵ
            if (visited[latestPos.x, latestPos.y]) continue;

            // �湮 ǥ��
            visited[latestPos.x, latestPos.y] = true;

            // ��ΰ� 1�� ��� ������ targeted�� false�� �����Ͽ� �̵��� ������.
            if (currentPath.path.Count != 1)
                currentPath.targeted = stageManager.HavePlayerInX(latestPos.x);

            // ���� ���� ���ڸ� ���� ��� ����
            if (currentPath.targeted)
            {
                if (!bestPath.targeted || (currentPath.targeted && currentPath.num.c > bestPath.num.c))
                {
                    bestPath = currentPath;
                }
            }
            else
            {
                if (!bestPath.targeted && currentPath.num.c > bestPath.num.c)
                {
                    bestPath = currentPath;
                }
            }

            // �ִ� �̵� Ƚ���� �������� �ʾ����� ���� �̵��� ť�� �߰�
            if (currentPath.path.Count - 1 < _movement)
            {
                queue.Enqueue(addPath(currentPath, MoveDirection.Left));
                queue.Enqueue(addPath(currentPath, MoveDirection.Right));
                queue.Enqueue(addPath(currentPath, MoveDirection.Up));
                queue.Enqueue(addPath(currentPath, MoveDirection.Down));
            }

            visited[latestPos.x, latestPos.y] = false;
        }

        return bestPath;
    }

    //(���) targeted ���¿� ���ڰ� ���� ��η� �����
    //private PathData SearchBestPath(PathData _path, MoveDirection _dir, int _movement, bool _isFirstMove)
    // {
    //    //��� Ż�� Ʈ����
    //    if (_movement == 0)
    //    {
    //        _path.path.RemoveAt(_path.path.Count - 1);
    //        return _path;
    //    }

    //    //�̹� �ֻ����� �ִ� ��ġ�� �̵��� ��� ���� �� ��ȯ
    //    if (_path.path.Count > 1 && stageManager.IsHaveDice(false, _path.LatestPath()))
    //    {
    //        _path.path.RemoveAt(_path.path.Count - 1);
    //        return _path;
    //    }

    //    //�̹� �迭�� Ž���� ��ġ�� �ִ� ��� ���� �� ��ȯ
    //    if (_path.path.Count > 2)
    //    {
    //        for (int i = 0;  i < _path.path.Count - 2; i++)
    //        {
    //            if(_path.path[i] == _path.path[_path.path.Count - 1])
    //            {
    //                _path.path.RemoveAt(_path.path.Count - 1);
    //                return _path;
    //            }    
    //        }
    //    }

    //    //x, y���� ������ ��� ��� ���� �� ��ȯ
    //    if (_path.LatestPath().x < 0 || _path.LatestPath().x >= stageManager.GridXSize ||
    //        _path.LatestPath().y < 0 || _path.LatestPath().y >= stageManager.GridYSize)
    //    {
    //        _path.path.RemoveAt(_path.path.Count - 1);
    //        return _path;
    //    }

    //    //�̹� �湮�� ��� ���� �� ��ȯ
    //    if (visited[_path.LatestPath().x, _path.LatestPath().y])
    //    {
    //        _path.path.RemoveAt(_path.path.Count - 1);
    //        return _path;
    //    }

    //    //�湮 ǥ��
    //    visited[_path.LatestPath().x, _path.LatestPath().y] = true;

    //    //path�� 1�ΰ�� ������ targeted�� false�� �����Ͽ� �̵��� ������.
    //    _path.targeted = stageManager.HavePlayerInX(_path.LatestPath().x);

    //    PathData bestPath = _path;
    //    PathData[] paths = new PathData[4];
    //    paths[1] = SearchBestPath(CreateNewPath(_path, MoveDirection.Up), MoveDirection.Up, _movement - 1, false);
    //    paths[2] = SearchBestPath(CreateNewPath(_path, MoveDirection.Down), MoveDirection.Down, _movement - 1, false);
    //    paths[3] = SearchBestPath(CreateNewPath(_path, MoveDirection.Left), MoveDirection.Left, _movement - 1, false);
    //    paths[4] = SearchBestPath(CreateNewPath(_path, MoveDirection.Right), MoveDirection.Right, _movement - 1, false);

    //    for (int i = 0; i < 4; i++)
    //    {
    //        if (paths[i].targeted && paths[i].num.c > bestPath.num.c)
    //            bestPath = paths[i];
    //    }

    //    if (!bestPath.targeted)
    //    {
    //        for (int i = 0; i < 4; i++)
    //        {
    //            if (paths[i].num.c > bestPath.num.c)
    //                bestPath = paths[i];
    //        }
    //    }

    //    for (int i = 0; i < stageManager.GridXSize; i++)
    //    {
    //        for (int j = 0;  j < stageManager.GridYSize; j++)
    //        {
    //            visited[i, j] = false;
    //        }
    //    }

    //    visited[_path.path[0].x, _path.path[0].y] = true;

    //    return bestPath;
    //}

    private PathData addPath(PathData _path, MoveDirection _dir)
    {
        PathData newPath = new PathData(_path.num, _path.targeted);
        newPath.path = new List<(int x, int y)>(_path.path);
        (int x, int y) newPos = moveTo(newPath.LatestPath(), _dir);

        newPath.path.Add(newPos);
        newPath.num = stageManager.MoveTo(_dir, newPath.num);
        return newPath;
    }

    private (int x, int y) moveTo((int x, int y) _xy, MoveDirection _dir)
    {
        switch (_dir)
        {
            case MoveDirection.Up:
                return (_xy.x, _xy.y + 1);
            case MoveDirection.Down:
                return (_xy.x, _xy.y - 1);
            case MoveDirection.Left:
                return (_xy.x + 1, _xy.y);
            case MoveDirection.Right:
                return (_xy.x - 1, _xy.y);
            default:
                return _xy;
        }
    }

    /// <summary>
    /// ���� ����ϴ� �ð�(���ð�)
    /// </summary>
    /// <returns></returns>
    private IEnumerator waitBeforeSearch(float _wait)
    {
        yield return new WaitForSeconds(Random.Range(1, _wait / 4));
        inputHolder = false;
        yield break;
    }

    private IEnumerator waitBeforeMove(float _wait)
    {
        yield return new WaitForSeconds(Random.Range(1f, _wait / 2));
        actionHolder = false;
        selectedDice.UnSetFrameBlinking();
        yield break;
    }

    private IEnumerator MovingCo()
    {
        selectedDice.RunAnimation(movingTo[movePointer]);
        float time = 0;
        Vector3 fromPos = selectedDice.transform.position;
        Vector3 targetPos = stageManager.EnemyGridPosFromXY(selectedPath.path[movePointer + 1]);

        while (true)
        {
            time += Time.deltaTime * 6.44f;

            selectedDice.transform.localPosition = Vector3.Lerp(fromPos, targetPos, time);

            if (time >= 1f)
                break;

            yield return new WaitForEndOfFrame();
        }

        selectedDice.SetCurrentNumbers(nextMoveNumber[movePointer + 1]);
        selectedDice.SetNumberUI();
        checkMovingCo = null;
        movePointer++;
        yield break;
    }
}
