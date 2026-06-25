using UnityEngine;

// "Ê ke" bay thẳng theo hướng đã được báo trước (warning), vừa bay vừa xoay quanh trục Z.
public class EkeProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float moveSpeed;
    private float rotationSpeed;

    private const float lifetime = 6f;

    public void Initialize(Vector2 direction, float moveSpeed, float rotationSpeed)
    {
        this.direction = direction.normalized;
        this.moveSpeed = moveSpeed;
        this.rotationSpeed = rotationSpeed;
        Destroy(gameObject, lifetime);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Spin playerSpin = collision.gameObject.GetComponent<Spin>();
            if (playerSpin != null) playerSpin.ApplyDamage();
            Destroy(gameObject);
        }
    }
}
