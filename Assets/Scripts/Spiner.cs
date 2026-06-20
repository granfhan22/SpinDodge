using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class Spin : MonoBehaviour
{
    [Header("Spin")]
    public float minSpinSpeed = 90f;
    public float maxSpinSpeed = 720f;

    [Header("Move")]
    public float minMoveSpeed ;
    public float maxMoveSpeed ;
    public float acceleration ;
    public float deceleration ;
    public float reverseThreshold ;
    public float idleDeceleration ;

    [Header("Skill")]
    public float dashSpeed ;
    public float dashDuration;
    public float skillCooldown;
    public float fadeAmount ;

    [Header("Visual")]
    public Transform visual;
    public SpriteRenderer visualRenderer;

    [Header("Health")]
    public int maxHealth ;

    [Header("Screen Shake")]
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.3f;

    protected Vector2 direction = Vector2.right; 
    protected float speedRatio;

    bool dashing;
    float dashTimer;
    Vector3 dashDirection;

    bool skillReady = true;
    float cooldownTimer;
    Color originalColor;
    Color fadedColor;

    protected int currentHealth;
    protected bool isDead;
    Vector3 cameraBasePos;
    Coroutine shakeCoroutine;

    protected virtual void Awake()
    {
        if (visual == null) visual = transform;
        if (visualRenderer == null) visualRenderer = visual.GetComponentInChildren<SpriteRenderer>();

        if (visualRenderer != null)
        {
            originalColor = visualRenderer.color;
            fadedColor = Color.Lerp(originalColor, Color.gray, fadeAmount);
            fadedColor.a = originalColor.a;
        }

        currentHealth = maxHealth;
        if (Camera.main != null) cameraBasePos = Camera.main.transform.localPosition;
    }

    protected virtual void Update()
    {
        if (isDead) return;

        Vector2 input = ReadDirectionInput();
        HandleMovement(input);
        ApplySpin();

        if (skillReady && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            UseSkill();

        HandleDash();
        HandleSkillCooldown();
    }

    //MovingInput
    protected virtual Vector2 ReadDirectionInput()
    {
        if (Keyboard.current == null) return Vector2.zero;

        float x = 0f, y = 0f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x += 1f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x -= 1f;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) y += 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) y -= 1f;

        Vector2 raw = new Vector2(x, y);
        return raw.sqrMagnitude > 0.0001f ? raw.normalized : Vector2.zero;
    }

    protected virtual void HandleMovement(Vector2 input)
    {
        bool hasInput = input.sqrMagnitude > 0.0001f;

        if (hasInput)
        {
            bool sameDirection = Vector2.Dot(input, direction) > 0.99f;
            if (sameDirection)
            {
                speedRatio += acceleration * Time.deltaTime;
            }
            else
            {
                // Pressing a different direction bleeds speed off first; only once it drops to the threshold do we actually commit to the new heading.
                speedRatio -= deceleration * Time.deltaTime;
                if (speedRatio <= reverseThreshold)
                    direction = input;
            }
        }
        else
        {
            speedRatio -= idleDeceleration * Time.deltaTime;
        }

        speedRatio = Mathf.Clamp01(speedRatio);

        if (dashing) return;

        float moveSpeed = Mathf.Lerp(minMoveSpeed, maxMoveSpeed, speedRatio);
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }

    void ApplySpin()
    {
        float spinSpeed = Mathf.Lerp(minSpinSpeed, maxSpinSpeed, speedRatio);
        visual.RotateAround(transform.position, Vector3.forward, spinSpeed * Time.deltaTime);
    }

    // Skill button
    protected virtual void UseSkill()
    {
        dashing = true;
        dashTimer = dashDuration;
        dashDirection = GetMouseDirection();

        if (visualRenderer != null)
        {
            skillReady = false;
            cooldownTimer = skillCooldown;
            visualRenderer.color = fadedColor;
        }
    }

    void HandleSkillCooldown()
    {
        if (skillReady || visualRenderer == null) return;

        cooldownTimer -= Time.deltaTime;
        float t = 1f - Mathf.Clamp01(cooldownTimer / skillCooldown);
        visualRenderer.color = Color.Lerp(fadedColor, originalColor, t);

        if (cooldownTimer <= 0f)
        {
            visualRenderer.color = originalColor;
            skillReady = true;
        }
    }
    // Dash Logic
    Vector3 GetMouseDirection()
    { 
        Vector3 fallback = new Vector3(direction.x, direction.y, 0f);
        if (Camera.main == null || Mouse.current == null)
            return fallback;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = transform.position.z;

        Vector3 toMouse = mouseWorld - transform.position;
        return toMouse.sqrMagnitude > 0.0001f ? toMouse.normalized : fallback;
    }

    void HandleDash()
    {
        if (!dashing) return;

        transform.Translate(dashDirection * dashSpeed * Time.deltaTime, Space.World);
        dashTimer -= Time.deltaTime;
        if (dashTimer <= 0f) dashing = false;
    }
    //Take Dame will get camera shake
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet")) TakeDamage();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Bullet")) TakeDamage();
    }

    protected virtual void TakeDamage(int amount = 1)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);

        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ScreenShake());

        if (currentHealth <= 0) Die();
    }

    // Override in child classes for death effects/animations; this default
    // just stops the object from doing anything else.
    protected virtual void Die()
    {
        isDead = true;
        dashing = false;
        gameObject.SetActive(false);
    }

    IEnumerator ScreenShake()
    {
        if (Camera.main == null) yield break;
        Transform cam = Camera.main.transform;

        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            Vector2 offset = Random.insideUnitCircle * shakeMagnitude;
            cam.localPosition = cameraBasePos + new Vector3(offset.x, offset.y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.localPosition = cameraBasePos;
        shakeCoroutine = null;
    }
}
