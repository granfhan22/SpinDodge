using UnityEngine;


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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Spin playerSpin = other.GetComponent<Spin>();
            playerSpin?.ApplyDamage();
        }
    }
}