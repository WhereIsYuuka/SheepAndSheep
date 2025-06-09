using UnityEngine;
using UnityEngine.UI;

public class SliderVolumeUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        SetVolumeSliders();
    }

    private void SetVolumeSliders()
    {
        if (musicSlider != null)
        {
            musicSlider.value = AudioManager.Instance.MusicVolume;
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        if (sfxSlider != null)
        {
            sfxSlider.value = AudioManager.Instance.SFXVolume;
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
    }

    private void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance.MusicVolume = value;
    }

    private void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance.SFXVolume = value;
    }
}