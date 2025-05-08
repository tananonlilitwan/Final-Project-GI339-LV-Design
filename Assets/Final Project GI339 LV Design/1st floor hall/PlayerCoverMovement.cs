using UnityEngine;

public class PlayerCoverMovement : MonoBehaviour
{
    [SerializeField] public float moveSpeed; // ความเร็วในการเคลื่อนที่
    [SerializeField]public float raycastDistance; // ระยะทางการตรวจจับด้วย Raycast
    public LayerMask coverLayer; // เลเยอร์ที่ใช้ในการตรวจสอบ (เช่น "Cover")
    public bool canMove = true;

    private void Update()
    {
        if (!canMove) return;
        // ตรวจสอบว่าผู้เล่นกด E หรือ Q
        if (Input.GetKeyDown(KeyCode.E))
        {
            MoveToCover(Vector3.forward);  // เคลื่อนที่ไปข้างหน้า (หรือคุณอาจจะใช้ตำแหน่งอื่น)
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            MoveToCover(Vector3.back);    // เคลื่อนที่ไปด้านหลัง (หรือคุณอาจจะใช้ตำแหน่งอื่น)
        }
    }

    void MoveToCover(Vector3 direction)
    {
        // ตรวจสอบการชนกับ Cover ด้วย Raycast
        RaycastHit hit;
        Vector3 destination = transform.position + direction; // จุดปลายทางที่ต้องการให้เดินไป

        // ตรวจสอบว่ามีการชนกับ Cover หรือไม่
        if (Physics.Raycast(transform.position, direction, out hit, raycastDistance, coverLayer))
        {
            // ถ้าชนกับ Cover, หลีกเลี่ยงการเดินไปข้างหน้า
            Debug.Log("Cover detected, avoiding obstacle.");
            // ทำการปรับทิศทางให้ Player เดินไปในทิศทางที่ปลอดภัย
            Vector3 avoidDirection = Vector3.Cross(direction, Vector3.up).normalized; // การหลบไปทางซ้ายหรือขวา
            destination = transform.position + avoidDirection;
        }

        // เคลื่อนที่ไปยังตำแหน่งที่คำนวณ
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
    }
}

