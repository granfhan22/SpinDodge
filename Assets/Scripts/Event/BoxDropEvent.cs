using System.Collections;
using UnityEngine;

// Event kiểu "bàn tay" trong No Humanity: bóng xuất hiện ở vị trí ngẫu nhiên trong màn hình
// và co nhỏ dần trong warningDuration giây, sau đó hộp bút rơi xuống đúng vị trí đó kèm
// rung màn hình, gây dame cho player nếu đang đứng trong damageRadius lúc rơi.
public class BoxDropEvent : GameEvent
{
    [SerializeField] private GameObject shadowPrefab;
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private Transform player;

    [Header("Timing")]
    [SerializeField] private float warningDuration = 1.5f;
    [SerializeField] private float boxLifetime = 1f;

    [Header("Impact")]
    [SerializeField] private float damageRadius = 1.5f;
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
        Destroy(boxObj, boxLifetime);

        if (player != null && Vector2.Distance(spawnPos, player.position) <= damageRadius)
        {
            Spin playerSpin = player.GetComponent<Spin>();
            if (playerSpin != null) playerSpin.ApplyDamage(damageAmount);
        }

        StartCoroutine(ScreenShake(shakeDuration, shakeMagnitude));
    }

    private Vector2 GetRandomPositionInView()
    {
        Camera cam = Camera.main;
        float height = cam.orthographicSize;
        float width = height * cam.aspect;

        return new Vector2(Random.Range(-width, width), Random.Range(-height, height));
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
