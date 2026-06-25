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
    }
    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    } // Kiểm tra nếu object ra khỏi màn hình sẽ tự động trả về Pool.

    private void OnCollisionEnter2D(Collision2D collision)
    {
<<<<<<< Updated upstream
        if(collision.gameObject.CompareTag("Player"))
=======
        if (collision.gameObject.CompareTag("Player"))
        {
            Spin playerSpin = collision.gameObject.GetComponent<Spin>();
            if (playerSpin != null) playerSpin.ApplyDamage();
            AudioManager.Instance?.PlayBulletHit();
            gameObject.SetActive(false);
        }
        else if (collision.gameObject.CompareTag("Bounder"))
>>>>>>> Stashed changes
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
