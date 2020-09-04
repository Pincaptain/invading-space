using UnityEngine;

public class Player : MonoBehaviour
{    
    public static Player Instance;

    private const short playerDefaultLives = 4;
    private const float playerDefaultSpeed = 5;
    private const float playerDefaultLaserDifference = 0.5f;

    public short PlayerLives;
    public long PlayerPoints;
    public float PlayerSpeed;
    public short PlayerSelectedSprite;

    public SpriteRenderer PlayerSpriteRenderer;
    public Animator PlayerAnimator;
    public Sprite[] PlayerSprites;

    public AudioClip PlayerHitClip;
    public AudioClip PlayerShootClip;
    public AudioClip PlayerGameOverClip;

    public GameObject PlayerLaser;

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

        SetPlayerProperties();
        SetPlayerVisualProperties();
        SetPlayerAudioProperties();
        SetPlayerComponents();
    }

    private void SetPlayerProperties()
    {
        PlayerLives = PlayerLives == 0 ? playerDefaultLives : PlayerLives;
        PlayerPoints = 0;
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        PlayerSpeed = PlayerSpeed == 0 ? playerDefaultSpeed : PlayerSpeed;
    }

    private void SetPlayerVisualProperties()
    {
        PlayerSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        PlayerAnimator = gameObject.GetComponent<Animator>();
    }

    private void SetPlayerAudioProperties()
    {
        PlayerHitClip = PlayerHitClip == null ? Resources.Load("Sounds/sfx_shieldDown") as AudioClip : PlayerHitClip;
        PlayerShootClip = PlayerShootClip == null ? Resources.Load("Sounds/sfx_laser1") as AudioClip : PlayerShootClip;
        PlayerGameOverClip = PlayerGameOverClip == null ? Resources.Load("Sounds/sfx_lose") as AudioClip : PlayerGameOverClip;
    }

    private void SetPlayerComponents()
    {
        PlayerLaser = PlayerLaser == null ? Resources.Load("Graphics/Prefabs/PlayerLaser") as GameObject : PlayerLaser;
    }

    private void PlayerAddLife()
    {
        if (PlayerLives >= 4) return;
        
        PlayerLives++;
        
        BaseController.Instance.UpdatePlayerLives();
    }

    private void PlayerRemoveLife()
    {
        if (PlayerLives <= 0) return;
        
        PlayerLives--;
        
        BaseController.Instance.UpdatePlayerLives();
    }

    private bool PlayerIsDead()
    {
        return PlayerLives == 0;
    }

    public void PlayerUpgradeSprite()
    {
        if (PlayerSelectedSprite + 1 > PlayerSprites.Length - 1) return;

        PlayerAddLife();

        PlayerSelectedSprite++;
        PlayerSpriteRenderer.sprite = PlayerSprites[PlayerSelectedSprite];
    }

    private void Update()
    {
        HandlePlayer();
        HandlePlayerInput();
    }

    private void HandlePlayer()
    {
        HandlePlayerBound();
    }

    private void HandlePlayerBound()
    {
        var pos = Camera.main.WorldToViewportPoint(transform.position);

        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);

        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    private void HandlePlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            BaseController.Instance.ToggleMenu();
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
            Move(Vector3.left);
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
            Move(Vector3.right);
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
            Move(Vector3.up);
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
            Move(Vector3.down);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            Shoot();
        }
    }

    private void Move(Vector3 vector)
    {
        transform.Translate(vector * PlayerSpeed * Time.deltaTime);
    }

    private void Shoot()
    {
        if (!CanShoot()) return;
        
        var startPosition = new Vector3(transform.position.x, transform.position.y + playerDefaultLaserDifference, 
            transform.position.z);
        var startRotation = transform.rotation;

        SoundController.Instance.PlayClip(PlayerShootClip);

        Instantiate(PlayerLaser, startPosition, startRotation);
    }

    private static bool CanShoot()
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        return Time.timeScale != 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Hit(collision);
    }

    private static bool IsLethal(Component collision)
    {
        return collision.CompareTag("Enemy");
    }

    private void Hit(Component collision)
    {
        if (!IsLethal(collision)) return;
        
        PlayerRemoveLife();

        SoundController.Instance.PlayClip(PlayerHitClip);

        if (!PlayerIsDead()) return;
        
        SoundController.Instance.PlayClip(PlayerGameOverClip);
        
        PlayerAnimator.enabled = true;
        PlayerAnimator.Play("Destroy");

        Destroy(gameObject, 0.5f);
        BaseController.Instance.GameOver();
    }
}
