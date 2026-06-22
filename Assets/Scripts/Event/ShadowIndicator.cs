using UnityEngine;

// Bóng báo trước vị trí vật thể (hộp bút) sắp rơi xuống: co nhỏ dần từ scale gốc về endScale
// và đậm dần lên (lerp màu về endColor) trong shrinkDuration giây, mô phỏng vật thể đang rơi gần lại.
// Set endScale bằng đúng tỉ lệ kích thước thật của box để bóng không biến mất trước khi box xuất hiện.
public class ShadowIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer shadowRenderer;
    [SerializeField] private Color endColor = new Color(0f, 0f, 0f, 1f);
    [SerializeField] private Vector3 endScale = new Vector3(0.5f, 0.5f, 1f);

    private Vector3 startScale;
    private Color startColor;
    private float shrinkDuration;
    private float timer;

    public void Setup(Vector2 position, float shrinkDuration)
    {
        transform.position = position;
        startScale = transform.localScale;
        if (shadowRenderer != null) startColor = shadowRenderer.color;
        this.shrinkDuration = shrinkDuration;
        timer = 0f;
    }

    private void Update()
    {
        if (shrinkDuration <= 0f) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / shrinkDuration);
        transform.localScale = Vector3.Lerp(startScale, endScale, t);
        if (shadowRenderer != null) shadowRenderer.color = Color.Lerp(startColor, endColor, t);
    }
}
