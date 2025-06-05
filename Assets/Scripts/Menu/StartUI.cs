using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    [SerializeField]
    private float fadeDuration = 1f; // Duration for the fade effect
    private Image imageLogo;
    private List<TextMeshProUGUI> textList;
    private void Awake()
    {
        imageLogo = GetComponent<Image>();
        textList = new List<TextMeshProUGUI>
        {
            GameObject.Find("Text1").GetComponent<TextMeshProUGUI>(),
            GameObject.Find("Text2").GetComponent<TextMeshProUGUI>()
        };

        imageLogo.color = new Color(1, 1, 1, 0);
        foreach (var text in textList)
        {
            text.color = new Color(0, 0, 0, 0);
        }
    }

    void Start()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(imageLogo.DOFade(1, 1f).SetEase(Ease.InOutSine))
                .Join(textList[0].DOFade(1, 1f).SetEase(Ease.InOutSine))
                .Join(textList[1].DOFade(1, 1f).SetEase(Ease.InOutSine))
                .AppendInterval(fadeDuration)
                .Append(textList[0].DOFade(0, 1f).SetEase(Ease.InOutSine))
                .Join(textList[1].DOFade(0, 1f).SetEase(Ease.InOutSine))
                .Join(imageLogo.DOFade(0, 1f).SetEase(Ease.InOutSine))
                .OnComplete(() => 
                {
                    // AudioManager.Instance.PlaySFX(0);
                    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Menu");
                });
    } 
}