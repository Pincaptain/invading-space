using UnityEngine;

public class SoundController : MonoBehaviour {
    
    public static SoundController Instance;

    public bool AudioIsMute;

    public AudioSource [] AudioSources;
    public AudioSource BackgroundSource;
    public AudioSource AudioSource;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(this);
        }

        AudioSources = GetComponents<AudioSource>();
        BackgroundSource = AudioSources[0];
        AudioSource = AudioSources[1];

        SetAudioState();
    }

    private void SetAudioState() {
        bool mute;
        bool.TryParse(PlayerPrefs.GetString("Mute"), out mute);

        if (mute) {
            MuteClips();
        } else {
            UnMuteClips();
        }
    }

    public void PlayClip(AudioClip clip) {
        AudioSource.clip = clip;
        AudioSource.Play();
    }

    public void MuteClips() {
        AudioIsMute = true;
        BackgroundSource.mute = true;
        AudioSource.mute = true;

        PlayerPrefs.SetString("Mute", "true");
    }

    public void UnMuteClips() {
        AudioIsMute = false;
        BackgroundSource.mute = false;
        AudioSource.mute = false;

        PlayerPrefs.SetString("Mute", "false");
    }

}
