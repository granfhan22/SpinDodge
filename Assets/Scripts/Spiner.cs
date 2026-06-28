using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


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
    public float skillCooldown;
    public float fadeAmount;

    [Header("Health")]
    public int maxHealth ;
    public Slider HealthSlider;

    [Header("Screen Shake")]
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.3f;

    [Header("Death")]
    public float deathShakeDuration = 0.6f;
    public float deathShakeMagnitude = 0.6f;

    [Header("Spinner Selection")]
    [SerializeField] private SpinnerData[] spinnerDataList;

    [Header("Invincibility")]
    public float invincibilityDuration = 2f;
    public float invincibilityBlinkInterval = 0.1f;

    protected Vector2 direction = Vector2.right; 
    protected float speedRatio;

    bool skillReady = true;
    float cooldownTimer;
    Color originalColor;
    Color fadedColor;

    SpinnerData currentData;
    GameObject shieldObject;
    [SerializeField] private GameObject shieldPrefab;
    bool isTeleportInvincible;

    protected int currentHealth;
    protected bool isDead;
    protected bool isInvincible;
    Vector3 cameraBasePos;
    Coroutine shakeCoroutine;
    Coroutine invincibilityCoroutine;
    SpriteRenderer visualRenderer;
    int bulletLayer;

    protected virtual void Awake()
    {
        visualRenderer = GetComponent<SpriteRenderer>();
        bulletLayer = LayerMask.NameToLayer("Bullet");

        ApplySpinnerData();
        SetupShieldIfNeeded();

        if (visualRenderer != null)
        {
            originalColor = visualRenderer.color;
            fadedColor = Color.Lerp(originalColor, Color.gray, fadeAmount);
            fadedColor.a = originalColor.a;
        }

        currentHealth = maxHealth;
        HealthSlider.maxValue = maxHealth;
        HealthSlider.value = currentHealth;
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

        HandleSkillCooldown();
    }

    private void ApplySpinnerData()
    {
        if (spinnerDataList == null || spinnerDataList.Length == 0) return;
        int index = Mathf.Clamp(SpinnerSelection.SelectedIndex, 0, spinnerDataList.Length - 1);
        SpinnerData d = spinnerDataList[index];
        if (d == null) return;

        currentData = d;
        minSpinSpeed = d.minSpinSpeed;
        maxSpinSpeed = d.maxSpinSpeed;
        minMoveSpeed = d.minMoveSpeed;
        maxMoveSpeed = d.maxMoveSpeed;
        acceleration = d.acceleration;
        deceleration = d.deceleration;
        reverseThreshold = d.reverseThreshold;
        idleDeceleration = d.idleDeceleration;
        skillCooldown = d.skillCooldown;
        fadeAmount = d.fadeAmount;
        maxHealth = d.maxHealth;

        if (visualRenderer != null)
        {
            if (d.icon != null) visualRenderer.sprite = d.icon;
            visualRenderer.color = Color.white;
        }
    }

    private void SetupShieldIfNeeded()
    {
        if (currentData == null || currentData.skillType != SkillType.Shield || shieldPrefab == null) return;
        shieldObject = Instantiate(shieldPrefab, transform);
        shieldObject.transform.localPosition = Vector3.zero;
        shieldObject.transform.localScale = Vector3.one * currentData.shieldRadius;
        shieldObject.SetActive(false);
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

        float moveSpeed = Mathf.Lerp(minMoveSpeed, maxMoveSpeed, speedRatio);
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }

    void ApplySpin()
    {
        float spinSpeed = Mathf.Lerp(minSpinSpeed, maxSpinSpeed, speedRatio);
        transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
    }

    // Skill button
    protected virtual void UseSkill()
    {
        switch (currentData?.skillType)
        {
            case SkillType.Shield: UseShield();   break;
            default:               UseTeleport(); break;
        }
        skillReady = false;
        cooldownTimer = skillCooldown;
        if (visualRenderer != null) visualRenderer.color = fadedColor;
    }

    void UseShield()
    {
        AudioManager.Instance?.ShieldSkill();
        if (shieldObject == null) return;
        shieldObject.SetActive(true);
        StartCoroutine(ShieldRoutine());
    }

    IEnumerator ShieldRoutine()
    {
        float duration = currentData != null ? currentData.shieldDuration : 2f;
        yield return new WaitForSecondsRealtime(duration);
        if (shieldObject != null) shieldObject.SetActive(false);
    }

    void UseTeleport()
    {
        AudioManager.Instance?.BlinkSkill();
        float range = currentData != null ? currentData.teleportRange : 5f;
        Vector3 dir = GetMouseDirection();
        Vector3 target = transform.position + dir * range;

        Bounds bounds = BounderArea.GetInnerBounds(Camera.main);
        target.x = Mathf.Clamp(target.x, bounds.min.x, bounds.max.x);
        target.y = Mathf.Clamp(target.y, bounds.min.y, bounds.max.y);
        transform.position = target;

        StartCoroutine(TeleportInvincibilityRoutine());
    }

    IEnumerator TeleportInvincibilityRoutine()
    {
        isTeleportInvincible = true;
        yield return new WaitForSeconds(0.3f);
        isTeleportInvincible = false;
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

    // Cho phép đạn/event ngoài (Bullet, EkeProjectile, BoxDropEvent...) gây dame lên player.
    public bool ApplyDamage(int amount = 1) => TakeDamage(amount);

    protected virtual bool TakeDamage(int amount = 1)
    {
        if (isDead || isInvincible || isTeleportInvincible) return false;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        AudioManager.Instance?.PlayPlayerDamage();

        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ScreenShake(shakeDuration, shakeMagnitude));
        HealthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
            return true;
        }

        if (invincibilityCoroutine != null) StopCoroutine(invincibilityCoroutine);
        invincibilityCoroutine = StartCoroutine(InvincibilityRoutine());
        return true;
    }


    IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        if (bulletLayer >= 0) Physics2D.IgnoreLayerCollision(gameObject.layer, bulletLayer, true);

        float elapsed = 0f;
        while (elapsed < invincibilityDuration)
        {
            if (visualRenderer != null) visualRenderer.enabled = !visualRenderer.enabled;
            yield return new WaitForSeconds(invincibilityBlinkInterval);
            elapsed += invincibilityBlinkInterval;
        }

        if (visualRenderer != null) visualRenderer.enabled = true;
        if (bulletLayer >= 0) Physics2D.IgnoreLayerCollision(gameObject.layer, bulletLayer, false);
        isInvincible = false;
        invincibilityCoroutine = null;
    }

    //what happen when u ded;
    protected virtual void Die()
    {
        isDead = true;

        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        if (visualRenderer != null) visualRenderer.enabled = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        yield return StartCoroutine(ScreenShake(deathShakeDuration, deathShakeMagnitude));

        if (GameManage.Instance != null) GameManage.Instance.ShowLoseScreen();
    }

    IEnumerator ScreenShake(float duration, float magnitude)
    {
        if (Camera.main == null) yield break;
        Transform cam = Camera.main.transform;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            Vector2 offset = Random.insideUnitCircle * magnitude;
            cam.localPosition = cameraBasePos + new Vector3(offset.x, offset.y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.localPosition = cameraBasePos;
        shakeCoroutine = null;
    }
}