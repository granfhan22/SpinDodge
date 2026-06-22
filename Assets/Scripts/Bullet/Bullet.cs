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
    } 

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
