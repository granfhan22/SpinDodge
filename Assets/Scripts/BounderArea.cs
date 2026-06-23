using UnityEngine;

// Tính vùng chơi thật (biên trong của 4 tường tag "Bounder") để spawn/random vị trí
// không bị giới hạn nhầm theo khung nhìn camera, vì map thật có thể rộng hơn camera.
public static class BounderArea
{
    public static Bounds GetInnerBounds(Camera fallbackCam)
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Bounder");
        float innerLeft = float.NegativeInfinity, innerRight = float.PositiveInfinity;
        float innerTop = float.PositiveInfinity, innerBottom = float.NegativeInfinity;

        foreach (GameObject wall in walls)
        {
            Collider2D col = wall.GetComponent<Collider2D>();
            if (col == null) continue;

            Bounds b = col.bounds;
            if (b.size.x >= b.size.y)
            {
                if (b.center.y >= 0) innerTop = Mathf.Min(innerTop, b.min.y);
                else innerBottom = Mathf.Max(innerBottom, b.max.y);
            }
            else
            {
                if (b.center.x <= 0) innerLeft = Mathf.Max(innerLeft, b.max.x);
                else innerRight = Mathf.Min(innerRight, b.min.x);
            }
        }

        if (float.IsInfinity(innerLeft) || float.IsInfinity(innerRight) ||
            float.IsInfinity(innerTop) || float.IsInfinity(innerBottom))
        {
            float height = fallbackCam.orthographicSize;
            float width = height * fallbackCam.aspect;
            return new Bounds(Vector3.zero, new Vector3(width * 2f, height * 2f, 0f));
        }

        Vector3 center = new Vector3((innerLeft + innerRight) * 0.5f, (innerTop + innerBottom) * 0.5f, 0f);
        Vector3 size = new Vector3(innerRight - innerLeft, innerTop - innerBottom, 0f);
        return new Bounds(center, size);
    }
}
