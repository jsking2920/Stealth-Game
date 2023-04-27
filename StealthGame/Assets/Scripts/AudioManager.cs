using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager S;

    public List<AudioClip> explosionClips;
    public AudioClip uiSelect;
    public AudioClip uiClick;

    private AudioSource source;

    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioSource backgroundMusicSource;

    private void Awake()
    {
        if (S != null)
        {
            Destroy(gameObject);
        }
        else
        {
            S = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        source = GetComponent<AudioSource>();

        backgroundMusicSource.loop = true;
        backgroundMusicSource.clip = menuMusic;
        backgroundMusicSource.Play();
    }

    public void PlayGameMusic()
    {
        backgroundMusicSource.Stop();
        backgroundMusicSource.clip = gameMusic;
        backgroundMusicSource.Play();
    }

    public void PlayMenuMusic()
    {
        backgroundMusicSource.Stop();
        backgroundMusicSource.clip = menuMusic;
        backgroundMusicSource.Play();
    }

    public void PlayExplosion()
    {
        source.PlayOneShot(explosionClips[Random.Range(0, explosionClips.Count)]);
    }

    public void PlayUISelect()
    {
        source.PlayOneShot(uiSelect);
    }

    public void PlayUIClick()
    {
        source.PlayOneShot(uiClick);
    }
}
