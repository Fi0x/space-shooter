using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        }else{
            instance = this;
        }
    }

    public float backgroundVolume = 1f;
    public float actionVolume = 1f;
    public float eventVolume = 1f;
    public AudioClip backgroundMusic;
    public AudioSource background;
    public AudioSource action;
    public AudioSource eventSource;
    private void Start()
    {
        action.loop = false;
        eventSource.loop = false;
        PlayBackgroundSfx(backgroundMusic, .5f, true);
    }

    public void PlayBackgroundSfx(AudioClip audioClip, float volume, bool loop)
    {
        background.clip = audioClip;
        background.volume = Mathf.Clamp(volume, 0f, 1f) * backgroundVolume;
        background.loop = loop;
        background.Play();
    }

    public void PlayActionSfx(AudioClip audioClip, float volume)
    {
        action.clip = audioClip;
        action.volume = Mathf.Clamp(volume, 0f, 1f) * actionVolume;
        action.PlayOneShot(audioClip, action.volume);
    }

    public void PlayEventSfx(AudioClip audioClip, float volume)
    {
        eventSource.clip = audioClip;
        eventSource.volume = Mathf.Clamp(volume, 0f, 1f) * eventVolume;
        eventSource.PlayOneShot(audioClip, eventSource.volume);
    }
}
