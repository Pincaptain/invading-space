using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BaseController : MonoBehaviour
{
    public static BaseController Instance;

    private float Scale;
    private int Level;
    private int EnemiesCount = 27;

    public Texture2D CursorTexture;

    private GameObject MenuPanel;
    private GameObject OverPanel;
    private GameObject LivesPanel;
    private GameObject LevelPanel;
    private GameObject VictoryPanel;
    private GameObject BackgroundPanel;

    private GameObject MuteText;
    private GameObject PointsText;

    private GameObject LifeImage;

    public List<GameObject> Enemies;

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

        Level = 1;
        Time.timeScale = 1;

        MenuPanel = GameObject.Find("PMenu");
        OverPanel = GameObject.Find("POver");
        LivesPanel = GameObject.Find("PLives");
        LevelPanel = GameObject.Find("PLevel");
        VictoryPanel = GameObject.Find("PVictory");
        BackgroundPanel = GameObject.Find("PBackground");

        MuteText = GameObject.Find("TMute");
        PointsText = GameObject.Find("TPoints");

        LifeImage = Resources.Load("Graphics/Prefabs/ILife") as GameObject;

        SetRandomBackground();
        SetDefaultCursor();
    }

    private void SetRandomBackground()
    {
        List<Object> resources = Resources.LoadAll("Graphics/Backgrounds").ToList();
        List<Sprite> textures = new List<Sprite>();

        textures = resources
            .Where(resource => resource is Sprite)
            .Cast<Sprite>()
            .ToList();

        BackgroundPanel.GetComponent<Image>().sprite = textures[Random.Range(0, textures.Count)];
    }

    public void SetDefaultCursor()
    {
        CursorTexture = CursorTexture == null ? Resources.Load("Graphics/Textures/UI/cursor") as Texture2D : CursorTexture;

        Cursor.SetCursor(CursorTexture, Vector2.zero, CursorMode.Auto);
        Cursor.visible = false;
    }

    private void Start()
    {
        SetAudioState();
        SetPlayerState();
        SetLevelState();
    }

    private void SetAudioState()
    {
        MuteText.GetComponent<Text>().text = SoundController.Instance.AudioIsMute ? "Unmute" : "Mute";
    }

    private void SetPlayerState()
    {
        UpdatePlayerPoints();
        UpdatePlayerLives();
    }

    public void UpdatePlayerPoints()
    {
        PointsText.GetComponent<Text>().text = string.Format("{0:0000000000000}", Player.Instance.PlayerPoints);
    }

    public void UpdatePlayerLives()
    {
        int lives = Player.Instance.PlayerLives;

        foreach (Transform child in LivesPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < lives; i++)
        {
            Instantiate(LifeImage, LivesPanel.transform, false);
        }
    }

    public void UpdateEnemiesCount()
    {
        EnemiesCount--;

        if (EnemiesCount == 0)
        {
            if (Level == 5)
            {
                Victory();
            }
            else
            {
                Level++;
                SetLevelState();
            }
        } 
    }

    public void SetLevelState()
    {
        StartCoroutine(SetLevelCoroutine());
    }

    IEnumerator SetLevelCoroutine()
    {
        Show(true, LevelPanel.GetComponent<RectTransform>());
        LevelPanel.GetComponentInChildren<Text>().text = string.Format("Wave {0}", Level);

        yield return new WaitForSeconds(1);

        Show(false, LevelPanel.GetComponent<RectTransform>());
        SetRandomBackground();
        StartNormalLevel();
    }

    public void StartNormalLevel()
    {
        int selected = Random.Range(0, 3);
        Vector3 startingPosition = new Vector3(0, 4, 0);
        Quaternion startingRotation = Enemies[selected].transform.rotation;

        GameObject instance = Instantiate(Enemies[selected], startingPosition, startingRotation);
        instance.name = "Enemies";
        EnemiesCount = 27;
    }

    public void ToggleMenu()
    {
        RectTransform menuRectTransform = MenuPanel.GetComponent<RectTransform>();

        if (!IsVisible(menuRectTransform))
        {
            Cursor.visible = true;

            Show(true, menuRectTransform);
            PauseGame();
        }
        else
        {
            Cursor.visible = false;

            Show(false, menuRectTransform);
            UnPauseGame();
        }
    }

    private void Show(bool show, RectTransform transform)
    {
        Vector3 visibleScale = new Vector3(1, 1, 1);
        Vector3 invisibleScale = new Vector3(0, 0, 0);

        transform.localScale = show ? visibleScale : invisibleScale;

        transform
            .Cast<Transform>()
            .Where(c => c.GetComponent<Button>() != null)
            .ToList()
            .ForEach(c => c.GetComponent<Button>().interactable = show ? true : false);
    }

    private bool IsVisible(RectTransform transform)
    {
        Vector3 visibleScale = new Vector3(1, 1, 1);
        Vector3 invisibleScale = new Vector3(0, 0, 0);

        return transform.localScale == visibleScale;
    }

    public void StartOverGame()
    {
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
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

    public void ExitGame()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void GameOver()
    {
        Cursor.visible = true;
        Show(true, OverPanel.GetComponent<RectTransform>());
    }

    public void Victory()
    {
        Cursor.visible = true;
        Show(true, VictoryPanel.GetComponent<RectTransform>());
    }

    public void PauseGame()
    {
        Scale = Time.timeScale;
        Time.timeScale = 0;
    }

    public void UnPauseGame()
    {
        Time.timeScale = Scale;
    }
}
