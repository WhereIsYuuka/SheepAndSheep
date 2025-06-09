using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private List<AudioClip> musics, SFXs;
    [SerializeField] private AudioSource musicSource, sfxSource;
    private int currentMusicIndex = 0;
    private float musicVolume = 0.5f;
    public float MusicVolume
    {
        get => musicVolume;
        set
        {
            musicVolume = value;
            if (musicSource != null)
            {
                musicSource.volume = musicVolume;
            }
        }
    }
    private float sfxVolume = 1f;
    public float SFXVolume
    {
        get => sfxVolume;
        set
        {
            sfxVolume = value;
            if (sfxSource != null)
            {
                sfxSource.volume = sfxVolume;
            }
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(AddAudioSourceAsync());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // RandomMusic();
    }

    private void PlaySong()
    {
        musicSource.volume = musicVolume;
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
        sfxSource.volume = sfxVolume;
        sfxSource.PlayOneShot(clip);
    }

    private IEnumerator AddAudioSourceAsync()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
        musics = new List<AudioClip>();
        SFXs = new List<AudioClip>();

        // load audio clips async from resources 
        ResourceRequest musicRequest = Resources.LoadAsync<AudioClip>("Audio/Music");
        yield return musicRequest;
        if (musicRequest.asset is AudioClip musicClip)
        {
            musics.Add(musicClip);
        }
        AudioClip[] musicClips = Resources.LoadAll<AudioClip>("Audio/Music");
        musics.AddRange(musicClips);

        AudioClip[] sfxClips = Resources.LoadAll<AudioClip>("Audio/SFX");
        SFXs.AddRange(sfxClips);
    }
}
