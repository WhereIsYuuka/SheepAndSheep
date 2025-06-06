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
    private RectTransform targetRectTransform;
    private Pool<Cell> cellPool;
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
                    cell.OnCellClicked.AddListener(() =>
                    {
                        cell.transform.DOLocalMove(targetRectTransform.localPosition, 0.5f)
                            .SetEase(Ease.OutBack)
                            .OnComplete(() =>
                            {
                                // Add the logic to move the cell to the target position
                            });
                    });
                }
            }
        }
    }
}
