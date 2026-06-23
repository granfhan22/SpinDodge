using UnityEngine;

// "Ê ke" bay thẳng theo hướng đã được báo trước (warning), vừa bay vừa xoay quanh trục Z.
// Collider trên object này phải gắn tag "Bullet" để tái dùng logic TakeDamage của Spin.
public class EkeProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float moveSpeed;
    private float rotationSpeed;

    public void Initialize(Vector2 direction, float moveSpeed, float rotationSpeed)
    {
        this.direction = direction.normalized;
        this.moveSpeed = moveSpeed;
        this.rotationSpeed = rotationSpeed;
    }

    private void Update()
    {
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
