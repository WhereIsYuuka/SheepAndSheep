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
    public int Value
    {
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
        set
        {
            mouseEnabled = value;
        }
    }

    bool isGray;
    public bool IsGray
    {
        get => isGray;
        set
        {
            isGray = value;
            SetAlpha(!value, true);
        }
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
        ChangeSize();
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

    public void SetAlpha(bool isEnabled, bool isTween = false)
    {
        Image image = GetComponent<Image>();
        if (image == null)
            return;
        Color color = isEnabled ? Color.white : Color.gray;
        if (isTween)
        {
            image.DOKill();
            image.DOColor(color, 0.3f);
        }
        else
        {
            image.color = color;
        }
    }
}