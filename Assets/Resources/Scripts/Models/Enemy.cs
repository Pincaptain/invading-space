using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Static enemy predefined variables
    private static int DEFAULT_ENEMY_POINTS = 115;
    private static float DEFAULT_ENEMY_SPEED = 2;
    private static float DEFAULT_ENEMY_BOUNDRY = 1;
    private static float DEFAULT_ENEMY_LASER_DIFFERENCE = 1f;

    // Enempy properties
    public int EnemyPoints;
    public float EnemySpeed;
    private bool EnemyIsDead;

    // Enemy visual properties
    public Vector3 EnemyLeftDirection;
    public Vector3 EnemyRightDirection;
    public Vector3 EnemyCurrentDirection;

    public SpriteRenderer EnemySpriteRenderer;
    public Animator EnemyAnimator;

    // Enemy audio properties
    public AudioClip EnemyHitClip;
    public AudioClip EnemyShootClip;
    public AudioClip EnemyDropClip;

    // Enemy components
    public GameObject EnemyLaser;
    public GameObject EnemyDrop;

    private void Awake()
    {
        // Set default enemy properties
        SetEnemyProperties();
        SetEnemyVisualProperties();
        SetEnemyAudioProperties();
        SetEnemyComponents();
    }

    private void SetEnemyProperties()
    {
        // Set default enemy properties
        EnemyPoints = EnemyPoints == 0 ? DEFAULT_ENEMY_POINTS : EnemyPoints;
        EnemySpeed = EnemySpeed == 0 ? DEFAULT_ENEMY_SPEED : EnemySpeed;
        EnemyIsDead = false;
    }

    private void SetEnemyVisualProperties()
    {
        // Set default enemy visual properties
        EnemyLeftDirection = new Vector3(transform.position.x - DEFAULT_ENEMY_BOUNDRY, transform.position.y, transform.position.z);
        EnemyRightDirection = new Vector3(transform.position.x + DEFAULT_ENEMY_BOUNDRY, transform.position.y, transform.position.z);
        EnemyCurrentDirection = EnemyCurrentDirection == new Vector3(0, 0, 0) ? EnemyLeftDirection : EnemyRightDirection;

        EnemySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        EnemyAnimator = gameObject.GetComponent<Animator>();
    }

    private void SetEnemyAudioProperties()
    {
        // Set enemy default audio properties
        EnemyHitClip = EnemyHitClip == null ? Resources.Load("Sounds/sfx_shieldUp") as AudioClip : EnemyHitClip;
        EnemyShootClip = EnemyShootClip == null ? Resources.Load("Sounds/sfx_laser2") as AudioClip : EnemyShootClip;
        EnemyDropClip = EnemyDropClip == null ? Resources.Load("Sounds/sfx_drop") as AudioClip : EnemyDropClip;
    }

    private void SetEnemyComponents()
    {
        // Set enemy default components
        EnemyLaser = EnemyLaser == null ? Resources.Load("Graphics/Prefabs/EnemyLaser") as GameObject : EnemyLaser;
        EnemyDrop = EnemyDrop == null ? Resources.Load("Graphics/Prefabs/Drop") as GameObject : EnemyDrop;
    }

    private void Update()
    {
        // Update enemy
        HandleEnemy();
    }

    private void HandleEnemy()
    {
        Move();
        Shoot();
        Drop();
    }

    private void Move()
    {
        // Size of single step
        float step = EnemySpeed * Time.deltaTime;

        // Assert enemy in left position
        if (transform.position == EnemyLeftDirection)
        {
            // Set move to right
            EnemyCurrentDirection = EnemyRightDirection;
        }
        // Assert enemy in right position
        else if (transform.position == EnemyRightDirection)
        {
            // Set move to right
            EnemyCurrentDirection = EnemyLeftDirection;
        }

        // Move towards position
        transform.position = Vector3.MoveTowards(transform.position, EnemyCurrentDirection, step);
    }

    private void Shoot()
    {
        // Assert enemy can shoot
        if (CanShoot())
        {
            // Canculate the starting transform
            Vector3 startPosition = new Vector3(transform.position.x, transform.position.y - DEFAULT_ENEMY_LASER_DIFFERENCE, transform.position.z);
            Quaternion startRotation = Quaternion.Euler(0, 0, -180);

            // Play enemy shoot clip
            SoundController.Instance.PlayClip(EnemyShootClip);

            // Shoot
            Instantiate(EnemyLaser, startPosition, startRotation);
        }
    }

    private bool CanShoot()
    {
        int random = Random.Range(0, 1000);
        // Assert enemy can shoot
        return random <= 1 && Time.timeScale != 0;
    }

    private void Drop()
    {
        // Assert enemy can drop
        if (CanDrop())
        {
            // Calculate the starting transform
            Vector3 startPosition = new Vector3(transform.position.x, transform.position.y - DEFAULT_ENEMY_LASER_DIFFERENCE, transform.position.z);
            Quaternion startRotation = Quaternion.Euler(0, 0, 0);

            // Play enemy drop clip
            SoundController.Instance.PlayClip(EnemyDropClip);

            // Instantiate the drop at specified transform
            Instantiate(EnemyDrop, startPosition, startRotation);
        }
    }

    private bool CanDrop()
    {
        int random = Random.Range(0, 10000);
        // Assert enemy can drop
        return random <= 1 && Time.timeScale != 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Hit(collision);
    }

    private void Hit(Collider2D collision)
    {
        // Assert collision is lethal
        if (IsLethal(collision))
        {
            // Increase player points
            Player.Instance.PlayerPoints += EnemyPoints;
            // Update interface
            BaseController.Instance.UpdatePlayerPoints();
            // Play enemy hit clip
            SoundController.Instance.PlayClip(EnemyHitClip);
            // Play enemy destroy animation
            EnemyAnimator.Play("Destroy", 0);
            // Set enemy dead
            EnemyIsDead = true;

            // Destroy enemy game object
            Destroy(gameObject, 0.5f);

            // Update enemies count
            BaseController.Instance.UpdateEnemiesCount();
        }
    }

    private bool IsLethal(Collider2D collision)
    {
        // Assert collision is player
        return collision.tag == "Player" && !EnemyIsDead;
    }
}
