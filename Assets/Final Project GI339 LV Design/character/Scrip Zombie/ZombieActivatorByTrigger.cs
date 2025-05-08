using UnityEngine;

public class ZombieActivatorByTrigger : MonoBehaviour
{
    [SerializeField] private GameObject zombieToActivate; // ตัว Zombie หรือ Spawner ที่จะเปิด
    [SerializeField] private string targetTag = "Player"; // แท็กของ Player

    private bool hasActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasActivated && other.CompareTag(targetTag))
        {
            if (zombieToActivate != null)
            {
                zombieToActivate.SetActive(true); // เปิด Zombie
                hasActivated = true; // ป้องกันไม่ให้เปิดซ้ำ
            }
        }
    }
}