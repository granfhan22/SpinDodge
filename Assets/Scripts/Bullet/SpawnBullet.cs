using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBullet : MonoBehaviour
{
    [SerializeField] private Transform playerPosition; 
    [Header("Camera bound property")]
    [SerializeField] private float topBound, bottomBound, leftBound, rightBound;

    [Header("Batch settings")]
    [SerializeField] private int batchSize = 20; // Change to 20 prefab spawn 
    [SerializeField] private float spawnIntervalSeconds = 2f; // Đổi sang 2s cho nhanh

    [Header("Bounder safety")]
    [SerializeField] private float spawnMargin = 0.4f;

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

        ClampBoundsInsideBounder();

        StartCoroutine(SpawnInterval());
    }

    // Tỉ lệ khung hình camera có thể khiến vùng spawn theo orthographicSize/aspect
    // vượt ra ngoài (hoặc đè lên) tường Bounder cố định trong scene, làm đạn vừa
    // spawn đã chạm tường và bị tắt ngay. Kẹp lại theo biên trong thực tế của tường.
    private void ClampBoundsInsideBounder()
    {
        Bounds inner = BounderArea.GetInnerBounds(Camera.main);

        leftBound = Mathf.Max(leftBound, inner.min.x + spawnMargin);
        rightBound = Mathf.Min(rightBound, inner.max.x - spawnMargin);
        topBound = Mathf.Min(topBound, inner.max.y - spawnMargin);
        bottomBound = Mathf.Max(bottomBound, inner.min.y + spawnMargin);
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