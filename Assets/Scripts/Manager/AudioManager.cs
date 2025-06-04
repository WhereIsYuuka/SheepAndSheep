using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private List<AudioClip> musics, SFXs;
    [SerializeField] private AudioSource musicSource, sfxSource;

    private void Awake()
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

    private void Start() {
        RandomMusic();
    }

    private void PlaySong()
    {
        musicSource.clip = musics[currentMusicIndex];
        musicSource.loop = true;
        musicSource.Play();
    }

    public void NextMusic()
    {
        currentMusicIndex = (currentMusicIndex + 1) % musics.Count;
        PlaySong();
    }

    public void PreMusic()
    {
        currentMusicIndex = (currentMusicIndex - 1 + musics.Count) % musics.Count;
        PlaySong();
    }

    public void RandomMusic()
    {
        currentMusicIndex = Random.Range(0, musics.Count);
        PlaySong();
    }

    public void PlaySFX(int index)
    {
        sfxSource.PlayOneShot(SFXs[index]);
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
