using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Cell : MonoBehaviour
{
    public UnityEvent OnCellClicked = new();
    int layer;
    int row;
    int col;

    bool mouseEnabled = true;
    public bool MouseEnabled
    {
        get => mouseEnabled;
        set => mouseEnabled = value;
    }

    void OnMouseDown()
    {
        if (!MouseEnabled)
            return;
        ChangeSize();
    }

    void OnMouseUp()
    {
        if (!MouseEnabled)
            return;
        OnCellClicked.Invoke();
    }

    void OnMouseExit()
    {
        if (!MouseEnabled)
            return;
        ResetSize();
    }

    private void ChangeSize()
    {
        transform.DOScale(Vector3.one * 1.2f, 0.1f);
    }

    private void ResetSize()
    {
        transform.DOScale(Vector3.one, 0.1f);
    }
}