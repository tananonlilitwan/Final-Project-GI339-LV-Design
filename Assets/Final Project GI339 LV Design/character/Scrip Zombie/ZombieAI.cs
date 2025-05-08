using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    [SerializeField] public Transform target;         // เป้าหมาย (Player)
    public float speed = 2f;         // ความเร็วในการเดิน
    public float health = 3f;        // พลังชีวิต

    void Update()
    {
        if (target != null)
        {
            // หาทิศทางไปยังเป้าหมาย
            Vector3 direction = (target.position - transform.position).normalized;

            // เดินเข้าหาเป้าหมาย
            transform.position += direction * speed * Time.deltaTime;

            // หากต้องการให้หันหน้าตามด้วย
            transform.forward = direction;
        }
    }

    public void OnHitByBullet()
    {
        health -= 1f;

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}