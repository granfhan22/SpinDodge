using UnityEngine;


public class ShieldBehavior : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Bullet>() != null)
        {
            AudioManager.Instance?.PlayBulletDestroy();
            other.gameObject.SetActive(false);
        }
    }
}