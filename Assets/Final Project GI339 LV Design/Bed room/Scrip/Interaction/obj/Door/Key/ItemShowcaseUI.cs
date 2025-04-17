using UnityEngine;

public class ItemShowcaseUI : MonoBehaviour
{
    public static ItemShowcaseUI Instance; // Instance สำหรับการเข้าถึงจากที่อื่นๆ

    public GameObject blurPanel; // พื้นหลังเบลอ (ใน Canvas)
    public float distanceFromCamera = 1.5f;  // ระยะห่างเริ่มต้นของไอเท็มจากกล้อง
    public float zoomSpeed = 1f; // ความเร็วในการซูม
    public float minZoom = 0.5f; // ระยะห่างซูมต่ำสุด
    public float maxZoom = 3f; // ระยะห่างซูมสูงสุด

    private GameObject currentItem; // ไอเท็มที่แสดงอยู่ใน Showcase
    private Transform showcaseParent; // ตำแหน่งที่ไอเท็มจะถูกวางบนกล้อง


    void Awake()
    {
        Instance = this;

        // สร้างตำแหน่งของ ShowcaseParent ซึ่งจะตามกล้อง
        showcaseParent = new GameObject("ShowcaseParent").transform;
        showcaseParent.SetParent(Camera.main.transform); // ตั้งให้เป็นลูกของกล้องหลัก
        showcaseParent.localPosition = new Vector3(0, 0, distanceFromCamera); // ตั้งระยะห่างเริ่มต้น
        showcaseParent.localRotation = Quaternion.identity; // ไม่หมุน
    }

    // ฟังก์ชันสำหรับการแสดงไอเท็ม 3D ใน Showcase
    public void ShowItem(GameObject sourceObj)
    {
        // ป้องกันไม่ให้ผู้เล่นเคลื่อนไหวขณะดูไอเท็ม
        PlayerController.Instance?.LockControlByShowcase(true);
        PlayerController.Instance?.SetInventoryState(true); // ✅ บอกว่าเหมือนเปิด inventory
        
        // ส่งสถานะให้กับ CameraController เพื่อเปิดสถานะการดู Inventory
        Camera.main.GetComponent<CameraController>().isInventoryOpen = true; // ตั้งค่า isInventoryOpen ของ CameraController
        
        if (currentItem != null) Destroy(currentItem); // ลบไอเท็มเก่าหากมี

        // สร้างไอเท็มใหม่โดยใช้ Clone ของจริง (ไม่ใช้ Prefab)
        currentItem = Instantiate(sourceObj, showcaseParent.position, Quaternion.identity, showcaseParent);
        Destroy(currentItem.GetComponent<Collider>()); // ลบ Collider เพื่อไม่ให้ชน
        Destroy(currentItem.GetComponent<Rigidbody>()); // ลบ Rigidbody เพื่อไม่ให้ตก

        blurPanel?.SetActive(true); // แสดงพื้นหลังเบลอ
        Time.timeScale = 0f; // หยุดเวลาเพื่อทำให้เกมหยุดขณะดูไอเท็ม
        
        // ซ่อนเคอร์เซอร์เมาส์
        Cursor.visible = false; // ซ่อนเคอร์เซอร์
        Cursor.lockState = CursorLockMode.Locked; // ล็อกเมาส์ในกลางจอ
    }

    // ฟังก์ชันสำหรับซ่อนไอเท็ม
    public void Hide()
    {
        if (currentItem != null) Destroy(currentItem); // ลบไอเท็มเมื่อซ่อน
        blurPanel?.SetActive(false); // ซ่อนพื้นหลังเบลอ
        Time.timeScale = 1f; // เริ่มเวลาใหม่
        
        // แสดงเคอร์เซอร์เมาส์อีกครั้ง
        //Cursor.visible = true; // แสดงเคอร์เซอร์
        Cursor.lockState = CursorLockMode.None; // ปลดล็อกเมาส์
        
        // ปิดสถานะ Inventory และยกเลิกการล็อกการควบคุม
        PlayerController.Instance?.LockControlByShowcase(false);
        PlayerController.Instance?.SetInventoryState(false); // ✅ ปิด inventory

    }

    void Update()
    {
        if (currentItem != null)
        {
            // การหมุนไอเท็มตามการเคลื่อนไหวของเมาส์
            float rotX = Input.GetAxis("Mouse X") * 5f; // หมุนตามแกน X
            float rotY = -Input.GetAxis("Mouse Y") * 5f; // หมุนตามแกน Y
            currentItem.transform.Rotate(Vector3.up, rotX, Space.World); // หมุนรอบแกน Y (ขึ้น/ลง)
            currentItem.transform.Rotate(Vector3.right, rotY, Space.World); // หมุนรอบแกน X (ซ้าย/ขวา)

            // การซูมด้วย Scroll Wheel
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                distanceFromCamera -= scroll * zoomSpeed; // ปรับระยะห่างจากกล้อง
                distanceFromCamera = Mathf.Clamp(distanceFromCamera, minZoom, maxZoom); // จำกัดระยะห่างไม่ให้ต่ำกว่าหรือสูงเกินไป
                showcaseParent.localPosition = new Vector3(0, 0, distanceFromCamera); // อัปเดตตำแหน่ง
            }
            
            // ถ้ากด Escape ซ่อนไอเท็ม
            if (Input.GetKeyDown(KeyCode.Escape)) Hide();  // กด Escape ซ่อนไอเท็ม
            if (Input.GetMouseButtonDown(1)) Hide(); // กดคลิกขวาซ่อนไอเท็ม
        }
    }
}