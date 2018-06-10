using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroController : MonoBehaviour
{
    private static IntroController _instance;

    public Texture2D CursorTexture;

    public AudioClip StartSound;
    public AudioClip ExitSound;
    
    private GameObject muteText;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this);
        }

        muteText = GameObject.Find("TMute");

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
        muteText.GetComponent<Text>().text =
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
