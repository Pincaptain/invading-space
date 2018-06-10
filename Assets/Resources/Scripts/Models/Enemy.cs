using UnityEngine;

public class Enemy : MonoBehaviour
{
    private const int DefaultEnemyPoints = 115;
    private const float DefaultEnemySpeed = 2;
    private const float DefaultEnemyBoundry = 1;
    private const float DefaultEnemyLaserDifference = 1f;

    public int EnemyPoints;
    public float EnemySpeed;
    private bool enemyIsDead;

    public Vector3 EnemyLeftDirection;
    public Vector3 EnemyRightDirection;
    public Vector3 EnemyCurrentDirection;

    public Animator EnemyAnimator;

    public AudioClip EnemyHitClip;
    public AudioClip EnemyShootClip;
    public AudioClip EnemyDropClip;

    public GameObject EnemyLaser;
    public GameObject EnemyDrop;

    private void Awake()
    {
        SetEnemyProperties();
        SetEnemyVisualProperties();
        SetEnemyAudioProperties();
        SetEnemyComponents();
    }

    private void SetEnemyProperties()
    {
        EnemyPoints = EnemyPoints == 0 ? DefaultEnemyPoints : EnemyPoints;
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        EnemySpeed = EnemySpeed == 0 ? DefaultEnemySpeed : EnemySpeed;
        enemyIsDead = false;
    }

    private void SetEnemyVisualProperties()
    {
        EnemyLeftDirection = new Vector3(transform.position.x - DefaultEnemyBoundry, transform.position.y, transform.position.z);
        EnemyRightDirection = new Vector3(transform.position.x + DefaultEnemyBoundry, transform.position.y, transform.position.z);
        EnemyCurrentDirection = EnemyCurrentDirection == new Vector3(0, 0, 0) ? EnemyLeftDirection : EnemyRightDirection;

        EnemyAnimator = gameObject.GetComponent<Animator>();
    }

    private void SetEnemyAudioProperties()
    {
        EnemyHitClip = EnemyHitClip == null ? Resources.Load("Sounds/sfx_shieldUp") as AudioClip : EnemyHitClip;
        EnemyShootClip = EnemyShootClip == null ? Resources.Load("Sounds/sfx_laser2") as AudioClip : EnemyShootClip;
        EnemyDropClip = EnemyDropClip == null ? Resources.Load("Sounds/sfx_drop") as AudioClip : EnemyDropClip;
    }

    private void SetEnemyComponents()
    {
        EnemyLaser = EnemyLaser == null ? Resources.Load("Graphics/Prefabs/EnemyLaser") as GameObject : EnemyLaser;
        EnemyDrop = EnemyDrop == null ? Resources.Load("Graphics/Prefabs/Drop") as GameObject : EnemyDrop;
    }

    private void Update()
    {
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
        var step = EnemySpeed * Time.deltaTime;

        if (transform.position == EnemyLeftDirection)
        {
            EnemyCurrentDirection = EnemyRightDirection;
        }
        else if (transform.position == EnemyRightDirection)
        {
            EnemyCurrentDirection = EnemyLeftDirection;
        }

        transform.position = Vector3.MoveTowards(transform.position, EnemyCurrentDirection, step);
    }

    private void Shoot()
    {
        if (!CanShoot()) return;

        var startPosition = new Vector3(transform.position.x, transform.position.y - DefaultEnemyLaserDifference, transform.position.z);
        var startRotation = Quaternion.Euler(0, 0, -180);

        SoundController.Instance.PlayClip(EnemyShootClip);

        Instantiate(EnemyLaser, startPosition, startRotation);
    }

    private static bool CanShoot()
    {
        var random = Random.Range(0, 1000);
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        return random <= 1 && Time.timeScale != 0;
    }

    private void Drop()
    {
        if (!CanDrop()) return;
        
        var startPosition = new Vector3(transform.position.x, transform.position.y - DefaultEnemyLaserDifference, transform.position.z);
        var startRotation = Quaternion.Euler(0, 0, 0);

        SoundController.Instance.PlayClip(EnemyDropClip);

        Instantiate(EnemyDrop, startPosition, startRotation);
    }

    private static bool CanDrop()
    {
        var random = Random.Range(0, 10000);
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        return random <= 1 && Time.timeScale != 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Hit(collision);
    }

    private void Hit(Collider2D collision)
    {
        if (!IsLethal(collision)) return;

        Player.Instance.PlayerPoints += EnemyPoints;
        
        BaseController.Instance.UpdatePlayerPoints();
        SoundController.Instance.PlayClip(EnemyHitClip);
        
        EnemyAnimator.Play("Destroy", 0);
        
        enemyIsDead = true;

        Destroy(gameObject, 0.5f);

        BaseController.Instance.UpdateEnemiesCount();
    }

    private bool IsLethal(Component collision)
    {
        return collision.CompareTag("Player") && !enemyIsDead;
    }
}
