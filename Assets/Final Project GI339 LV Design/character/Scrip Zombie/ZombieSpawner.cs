using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;    // พรีแฟบของ Zombie
    public Transform[] spawnPoints;    // จุดเกิดที่เป็นไปได้
    public float spawnInterval = 10f;   // เวลาระหว่างการเกิด

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnZombie();
            timer = 0f;
        }
    }

    void SpawnZombie()
    {
        if (spawnPoints.Length == 0) return;

        // เลือกจุดสุ่มจาก spawnPoints
        int index = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[index];

        // สร้าง Zombie ที่จุดที่เลือก
        Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);
    }
}