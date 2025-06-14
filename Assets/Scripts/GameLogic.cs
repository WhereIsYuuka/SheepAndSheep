using System.Collections.Generic;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class GameLogic : MonoBehaviour
{
    public static GameLogic Instance { get; private set; }
    public GameObject cellPrefab;
    [Header("=====The parameters of the center cell=====")]
    [ReadOnly(true)] public int layer = 3;
    [SerializeField]
    [ReadOnly(true)] public int row = 3;
    [SerializeField]
    [ReadOnly(true)] public int col = 3;
    [SerializeField]
    private float offsetSize = 85f;
    [SerializeField]
    private float barOffset = 115f; // Offset for the bar position
    [SerializeField]
    private RectTransform targetRectTransform;
    [SerializeField]
    [Header("=====The parameters of the extra cell=====")]
    [ReadOnly(true)] public List<ExtraCellConfig> extraCellConfigs = new();
    private Dictionary<ExtraCellDirection, List<List<Cell>>> extraCells = new();

    private Pool<Cell> cellPool;
    /// <summary>
    /// List of cells on the bar.
    /// </summary>
    private List<Cell> cells = new();
    [SerializeField]
    private Cell[,,] cellArray;
    private bool isFailed;
    public GameObject failPanel;
    public GameObject winPanel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        transform.localPosition = Vector3.zero; // Reset position to origin
        cellPool = new Pool<Cell>(
        () => Instantiate(cellPrefab, transform).GetComponent<Cell>(),
        (cell) => { cell.gameObject.SetActive(false); },
        layer * row * col
        );
        GenerateCells();
        GenerateExtraCells();
        AssignValuesForAllCells();
    }

    private void GenerateCells()
    {
        cellArray = new Cell[layer, row, col];
        // float centerX = (col - 1) * offsetSize / 2f;
        // float centerY = (row - 1) * offsetSize * 1.2f / 2f;
        // float centerZ = (layer - 1) * offsetSize / 2f / 2f;
        for (int i = 0; i < layer; i++)
        {
            int curRow = row - i;
            int curCol = col - i;
            if (curRow <= 0 || curCol <= 0)
                break;

            float offsetX = (curCol - 1) * offsetSize / 2f;
            float offsetY = (curRow - 1) * offsetSize * 1.2f / 2f;

            for (int j = 0; j < curRow; j++)
            {
                for (int k = 0; k < curCol; k++)
                {
                    Cell cell = cellPool.GetObject();
                    cell.gameObject.SetActive(true);
                    cell.transform.SetParent(transform);
                    cell.transform.localPosition = new Vector3(
                        k * offsetSize - offsetX,
                        j * offsetSize * 1.02f - offsetY,
                        0
                    );
                    cell.name = $"Cell_{i}_{j}_{k}";
                    cell.Layer = i;
                    cell.Row = j;
                    cell.Col = k;
                    cellArray[i, j, k] = cell; // Store the cell in the array

                    cell.Value = Random.Range(1, 15);
                    cell.OnCellClicked.AddListener(() =>
                    {
                        AudioManager.Instance.PlaySFX(1);
                        AddCellToBar(cell);
                    });
                }
            }
        }
    }

    /// <summary>
    /// Generate extra cells based on the configurations provided
    /// </summary>
    private void GenerateExtraCells()
    {
        foreach (var config in extraCellConfigs)
        {
            List<Cell> cellList = new List<Cell>();
            Vector2 dir = Vector2.zero;
            dir = config.direction switch
            {
                ExtraCellDirection.Left => Vector2.left,
                ExtraCellDirection.Right => Vector2.right,
                ExtraCellDirection.Up => Vector2.up,
                ExtraCellDirection.Down => Vector2.down,
                _ => Vector2.zero
            };
            for (int i = 0; i < config.count; i++)
            {
                Cell cell = cellPool.GetObject();
                cell.gameObject.SetActive(true);
                cell.transform.SetParent(transform);
                cell.transform.localPosition = new Vector3(
                    config.startPosition.localPosition.x + i * config.offset * dir.x,
                    config.startPosition.localPosition.y + i * config.offset * dir.y,
                    0
                );
                cell.Layer = -1;
                cell.Row = i; // Extra cells are always in the first row
                cell.Col = 0; // Extra cells are always in the first column
                cell.name = $"ExtraCell_{config.direction}_{i}";
                cell.Value = Random.Range(1, 15);
                cell.MouseEnabled = true;
                cell.OnCellClicked.AddListener(() =>
                {
                    AudioManager.Instance.PlaySFX(1);
                    AddCellToBar(cell);
                });
                // cellList.Add(cell);
                cellList.Insert(0, cell); 
            }
            if (!extraCells.ContainsKey(config.direction))
                extraCells[config.direction] = new List<List<Cell>>();
            extraCells[config.direction].Add(cellList);
        }
        UpdateAllCellInteractable();
    }

    /// <summary>
    /// Assign values for all cells
    /// </summary>
    private void AssignValuesForAllCells()
    {
        List<Cell> allCells = new List<Cell>();
        for (int i = 0; i < layer; i++)
        {
            int curRow = row - i;
            int curCol = col - i;
            if (curRow <= 0 || curCol <= 0)
                break;

            for (int j = 0; j < curRow; j++)
                for (int k = 0; k < curCol; k++)
                    if (cellArray[i, j, k] != null)
                        allCells.Add(cellArray[i, j, k]);
        }
        foreach (var extraCellList in extraCells.Values)
        {
            foreach (var extraCell in extraCellList)
            {
                allCells.AddRange(extraCell);
            }
        }

        int totalCells = allCells.Count;
        int group = totalCells / 3;
        int valueTypeCount = Mathf.Min(14, group);

        List<int> values = new List<int>();
        int baseCount = group / valueTypeCount * 3; // base count for each value type
        int remainGroup = group - (baseCount / 3) * valueTypeCount;

        // 先均分
        for (int v = 1; v <= valueTypeCount; v++)
            for (int i = 0; i < baseCount; i++)
                values.Add(v);

        // 剩余组均匀分配
        int vType = 1;
        for (int i = 0; i < remainGroup; i++)
        {
            for (int j = 0; j < 3; j++)
                values.Add(vType);
            vType++;
            if (vType > valueTypeCount) vType = 1;
        }
        // List<int> values = new List<int>();
        // int perType = totalCells / valueTypeCount;
        // perType = perType / 3 * 3;
        // int filled = 0;
        // for (int v = 1; v <= valueTypeCount; v++)
        // {
        //     for (int i = 0; i < perType; i++)
        //     {
        //         values.Add(v);
        //         filled++;
        //         if (filled >= totalCells)
        //             break;
        //     }
        //     if (filled >= totalCells)
        //         break;
        // }

        // int remain = totalCells - filled;
        // int[] typeCount = new int[valueTypeCount + 1];  //TODO
        // for(int i = 1; i <= valueTypeCount; i++)
        // {
        //     typeCount[i] = perType;
        // }
        // int index = 1;
        // while (remain > 0)
        // {
        //     if (typeCount[index] % 3 == 0)
        //     {
        //         values.Add(index);
        //         typeCount[index]++;
        //         remain--;
        //     }
        //     index++;
        //     if (index > valueTypeCount)
        //         index = 1;
        // }

        for (int i = values.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            int temp = values[i];
            values[i] = values[rand];
            values[rand] = temp;
        }

        for (int i = 0; i < allCells.Count; i++)
            allCells[i].Value = values[i];
    }

    /// <summary>
    /// Update the position of cells on the bar.
    /// </summary>
    /// <param name="idx">the index of the cell that will be move</param>
    private void UpdateAllCellOnBar()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            var cell = cells[i];
            cell.transform.DOLocalMoveX(
                i * barOffset, 0.5f
            );
        }
    }

    /// <summary>
    /// Update the position of cells after the cell will be removed.
    /// </summary>
    /// <param name="idx"></param>
    private void UpdatePartCellOnBar(int idx)
    {
        for (int i = idx; i < cells.Count; i++)
        {
            var cell = cells[i];
            cell.transform.DOLocalMoveX(
                (i + 1) * barOffset, 0.5f
            );
        }
    }

    private void AddCellToBar(Cell cell)
    {
        bool isAdd = false;
        cell.transform.SetParent(targetRectTransform);

        if (cell.Layer >= 0 && cell.Layer < layer)
            cellArray[cell.Layer, cell.Row, cell.Col] = null;   // Remove from the array
        else
            foreach (var extraCellList in extraCells.Values)
            {
                foreach (var extraCell in extraCellList)
                {
                    if (extraCell.Contains(cell))
                    {
                        extraCell.Remove(cell); // Remove from the extra cells list
                        break;
                    }
                }
            }
        for (int i = 0; i < cells.Count; i++)
        {
            Cell c = cells[i];
            if (c.Value == cell.Value)
            {
                if (i < cells.Count - 1)
                {
                    if (cells[i + 1].Value == cell.Value)
                    {
                        UpdatePartCellOnBar(i + 2);
                        var a = cells[i];
                        var b = cells[i + 1];
                        cells.RemoveRange(i, 2);
                        UpdateAllCellInteractable();
                        cell.transform.DOScale(cell.transform.localScale * 0.9f, 0.5f)
                        .SetEase(Ease.OutBack);

                        cell.transform.DOLocalMove(
                            new Vector3(b.transform.localPosition.x + barOffset, 0, 0), 0.5f
                        ).SetEase(Ease.OutBack).OnComplete(() =>
                        {
                            cellPool.ReturnObject(cell);
                            cellPool.ReturnObject(a);
                            cellPool.ReturnObject(b);
                            AudioManager.Instance.PlaySFX(2);
                            UpdateAllCellOnBar();
                            CheckWin();
                        });
                        return;
                    }
                    else
                    {
                        cells.Insert(i + 1, cell);
                        isAdd = true;
                        break;
                    }

                }
            }
        }
        if (!isAdd)
        {
            cells.Add(cell);
        }

        cell.transform.DOLocalMove(new Vector3(
            cells.Count * barOffset, 0, 0
        ), 0.5f).SetEase(Ease.OutBack);
        cell.MouseEnabled = false;

        UpdateAllCellOnBar();
        UpdateAllCellInteractable();
        CheckWin();

        // Check if the game is over
        if (cells.Count >= 7)
        {
            failPanel.SetActive(true);
            Debug.Log($"Game Over!");
            isFailed = true;
        }
    }

    public bool IsCellUncovered(Cell cell)
    {
        int l = cell.Layer;
        int r = cell.Row;
        int c = cell.Col;

        int upLayer = l - 1;
        if (upLayer < 0)
            return true;

        int upRow = row - upLayer;
        int upCol = col - upLayer;

        int[,] offsets = new int[,] {
            { 0, 0 },
            // { -1, 0 },   // left
            { 1, 0 },    // right
            // { 0, 1 },  // down
            { 0, 1 },    // up
            { 1, 1 }
        };

        for (int i = 0; i < offsets.GetLength(0); i++)
        {
            int curRow = r + offsets[i, 1];
            int curCol = c + offsets[i, 0];
            if (curRow >= 0 && curRow < upRow && curCol >= 0 && curCol < upCol)
            {
                Cell upCell = cellArray[upLayer, curRow, curCol];
                if (upCell != null && upCell.gameObject.activeSelf)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void UpdateAllCellInteractable()
    {
        for (int i = 0; i < layer; i++)
        {
            int curRow = row - i;
            int curCol = col - i;
            if (curRow <= 0 || curCol <= 0)
                break;
            for (int j = 0; j < curRow; j++)
            {
                for (int k = 0; k < curCol; k++)
                {
                    var cell = cellArray[i, j, k];
                    if (cell != null)
                    {
                        bool uncovered = IsCellUncovered(cell);
                        cell.MouseEnabled = uncovered;
                        cell.IsGray = !uncovered;
                    }
                }
            }
        }
        foreach (var extraCellList in extraCells.Values)
        {
            foreach (var cellList in extraCellList)
            {
                for (int i = 0; i < cellList.Count; i++)
                {
                    cellList[i].MouseEnabled = (i == 0);
                    cellList[i].IsGray = (i != 0);
                }
                // bool uncovered = extraCell == extraCellList[0];
                // extraCell.MouseEnabled = uncovered;
                // extraCell.IsGray = !uncovered;
            }
        }
    }

    /// <summary>
    /// Shuffle the values of the active cells in the game
    /// </summary>
    public void ShuffelCells()
    {
        if (isFailed)
            return;
        List<Cell> activeCells = new List<Cell>();
        for (int i = 0; i < layer; i++)
        {
            int curRow = row - i;
            int curCol = col - i;
            if (curRow <= 0 || curCol <= 0)
                break;
            for (int j = 0; j < curRow; j++)
            {
                for (int k = 0; k < curCol; k++)
                {
                    var cell = cellArray[i, j, k];
                    if (cell != null && cell.gameObject.activeSelf)
                    {
                        activeCells.Add(cell);
                    }
                }
            }
        }
        List<int> values = new List<int>();
        foreach (var cell in activeCells)
            values.Add(cell.Value);

        for (int i = values.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            int temp = values[i];
            values[i] = values[rand];
            values[rand] = temp;
        }

        for (int i = 0; i < activeCells.Count; i++)
        {
            activeCells[i].Value = values[i];
        }
    }

    private void CheckWin()
    {
        for (int i = 0; i < layer; i++)
        {
            int curRow = row - i;
            int curCol = col - i;
            if (curRow <= 0 || curCol <= 0)
                break;
            for (int j = 0; j < curRow; j++)
                for (int k = 0; k < curCol; k++)
                    if (cellArray[i, j, k] != null)
                    {
                        Debug.Log($"Cell {i}_{j}_{k} is not null, game not win yet.");
                        return; 
                    }
        }
        // extra
        foreach (var extraCellList in extraCells.Values)
            foreach (var cellList in extraCellList)
                if (cellList.Count > 0)
                {
                    Debug.Log("Extra cell list is not empty, game not win yet.");
                    return;
                } 

        if(cells.Count > 0)
        {
            Debug.Log("Bar is not empty, game not win yet.");
            return;
        }
        
        if (winPanel != null)
            winPanel.SetActive(true);
        Debug.Log("You Win!");
    }
}

public enum ExtraCellDirection
{
    Left,
    Right,
    Up,
    Down
}
/// <summary>
/// Configuration for extra cells in the game.
/// </summary>
[System.Serializable]
public class ExtraCellConfig
{
    public ExtraCellDirection direction;
    public int count;
    public float offset;
    public RectTransform startPosition;
}
