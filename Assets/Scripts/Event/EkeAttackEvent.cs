using System.Collections;
using UnityEngine;

// Event kiểu "No Humanity": báo trước bằng WarningIndicator (2 vạch đỏ nhấp nháy),
// sau warningDuration giây thì spawn ê ke bay theo đúng hướng đã báo, vừa bay vừa xoay.
public class EkeAttackEvent : GameEvent
{
    [SerializeField] private GameObject warningPrefab;
    [SerializeField] private GameObject ekePrefab;
    [SerializeField] private Transform player;

    [Header("Timing")]
    [SerializeField] private float warningDuration = 1.5f;

    [Header("Ê ke movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSpeed = 360f;

    public override IEnumerator Execute()
    {
        Vector2 spawnPos = GetRandomEdgePosition();
        Vector2 direction = ((Vector2)player.position - spawnPos).normalized;

        GameObject warningObj = Instantiate(warningPrefab);
        WarningIndicator warning = warningObj.GetComponent<WarningIndicator>();
        warning.Setup(spawnPos, direction);

        yield return new WaitForSeconds(warningDuration);

        Destroy(warningObj);

        GameObject ekeObj = Instantiate(ekePrefab, spawnPos, Quaternion.identity);
        ekeObj.GetComponent<EkeProjectile>().Initialize(direction, moveSpeed, rotationSpeed);
    }

    private Vector2 GetRandomEdgePosition()
    {
        Camera cam = Camera.main;
        float height = cam.orthographicSize;
        float width = height * cam.aspect;

        switch (Random.Range(0, 4))
        {
            case 0: return new Vector2(Random.Range(-width, width), height);
            case 1: return new Vector2(Random.Range(-width, width), -height);
            case 2: return new Vector2(-width, Random.Range(-height, height));
            default: return new Vector2(width, Random.Range(-height, height));
        }
    }
}
