using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroController : MonoBehaviour
{
    public static IntroController Instance;

    public Texture2D CursorTexture;

    public AudioClip StartSound;
    public AudioClip ExitSound;
    
    private GameObject MuteText;

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

        MuteText = GameObject.Find("TMute");

        SetDefaultCursor();
        SetDefaultAudioClips();
    }

    private void SetDefaultCursor()
    {
        CursorTexture = CursorTexture == null ? Resources.Load("Graphics/Textures/UI/cursor") as Texture2D : CursorTexture;
        Cursor.SetCursor(CursorTexture, Vector2.zero, CursorMode.Auto);
    }

    private void SetDefaultAudioClips()
    {
        StartSound = StartSound == null ? Resources.Load("Sounds/sfx_zap") as AudioClip : StartSound;
        ExitSound = ExitSound == null ? Resources.Load("Sounds/sfx_zap") as AudioClip : ExitSound;
    }

    private void Start()
    {
        SetAudioState();
    }

    private void SetAudioState()
    {
        MuteText.GetComponent<Text>().text =
            SoundController.Instance.AudioIsMute ? "Unmute" : "Mute";
    }

    public void StartGame()
    {
        SoundController.Instance.PlayClip(StartSound);
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }

    public void ExitGame()
    {
        SoundController.Instance.PlayClip(ExitSound);
        Application.Quit();
    }

    public void MuteGame()
    {
        if (SoundController.Instance.AudioIsMute)
        {
            SoundController.Instance.UnMuteClips();
        }
        else
        {
            SoundController.Instance.MuteClips();
        }

        SetAudioState();
    }

}
