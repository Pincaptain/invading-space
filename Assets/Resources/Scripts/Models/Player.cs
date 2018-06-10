using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Player instance
    public static Player Instance;

    // Static player predefined variables
    private static short PLAYER_DEFAULT_LIVES = 4;
    private static float PLAYER_DEFAULT_SPEED = 5;
    private static float PLAYER_DEFAULT_LASER_DIFFERENCE = 0.5f;

    // Player properties
    public short PlayerLives;
    public long PlayerPoints;
    public float PlayerSpeed;
    public short PlayerSelectedSprite;

    // Player visual properties
    public SpriteRenderer PlayerSpriteRenderer;
    public Animator PlayerAnimator;
    public Sprite[] PlayerSprites;

    // Player audio properties
    public AudioClip PlayerHitClip;
    public AudioClip PlayerShootClip;
    public AudioClip PlayerGameOverClip;

    // Player components
    public GameObject PlayerLaser;

    private void Awake()
    {
        // Set the player instance
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }

        // Set all the properties  
        SetPlayerProperties();
        SetPlayerVisualProperties();
        SetPlayerAudioProperties();
        SetPlayerComponents();
    }

    private void SetPlayerProperties()
    {
        // Set default player properties
        PlayerLives = PlayerLives == 0 ? PLAYER_DEFAULT_LIVES : PlayerLives;
        PlayerPoints = 0;
        PlayerSpeed = PlayerSpeed == 0 ? PLAYER_DEFAULT_SPEED : PlayerSpeed;
    }

    private void SetPlayerVisualProperties()
    {
        // Set default player visual properties
        PlayerSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        PlayerAnimator = gameObject.GetComponent<Animator>();
    }

    private void SetPlayerAudioProperties()
    {
        // Set default player audio properties
        PlayerHitClip = PlayerHitClip == null ? Resources.Load("Sounds/sfx_shieldDown") as AudioClip : PlayerHitClip;
        PlayerShootClip = PlayerShootClip == null ? Resources.Load("Sounds/sfx_laser1") as AudioClip : PlayerShootClip;
        PlayerGameOverClip = PlayerGameOverClip == null ? Resources.Load("Sounds/sfx_lose") as AudioClip : PlayerGameOverClip;
    }

    private void SetPlayerComponents()
    {
        // Set the player components
        PlayerLaser = PlayerLaser == null ? Resources.Load("Graphics/Prefabs/PlayerLaser") as GameObject : PlayerLaser;
    }

    public void PlayerAddLife()
    {
        // Assert lives less then 4
        if (PlayerLives < 4)
        {
            // Increase lives
            PlayerLives++;
            // Update interface
            BaseController.Instance.UpdatePlayerLives();
        }
    }

    public void PlayerRemoveLife()
    {
        // Assert lives greater then 0
        if (PlayerLives > 0)
        {
            // Decrease lives
            PlayerLives--;
            // Update interface
            BaseController.Instance.UpdatePlayerLives();
        }
    }

    public bool PlayerIsDead()
    {
        // Assert lives equal to 0
        return PlayerLives == 0;
    }

    public void PlayerChangeSprite(short sprite)
    {
        // Assert sprite within bounds
        if (sprite >= 0 || sprite <= PlayerSprites.Length - 1)
        {
            // Change player sprite based on paremeters
            PlayerSpriteRenderer.sprite = PlayerSprites[sprite];
            PlayerSelectedSprite = sprite;
        }
    }

    public void PlayerUpgradeSprite()
    {
        // Assert can upgrade
        if (PlayerSelectedSprite + 1 <= PlayerSprites.Length - 1)
        {
            // Increase the number of lives
            PlayerAddLife();
            // Increase current sprite
            PlayerSelectedSprite++;
            // Change  player sprite
            PlayerSpriteRenderer.sprite = PlayerSprites[PlayerSelectedSprite];
        }
    }

    public void PlayerDowngradeSprite()
    {
        // Assert can downgrade
        if (PlayerSelectedSprite - 1 >= 0)
        {
            // Decrease current sprite
            PlayerSelectedSprite--;
            // Change player sprite
            PlayerSpriteRenderer.sprite = PlayerSprites[PlayerSelectedSprite];
        }
    }

    private void Update()
    {
        // Update player
        HandlePlayer();
        HandlePlayerInput();
    }

    private void HandlePlayer()
    {
        // Update player boudaries
        HandlePlayerBound();
    }

    private void HandlePlayerBound()
    {
        // Get the normalized viewport position
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        // Clamp the values
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);

        // Set the player position
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    private void HandlePlayerInput()
    {
        // Assert escape clicked
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Controller toggle menu
            BaseController.Instance.ToggleMenu();
        }

        // Assert move key clicked
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            Move(Vector3.left);
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            Move(Vector3.right);
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            Move(Vector3.up);
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            Move(Vector3.down);
        }

        // Assert space clicked
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    private void Move(Vector3 vector)
    {
        // Move the player based on vector and speed
        transform.Translate(vector * PlayerSpeed * Time.deltaTime);
    }

    private void Shoot()
    {
        // Assert player can shoot
        if (CanShoot())
        {
            // Laser starting position and rotation
            Vector3 startPosition = new Vector3(transform.position.x, transform.position.y + PLAYER_DEFAULT_LASER_DIFFERENCE, transform.position.z);
            Quaternion startRotation = transform.rotation;

            // Controller play shoot sound
            SoundController.Instance.PlayClip(PlayerShootClip);

            // Fire the laser at position and rotation
            Instantiate(PlayerLaser, startPosition, startRotation);
        } 
    }

    private bool CanShoot()
    {
        // Assert game not paused
        return Time.timeScale != 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Hit(collision);
    }

    private bool IsLethal(Collider2D collision)
    {
        // Assert collision is an enemy
        return collision.tag == "Enemy";
    }

    private void Hit(Collider2D collision)
    {
        // Assert collision lethal
        if (IsLethal(collision))
        {
            // Remove life
            PlayerRemoveLife();

            // Controller play hit sound
            SoundController.Instance.PlayClip(PlayerHitClip);

            // Assert player is dead
            if (PlayerIsDead())
            {
                // Sound controller play game over sound
                SoundController.Instance.PlayClip(PlayerGameOverClip);
                // Animator play destroy animation
                PlayerAnimator.enabled = true;
                PlayerAnimator.Play("Destroy");

                // Destroy game object after 0.5 seconds
                Destroy(gameObject, 0.5f);
                // Update interface
                BaseController.Instance.GameOver();
            }
        }
    }
}
