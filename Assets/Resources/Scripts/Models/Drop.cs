using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{
    // Static player predefined variables
    public static float DEFAULT_DROP_SPEED = 5;

    // Drop properties
    public float DropSpeed;

    // Drop audio properties
    public AudioClip DropObtainClip;

    // Drop visual properties
    public SpriteRenderer DropSpriteRenderer;

    private void Awake()
    {
        SetDropProperties();
        SetDropAudioProperties();
        SetDropVisualProperties();
    }

    private void SetDropProperties()
    {
        // Set default drop properties
        DropSpeed = DropSpeed == 0 ? DEFAULT_DROP_SPEED : DropSpeed;
    }

    private void SetDropAudioProperties()
    {
        // Set default drop audio properties
        DropObtainClip = DropObtainClip == null ? Resources.Load("Sounds/sfx_drop_obtain") as AudioClip : DropObtainClip;
    }

    private void SetDropVisualProperties()
    {
        // Set default drop visual properties
        DropSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Handle drop update
        HandleDrop();
    }

    private void HandleDrop()
    {
        // Move the laser down the y axis
        transform.Translate(Vector2.down * DropSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Hit(collision);
    }

    protected void Hit(Collider2D collision)
    {
        // Assert is player
        if (IsPlayer(collision))
        {
            // Upgrade the player sprite
            Player.Instance.PlayerUpgradeSprite();
            // Play obtain sound
            SoundController.Instance.PlayClip(DropObtainClip);

            // Destroy the gameobject
            Destroy(gameObject);
        }
    }

    private bool IsPlayer(Collider2D collision)
    {
        return collision.tag == "Player";
    }

    private void OnBecameInvisible()
    {
        // Assert away from camera and destroy
        Destroy(gameObject);
    }
}
