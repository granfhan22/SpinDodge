using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rockRb;
    public float throwSpeed { get; private set; } = 2f;
    private Vector2 direction;
   
    void Awake()
    {
        rockRb = GetComponent<Rigidbody2D>();
    }
    public void Initialize(Vector2 targetPosition)
    {
        direction = (targetPosition - (Vector2)transform.position).normalized;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        rockRb.linearVelocity = direction * throwSpeed;
        // Tính toán góc quay của dao dựa trên hướng đi
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }
    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    } // Kiểm tra nếu object ra khỏi màn hình sẽ tự động trả về Pool.

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Bounder"))
        {
            gameObject.SetActive(false);
        }
        
    }

}
