using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class GameLogic : MonoBehaviour
{
    public GameObject cellPrefab;
    [SerializeField]
    private int layer = 3;
    [SerializeField]
    private int row = 3;
    [SerializeField]
    private int col = 3;
    [SerializeField]
    private float offsetSize = 85f;
    [SerializeField]
    private float barOffset = 115f; // Offset for the bar position
    [SerializeField]
    private RectTransform targetRectTransform;
    private Pool<Cell> cellPool;
    private List<Cell> cells = new();
    private bool isFailed;

    void Start()
    {
        transform.localPosition = Vector3.zero; // Reset position to origin
        cellPool = new Pool<Cell>(
        () => Instantiate(cellPrefab, transform).GetComponent<Cell>(),
        (cell) => { cell.gameObject.SetActive(false); },
        layer * row * col
        );
        GenerateCells();
    }

    private void GenerateCells()
    {
        // float centerX = (col - 1) * offsetSize / 2f;
        // float centerY = (row - 1) * offsetSize * 1.2f / 2f;
        // float centerZ = (layer - 1) * offsetSize / 2f / 2f;
        for (int i = 0; i < layer; i++)
        {
            int curRow = row - i;
            int curCol = col - i;
            if (curRow <= 0 || curCol <= 0)
                break; // Skip if no cells to generate in this layer

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
                        // i * offsetSize - centerZ
                        0
                    );
                    cell.name = $"Cell_{i}_{j}_{k}";
                    cell.MouseEnabled = true; // Enable mouse interaction
                    cell.Value = Random.Range(1, 15); // Random value between 1 and 13
                    cell.OnCellClicked.AddListener(() =>
                    {
                        AddCellToBar(cell);
                        // cell.transform.DOLocalMove(targetRectTransform.localPosition, 0.5f)
                        //     .SetEase(Ease.OutBack)
                        //     .OnComplete(() =>
                        //     {
                        //         //TODO: Add the logic to move the cell to the target position
                        //         AddCellToBar(cell);
                        //     });
                    });
                }
            }
        }
    }

/// <summary>
/// Update the position of cells on the bar.
/// </summary>
/// <param name="idx">the index of the cell that will be move</param>
    private void UpdateCellOnBar(int idx = 0)
    {
        for (int i = idx; i < cells.Count; i++)
        {
            var cell = cells[i];
            cell.transform.DOLocalMoveX(
                i * barOffset, 0.5f
            );
        }
    }

    private void AddCellToBar(Cell cell)
    {
        bool isAdd = false;
        cell.transform.SetParent(targetRectTransform);
        for (int i = 0; i < cells.Count; i++)
        {
            Cell c = cells[i];
            if (c.Value == cell.Value)
            {
                if (i < cells.Count - 1)
                {
                    if (cells[i + 1].Value == cell.Value)
                    {
                        UpdateCellOnBar(i + 2);
                        var a = cells[i];
                        var b = cells[i + 1];
                        cells.RemoveRange(i, 2);
                        cell.transform.DOLocalMove(
                            new Vector3(b.transform.localPosition.x + barOffset, 0, 0), 0.5f
                        ).SetEase(Ease.OutBack).OnComplete(() =>
                        {
                            cellPool.ReturnObject(cell);
                            cellPool.ReturnObject(a);
                            cellPool.ReturnObject(b);
                            AudioManager.Instance.PlaySFX(2);
                            UpdateCellOnBar();
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
        cell.MouseEnabled = false; // Disable mouse interaction after adding to bar

        UpdateCellOnBar();

        if (cells.Count >= 7)
        {
            Debug.Log($"Game Over!");
            isFailed = true;
        }
    }
}
