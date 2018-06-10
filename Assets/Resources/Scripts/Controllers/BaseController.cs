using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BaseController : MonoBehaviour
{
    public static BaseController Instance;

    private float scale;
    private int level;
    private int enemiesCount = 27;

    public Texture2D CursorTexture;

    private GameObject menuPanel;
    private GameObject overPanel;
    private GameObject livesPanel;
    private GameObject levelPanel;
    private GameObject victoryPanel;
    private GameObject backgroundPanel;

    private GameObject muteText;
    private GameObject pointsText;

    private GameObject lifeImage;

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

        level = 1;
        Time.timeScale = 1;

        menuPanel = GameObject.Find("PMenu");
        overPanel = GameObject.Find("POver");
        livesPanel = GameObject.Find("PLives");
        levelPanel = GameObject.Find("PLevel");
        victoryPanel = GameObject.Find("PVictory");
        backgroundPanel = GameObject.Find("PBackground");

        muteText = GameObject.Find("TMute");
        pointsText = GameObject.Find("TPoints");

        lifeImage = Resources.Load("Graphics/Prefabs/ILife") as GameObject;

        SetRandomBackground();
        SetDefaultCursor();
    }

    private void SetRandomBackground()
    {
        var resources = Resources.LoadAll("Graphics/Backgrounds").ToList();
        var textures = resources
            .Where(resource => resource is Sprite)
            .Cast<Sprite>()
            .ToList();

        backgroundPanel.GetComponent<Image>().sprite = textures[Random.Range(0, textures.Count)];
    }

    private void SetDefaultCursor()
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
        muteText.GetComponent<Text>().text = SoundController.Instance.AudioIsMute ? "Unmute" : "Mute";
    }

    private void SetPlayerState()
    {
        UpdatePlayerPoints();
        UpdatePlayerLives();
    }

    public void UpdatePlayerPoints()
    {
        pointsText.GetComponent<Text>().text = string.Format("{0:0000000000000}", Player.Instance.PlayerPoints);
    }

    public void UpdatePlayerLives()
    {
        int lives = Player.Instance.PlayerLives;

        foreach (Transform child in livesPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (var i = 0; i < lives; i++)
        {
            Instantiate(lifeImage, livesPanel.transform, false);
        }
    }

    public void UpdateEnemiesCount()
    {
        enemiesCount--;

        if (enemiesCount != 0) return;
        
        if (level == 5)
        {
            Victory();
        }
        else
        {
            level++;
            SetLevelState();
        }
    }

    private void SetLevelState()
    {
        StartCoroutine(SetLevelCoroutine());
    }

    private IEnumerator SetLevelCoroutine()
    {
        Show(true, levelPanel.GetComponent<RectTransform>());
        levelPanel.GetComponentInChildren<Text>().text = string.Format("Wave {0}", level);

        yield return new WaitForSeconds(1);

        Show(false, levelPanel.GetComponent<RectTransform>());
        SetRandomBackground();
        StartNormalLevel();
    }

    private void StartNormalLevel()
    {
        var selected = Random.Range(0, 3);
        var startingPosition = new Vector3(0, 4, 0);
        var startingRotation = Enemies[selected].transform.rotation;

        var instance = Instantiate(Enemies[selected], startingPosition, startingRotation);
        instance.name = "Enemies";
        enemiesCount = 27;
    }

    public void ToggleMenu()
    {
        var menuRectTransform = menuPanel.GetComponent<RectTransform>();

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

    private static void Show(bool show, Transform transform)
    {
        var visibleScale = new Vector3(1, 1, 1);
        var invisibleScale = new Vector3(0, 0, 0);

        transform.localScale = show ? visibleScale : invisibleScale;

        transform
            .Cast<Transform>()
            .Where(c => c.GetComponent<Button>() != null)
            .ToList()
            .ForEach(c => c.GetComponent<Button>().interactable = show);
    }

    private static bool IsVisible(Transform transform)
    {
        var visibleScale = new Vector3(1, 1, 1);

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
        Show(true, overPanel.GetComponent<RectTransform>());
    }

    private void Victory()
    {
        Cursor.visible = true;
        Show(true, victoryPanel.GetComponent<RectTransform>());
    }

    public void PauseGame()
    {
        scale = Time.timeScale;
        Time.timeScale = 0;
    }

    public void UnPauseGame()
    {
        Time.timeScale = scale;
    }
}
