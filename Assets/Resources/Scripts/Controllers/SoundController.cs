using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance;

    public bool AudioIsMute;

    public AudioSource [] AudioSources;
    public AudioSource BackgroundSource;
    public AudioSource AudioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }

        AudioSources = GetComponents<AudioSource>();
        BackgroundSource = AudioSources[0];
        AudioSource = AudioSources[1];

        SetAudioState();
    }

    private void SetAudioState()
    {
        bool mute = false;
        bool.TryParse(PlayerPrefs.GetString("Mute"), out mute);

        if (mute)
        {
            MuteClips();
        }
        else
        {
            UnMuteClips();
        }
    }

    public void PlayClip(AudioClip clip)
    {
        AudioSource.clip = clip;
        AudioSource.Play();
    }

    public void StopClip()
    {
        if (AudioSource.clip != null)
        {
            AudioSource.Stop();
        }
    }

    public void PauseClip()
    {
        if (AudioSource.clip != null)
        {
            AudioSource.Pause();
        }
    }

    public void UnPauseClip()
    {
        if (AudioSource.clip != null)
        {
            AudioSource.UnPause();
        }
    }

    public void MuteClips()
    {
        AudioIsMute = true;
        BackgroundSource.mute = true;
        AudioSource.mute = true;

        PlayerPrefs.SetString("Mute", "true");
    }

    public void UnMuteClips()
    {
        AudioIsMute = false;
        BackgroundSource.mute = false;
        AudioSource.mute = false;

        PlayerPrefs.SetString("Mute", "false");
    }

    public float PositionClip(int i)
    {
        if (AudioSources[i].clip == null)
        {
            return -1;
        }

        return AudioSources[i].time;
    }

    public void PositionClip(int i, float position)
    {
        if (AudioSources[i].clip == null)
        {
            return;
        }

        AudioSources[i].time = position;
    }
}
