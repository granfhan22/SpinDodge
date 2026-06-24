using UnityEngine;

// Gắn trên box rơi của BoxDropEvent: khi chạm player thì gây dame trực tiếp qua va chạm
// thay vì BoxDropEvent tự tính khoảng cách.
public class BoxImpact : MonoBehaviour
{
    private int damageAmount = 1;
    private bool hasHit;

    public void Initialize(int damageAmount)
    {
        this.damageAmount = damageAmount;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit || !other.CompareTag("Player")) return;

        Spin playerSpin = other.GetComponent<Spin>();
        if (playerSpin == null) return;

        playerSpin.ApplyDamage(damageAmount);
        hasHit = true;
    }
}
