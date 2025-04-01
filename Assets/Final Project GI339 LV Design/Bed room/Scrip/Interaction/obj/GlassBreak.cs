using System.Collections;
using UnityEngine;

public class GlassBreak : MonoBehaviour
{
    public GameObject glassShardsPrefab; // Prefab เศษกระจก
    public int shardsAmount = 5; // จำนวนเศษกระจกที่แตกออกมา
    public float explosionForce = 5f; // แรงกระจายของเศษกระจก
    public float explosionRadius = 2f; // รัศมีที่เศษกระจกกระจายออก
    public float destroyDelay = 3f; // เวลาที่เศษกระจกจะหายไป

    public GameObject[] glassObjects; //Object ของกระจกสามบาน

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BreakGlass();
        }
    }

    private void BreakGlass()
    {
        // ทำลายกระจกทั้งสามบาน
        foreach (GameObject glassObject in glassObjects)
        {
            if (glassObject != null)
            {
                Destroy(glassObject); // ทำลายกระจก
            }
        }

        // สร้างเศษกระจกและทำให้มันกระจาย
        for (int i = 0; i < shardsAmount; i++)
        {
            GameObject shard = Instantiate(glassShardsPrefab, transform.position, Random.rotation);
            Rigidbody rb = shard.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomDirection = Random.insideUnitSphere; // กระจายแบบสุ่ม
                rb.AddForce(randomDirection * explosionForce, ForceMode.Impulse);
            }

            // ลบเศษกระจกหลังจาก 3 วินาที
            Destroy(shard, destroyDelay);
        }
    }
}