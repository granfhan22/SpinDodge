using System.Collections;
using UnityEngine;

public class SpawnBullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform playerPosition;
    [Header("Camera bound property")]
    [SerializeField] private float topBound, bottomBound, leftBound, rightBound;

    [Header("Batch settings")]
    [SerializeField] private int batchSize = 10;
    [SerializeField] private float spawnIntervalSeconds = 3f;

    private void Start()
    {
        

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("SpawnBullet: Main camera not found.");
            enabled = false;
            return;
        }

        float height = cam.orthographicSize;
        float width = height * cam.aspect;

        leftBound = -width;
        rightBound = width;
        topBound = height;
        bottomBound = -height;

        StartCoroutine(SpawnInterval());
    }

    private IEnumerator SpawnInterval()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.5f);
            SpawnBatch();
            yield return new WaitForSeconds(spawnIntervalSeconds);
        }
    }

    private void SpawnBatch()
    {
        for (int i = 0; i < batchSize; i++)
        {
            Vector2 spawnPos = GetRandomPosition();
            GameObject bulletObj = BulletPool.Instance.GetPoolObject();
            if (bulletObj != null)
            {
                bulletObj.transform.position = spawnPos;
                // Lấy vị trí của player 
                Bullet bullet = bulletObj.GetComponent<Bullet>();
                bullet.Initialize(playerPosition.transform.position);

                bulletObj.SetActive(true);
            }

        }
    }

    private Vector2 GetRandomPosition()
    {
        SpawnPosition side = (SpawnPosition)Random.Range(0, 4);
        switch (side)
        {
            case SpawnPosition.Top:
                return new Vector2(Random.Range(leftBound, rightBound), topBound);
            case SpawnPosition.Bottom:
                return new Vector2(Random.Range(leftBound, rightBound), bottomBound);
            case SpawnPosition.Left:
                return new Vector2(leftBound, Random.Range(bottomBound, topBound));
            case SpawnPosition.Right:
                return new Vector2(rightBound, Random.Range(bottomBound, topBound));
            default:
                return Vector2.zero;
        }
    }

    private enum SpawnPosition
    {
        Top,
        Bottom,
        Left,
        Right
    }
}