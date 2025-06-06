using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerExitHandler
{
    public UnityEvent OnCellClicked = new();
    int layer;
    public int Layer
    {
        get => layer;
        set => layer = value;
    }
    int row;
    public int Row
    {
        get => row;
        set => row = value;
    }
    int col;
    public int Col
    {
        get => col;
        set => col = value;
    }
    int value;
    public int Value {
        get => value;
        set
        {
            this.value = value;
            GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/BACard/{value}");
        }
    }

    bool mouseEnabled = true;
    public bool MouseEnabled
    {
        get => mouseEnabled;
        set => mouseEnabled = value;
    }

    private void ChangeSize()
    {
        transform.DOScale(Vector3.one * 1.2f, 0.1f);
    }

    private void ResetSize()
    {
        transform.DOScale(Vector3.one, 0.1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!MouseEnabled)
            return;
        OnCellClicked.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!MouseEnabled)
            return;
        ChangeSize();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!MouseEnabled)
            return;
        ResetSize();
    }
}