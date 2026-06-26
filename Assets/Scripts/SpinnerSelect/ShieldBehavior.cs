using UnityEngine;

// Gắn trên shield prefab (child của player).
// Phá bullet khi chạm. EkeProjectile dùng OnCollisionEnter2D nên xuyên qua.
// BoxImpact check tag "Player" nên cũng không bị chặn.
public class ShieldBehavior : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Bullet bullet = other.GetComponent<Bullet>();
        if (bullet != null)
            other.gameObject.SetActive(false);
    }
}
