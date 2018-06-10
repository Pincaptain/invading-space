using UnityEngine;

public class Laser : MonoBehaviour {
    
    private const float DefaultLaserSpeed = 10;

    public float LaserSpeed;

    private void Awake() {
        SetLaserProperties();
    }

    private void SetLaserProperties() {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        LaserSpeed = LaserSpeed == 0 ? DefaultLaserSpeed : LaserSpeed;
    }

    private void Update() {
        HandleLaser();
    }

    private void HandleLaser() {
        transform.Translate(Vector2.up * LaserSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Hit(collision);
    }

    private void Hit(Component collision) {
        if (IsLethal(collision)) {
            Destroy(gameObject);
        }
    }

    private bool IsLethal(Component collision) {
        return !gameObject.CompareTag(collision.tag);
    }

    private void OnBecameInvisible() {
        Destroy(gameObject);
    }

}
