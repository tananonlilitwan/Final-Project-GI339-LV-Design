using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public GameObject gun; // อ้างอิงถึงปืน
    public GameObject bulletPrefab; // Prefab ของกระสุน
    public Transform shootingPoint; // ตำแหน่งที่กระสุนจะออกจากปืน
    [SerializeField] public float bulletSpeed; // ความเร็วของกระสุน
    public KeyCode fireKey = KeyCode.Mouse0; // ปุ่มยิง (เมาส์ซ้าย)

    private bool isGunVisible = true; // สถานะของปืน (ซ่อนหรือแสดง)

    private void Update()
    {
        // เช็คการกดปุ่มยิง
        if (Input.GetKeyDown(fireKey))
        {
            Shoot();
        }
    }

    // ฟังก์ชันสำหรับยิงกระสุน
    private void Shoot()
    {
        // สร้างกระสุนจากตำแหน่ง shootingPoint
        GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);

        // เพิ่มแรงให้กระสุน
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootingPoint.forward * bulletSpeed;
        }
    }
}