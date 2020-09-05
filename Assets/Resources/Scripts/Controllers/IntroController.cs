using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroController : MonoBehaviour
{    
    private static IntroController instance;

    public Texture2D CursorTexture;

    public AudioClip StartSound;
    public AudioClip ExitSound;
    
    private GameObject muteText;
    private GameObject rotationText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }

        muteText = GameObject.Find("TMute");
        rotationText = GameObject.Find("TRotation");

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
        SetRotationState();
    }

    private void SetAudioState()
    {
        muteText.GetComponent<Text>().text =
            SoundController.Instance.AudioIsMute ? "Unmute" : "Mute";
    }

    private void SetRotationState()
    {
        bool rotationState = IntToBool(PlayerPrefs.GetInt("rotation", 0));

        rotationText.GetComponent<Text>().text = 
            rotationState ? "Disable Rotation" : "Enable Rotation";
    }

    private void SetRotationState(bool rotationState)
    {
        rotationText.GetComponent<Text>().text = 
            rotationState ? "Disable Rotation" : "Enable Rotation";
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

    public void AllowRotation()
    {
        int initial = PlayerPrefs.GetInt("rotation", 0);
        int current = initial == 0 ? 1 : 0;

        PlayerPrefs.SetInt("rotation", current);

        SetRotationState(IntToBool(current));
    }

    private bool IntToBool(int value)
    {
        return value != 0;
    }
}
