using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // Static laser predefined variables
    private static float DEFAULT_LASER_SPEED = 10;

    // Laser properties
    public float LaserSpeed;
    public short LaserSelectedSprite;

    // Laser visual properties
    public SpriteRenderer LaserSpriteRenderer;
    public Sprite[] LaserSprites;

    private void Awake()
    {
        // Set all the properties
        SetLaserProperties();
        SetLaserVisualProperties();
    }

    private void SetLaserProperties()
    {
        // Set default laser properties
        LaserSpeed = LaserSpeed == 0 ? DEFAULT_LASER_SPEED : LaserSpeed;
    }

    private void SetLaserVisualProperties()
    {
        // Set default laser visual properties
        LaserSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void LaserChangeSprite(short sprite)
    {
        if (sprite >= 0 || sprite <= LaserSprites.Length - 1)
        {
            // Change the laser sprite
            LaserSpriteRenderer.sprite = LaserSprites[sprite];
            LaserSelectedSprite = sprite;
        }
    }

    private void Update()
    {
        // Update laser
        HandleLaser();
    }

    private void HandleLaser()
    {
        // Move the laser up the y axis
        transform.Translate(Vector2.up * LaserSpeed * Time.deltaTime);
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
            // Destroy game object
            Destroy(gameObject);
        }
    }

    private bool IsLethal(Collider2D collision)
    {
        // Assert collision is lethal
        return gameObject.tag != collision.tag;
    }

    private void OnBecameInvisible()
    {
        // Assert away from camera and destroy
        Destroy(gameObject);
    }

}
