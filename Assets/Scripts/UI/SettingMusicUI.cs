using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMusicUI : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> buttons = new();
    private void Start()
    {
        FindBtns();
        SetMethodForButton();
    }

    private void FindBtns()
    {
        buttons.Clear();
        foreach (Transform child in transform)
        {
            if (child.gameObject.name.EndsWith("Btn"))
            {
                buttons.Add(child.gameObject);
            }
        }

        if (buttons.Count == 0)
        {
            Debug.LogWarning("No buttons found in SettingUI.");
        }
    }

    private void SetMethodForButton()
    {
        var actionMap = new Dictionary<string, System.Action>(StringComparer.OrdinalIgnoreCase)
        {
            { "prebtn",    () => AudioManager.Instance.PreMusic() },
            { "randombtn", () => AudioManager.Instance.RandomMusic() },
            { "nextbtn",   () => AudioManager.Instance.NextMusic() }
        };

        foreach (var button in buttons)
        {
            var btn = button.GetComponent<Button>();
            btn.onClick.RemoveAllListeners(); // Clear existing listeners
            if (actionMap.TryGetValue(button.name, out var action))
            {
                button.GetComponent<Button>().onClick.AddListener(() => action());
            }
        }
    }
}