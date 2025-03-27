using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    public float moveSpeed = 10f;   // ความเร็วในการเคลื่อนที่
    public float lookSpeed = 2f;    // ความเร็วในการหมุนกล้อง
    public float verticalSpeed = 5f; // ความเร็วในการลอยขึ้น-ลง

    private float yaw = 0f;  // หมุนแกน Y
    private float pitch = 0f; // หมุนแกน X

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // ล็อกเมาส์ไว้กลางจอ
        Cursor.visible = false;
    }

    void Update()
    {
        // --- การควบคุมเมาส์ ---
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f); // จำกัดการก้ม-เงย

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // --- การเคลื่อนที่ด้วย WASD ---
        float moveX = Input.GetAxis("Horizontal"); // A, D
        float moveZ = Input.GetAxis("Vertical");   // W, S

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        transform.position += move * moveSpeed * Time.deltaTime;

        // --- การลอยขึ้น-ลงด้วย Arrow Up/Down ---
        if (Input.GetKey(KeyCode.UpArrow))
            transform.position += Vector3.up * verticalSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.DownArrow))
            transform.position -= Vector3.up * verticalSpeed * Time.deltaTime;

        // --- ปลดล็อกเมาส์เมื่อกด ESC ---
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
