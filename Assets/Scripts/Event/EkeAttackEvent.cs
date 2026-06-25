using System.Collections;
using UnityEngine;


public class EkeAttackEvent : GameEvent
{
    [SerializeField] private GameObject warningPrefab; // for 2 warning indicator ( 2 red line mb)
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
        AudioManager.Instance?.PlayEkeWarning();

        yield return new WaitForSeconds(warningDuration);

        Destroy(warningObj);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject ekeObj = Instantiate(ekePrefab, spawnPos, Quaternion.Euler(0f, 0f, angle));
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
