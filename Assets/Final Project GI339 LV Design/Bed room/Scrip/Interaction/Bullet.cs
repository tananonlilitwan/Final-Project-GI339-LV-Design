using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 5f; // อายุของกระสุน (กันลอยไปไกล)

    void Start()
    {
        Destroy(gameObject, lifetime); // ทำลายตัวเองเมื่อครบเวลา
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ถ้าโดน Ghost ให้เรียก OnHitByBullet()
        if (collision.gameObject.CompareTag("Ghost"))
        {
            GhostAI ghost = collision.gameObject.GetComponent<GhostAI>();
            if (ghost != null)
            {
                ghost.OnHitByBullet();
            }
        }

        // ไม่ว่าจะโดนอะไร ก็ทำลายกระสุน
        Destroy(gameObject);
    }
}