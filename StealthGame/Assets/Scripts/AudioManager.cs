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
