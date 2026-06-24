using System.Collections;
using UnityEngine;

// Event kiểu "bàn tay" trong No Humanity: bóng xuất hiện ở vị trí ngẫu nhiên trong màn hình
// và co nhỏ dần trong warningDuration giây, sau đó hộp bút rơi xuống đúng vị trí đó kèm
// rung màn hình. Damage gây ra bởi va chạm vật lý của box (BoxImpact) với player, không
// phải tính khoảng cách.
public class BoxDropEvent : GameEvent
{
    [SerializeField] private GameObject shadowPrefab;
    [SerializeField] private GameObject boxPrefab;

    [Header("Timing")]
    [SerializeField] private float warningDuration = 1.5f;
    [SerializeField] private float boxLifetime = 1f;

    [Header("Impact")]
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float shakeDuration = 0.25f;
    [SerializeField] private float shakeMagnitude = 0.4f;

    public override IEnumerator Execute()
    {
        Vector2 spawnPos = GetRandomPositionInView();

        GameObject shadowObj = Instantiate(shadowPrefab);
        shadowObj.GetComponent<ShadowIndicator>().Setup(spawnPos, warningDuration);

        yield return new WaitForSeconds(warningDuration);

        Destroy(shadowObj);

        GameObject boxObj = Instantiate(boxPrefab, spawnPos, Quaternion.identity);
        BoxImpact impact = boxObj.GetComponent<BoxImpact>();
        if (impact != null) impact.Initialize(damageAmount);
        Destroy(boxObj, boxLifetime);

        StartCoroutine(ScreenShake(shakeDuration, shakeMagnitude));
    }

    private Vector2 GetRandomPositionInView()
    {
        // Dùng biên map thật (tường Bounder) thay vì chỉ khung nhìn camera, vì map thật
        // có thể rộng hơn camera — nếu chỉ random trong camera, hộp sẽ không bao giờ rơi
        // trúng player khi họ đứng gần biên/tường.
        Bounds area = BounderArea.GetInnerBounds(Camera.main);
        return new Vector2(Random.Range(area.min.x, area.max.x), Random.Range(area.min.y, area.max.y));
    }

    private IEnumerator ScreenShake(float duration, float magnitude)
    {
        Camera cam = Camera.main;
        if (cam == null) yield break;

        Vector3 basePos = cam.transform.localPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            Vector2 offset = Random.insideUnitCircle * magnitude;
            cam.transform.localPosition = basePos + new Vector3(offset.x, offset.y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.transform.localPosition = basePos;
    }
}
