using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player; // ตัวละครที่กล้องต้องติดตาม
    public Transform gun; // ปืนที่จะขยับ
    public float mouseSensitivity = 3f; // ความไวของเมาส์
    public float distanceFromPlayer = 3f; // ระยะห่างจากตัวละคร
    public float heightOffset = 1.5f; // ความสูงของกล้องจากพื้น
    

    private float yaw = 0f; // มุมหมุนของกล้อง
    private float pitch = 0f; // มุมก้มเงยของกล้อง

    void Start()
    {
        // ซ่อนเคอร์เซอร์ของเมาส์
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        // รับค่าการเคลื่อนเมาส์
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // หมุนกล้องตามการเคลื่อนของเมาส์
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -30f, 45f); // จำกัดมุมก้มเงยของกล้อง

        // คำนวณตำแหน่งกล้องให้อยู่ด้านหลังตัวละคร
        Vector3 offset = new Vector3(0, heightOffset, -distanceFromPlayer);
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        transform.position = player.position + rotation * offset;

        // กล้องมองไปที่ตัวละครเสมอ
        transform.LookAt(player.position + Vector3.up * heightOffset);
        
        // 🟢 หมุนปืนขึ้นลงตาม Pitch
        if (gun != null)
        {
            gun.localRotation = Quaternion.Euler(pitch, 0, 0);
        }

    }
}