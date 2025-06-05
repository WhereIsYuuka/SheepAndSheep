using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    private Button startButton;
    private Button exitButton;

    void Awake()
    {
        startButton = GameObject.Find("StartButton").GetComponent<Button>();
        exitButton = GameObject.Find("ExitButton").GetComponent<Button>();
    }

    void Start()
    {
        startButton.onClick.AddListener(() =>
        {
            // AudioManager.Instance.PlaySFX(0);
            SceneManager.LoadSceneAsync("GameScene");
        });
        exitButton.onClick.AddListener(() =>
        {
            // AudioManager.Instance.PlaySFX(0);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Stop playing in the editor
#else
            Application.Quit();
#endif
        });
    }
}