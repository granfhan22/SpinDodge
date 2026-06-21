using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float deceleration = 4f;

    [Header("Debug")]
    public float currentSpeed; // tốc độ hiện tại

    [Header("Spinning Speed")]
    [SerializeField] private float maxSpinSpeed = 20f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void FixedUpdate()
    {
        Move();
        Spin();

        // cập nhật tốc độ để xem trong Inspector
        currentSpeed = rb.linearVelocity.magnitude;
    }


    void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(horizontal, vertical).normalized;


        // Có input -> lấy đà
        if(input != Vector2.zero)
        {
            rb.linearVelocity += input * acceleration * Time.fixedDeltaTime;


            // giới hạn max speed
            if(rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
        else
        {
            // Không input -> trượt chậm dần
            rb.linearVelocity = Vector2.MoveTowards(
                rb.linearVelocity,
                Vector2.zero,
                deceleration * Time.fixedDeltaTime
            );
        }
    }


    void Spin()
    {
        float speedPercent = currentSpeed / maxSpeed;

        float currentSpinSpeed = speedPercent * maxSpinSpeed;

        transform.Rotate(
            Vector3.forward * currentSpinSpeed
        );
    }
}