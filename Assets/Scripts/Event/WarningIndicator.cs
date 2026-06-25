using UnityEngine;

// 2 vạch đỏ nhấp nháy báo trước hướng mà EkeAttackEvent sẽ bay tới.
public class WarningIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] bars;
    [SerializeField] private float blinkInterval = 0.15f;

    private float blinkTimer;

    public void Setup(Vector2 origin, Vector2 direction)
    {
        transform.position = origin;
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    private void Update()
    {
        blinkTimer += Time.deltaTime;
        if (blinkTimer < blinkInterval) return;

        blinkTimer = 0f;
        foreach (SpriteRenderer bar in bars)
        {
            bar.enabled = !bar.enabled;
        }
    }
}
