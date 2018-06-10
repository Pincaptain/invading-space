using UnityEngine;

public class Drop : MonoBehaviour
{
    private const float DefaultDropSpeed = 5;

    public float DropSpeed;

    public AudioClip DropObtainClip;

    private void Awake()
    {
        SetDropProperties();
        SetDropAudioProperties();
    }

    private void SetDropProperties()
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        DropSpeed = DropSpeed == 0 ? DefaultDropSpeed : DropSpeed;
    }

    private void SetDropAudioProperties()
    {
        DropObtainClip = DropObtainClip == null ? Resources.Load("Sounds/sfx_drop_obtain") as AudioClip : DropObtainClip;
    }

    private void Update()
    {
        HandleDrop();
    }

    private void HandleDrop()
    {
        transform.Translate(Vector2.down * DropSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Hit(collision);
    }

    private void Hit(Component collision)
    {
        if (!IsPlayer(collision)) return;
        
        Player.Instance.PlayerUpgradeSprite();
        SoundController.Instance.PlayClip(DropObtainClip);

        Destroy(gameObject);
    }

    private static bool IsPlayer(Component collision)
    {
        return collision.CompareTag("Player");
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
