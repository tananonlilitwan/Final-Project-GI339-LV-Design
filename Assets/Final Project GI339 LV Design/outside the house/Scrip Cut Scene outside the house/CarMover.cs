using UnityEngine;

public class CarMover : MonoBehaviour
{
    public Transform[] waypoints; // จุดที่รถจะวิ่งผ่าน (Waypoint)
    public float speed = 3f; // ความเร็วในการเคลื่อนที่ของรถ
    public float rotationSpeed = 2f;      // ความเร็วในการหมุน
    private int index = 0; // ตำแหน่งของ Waypoint ปัจจุบันที่รถจะไป
    private bool isMoving = false; // ตัวแปรเช็คว่ารถกำลังเคลื่อนที่อยู่หรือไม่
    
    [Header("สิ่งที่จะเปิดเมื่อรถถึงจุดสุดท้าย")]
    public GameObject player;         // ตัวละครผู้เล่น
    public GameObject dotHitCanvas;   // UI จุดเล็งยิง
    public Vector3 playerOffset = new Vector3(2f, 0, 0); // ตำแหน่งที่ผู้เล่นจะเกิดจากตำแหน่งรถ

    private bool finished = false;    // ป้องกันไม่ให้ทำซ้ำหลายครั้ง
    
    [Header("กล้อง")]
    public Camera carCamera;         // กล้องที่ติดอยู่กับรถ
    
    [Header("UI โชว์ไอเท็ม")]
    public GameObject panelItemShowcaseUI; // <-- GameObject ของ Panel ที่มี ItemShowcaseUI อยู่

    void Start()
    {
        // ปิด Player และ UI จุดเล็งยิง ตั้งแต่เริ่ม
        if (player != null)
            player.SetActive(false);

        if (dotHitCanvas != null)
            dotHitCanvas.SetActive(false);

        // เปิด Panel ItemShowcaseUI เพื่อป้องกัน Null
        if (panelItemShowcaseUI != null && !panelItemShowcaseUI.activeSelf)
        {
            panelItemShowcaseUI.SetActive(true);
            Debug.Log("เปิด Panel ItemShowcaseUI เพื่อป้องกัน Null");
        }
    }
    
    void Update()
    {
        // ถ้ารถยังไม่เริ่มวิ่ง หรือวิ่งครบทุกจุดแล้ว ให้หยุดการทำงาน
        if (!isMoving || index >= waypoints.Length) return;
        
        // หมุนรถให้ค่อยๆ หันหน้าไปยัง Waypoint ปัจจุบัน
        Vector3 direction = waypoints[index].position - transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime * 100f
            );
        }
  
        // ให้รถเคลื่อนที่เข้าไปใกล้ตำแหน่ง Waypoint ปัจจุบัน         // ตำแหน่งปัจจุบันของรถ     // ตำแหน่งของ Waypoint ปัจจุบัน              // ระยะทางที่จะขยับในแต่ละเฟรม
        transform.position = Vector3.MoveTowards(transform.position, waypoints[index].position, speed * Time.deltaTime);

        // ถ้ารถอยู่ใกล้ Waypoint ปัจจุบันมากพอ (ระยะห่างน้อยกว่า 0.1f) ให้ไปยัง Waypoint ถัดไป
        if (Vector3.Distance(transform.position, waypoints[index].position) < 0.1f)
        {
            index++; // ไปยัง Waypoint ถัดไป
            
            // ถ้าถึง Waypoint ลำดับที่ 4 (Element 3)
            if (index == 3)
            {
                Debug.Log("ถึง Waypoint 4: กำลังหมุนไปทางขวา...");
                // รถจะหันขวาโดยอัตโนมัติเพราะเราใช้ LookRotation อยู่แล้ว
            }
            
            // ถ้าถึงจุดสุดท้าย
            if (index >= waypoints.Length && !finished)
            {
                finished = true;
                Debug.Log("ถึงจุดสุดท้ายแล้ว! กำลังเปิด Player และ UI");

                // วางตำแหน่ง Player ไว้ข้าง ๆ รถ
                if (player != null)
                {
                    player.transform.position = transform.position + transform.right * playerOffset.x + transform.up * playerOffset.y + transform.forward * playerOffset.z;
                    player.SetActive(true);
                }

                // เปิด UI
                if (dotHitCanvas != null)
                {
                    dotHitCanvas.SetActive(true);
                }
                
                // ปิดกล้องของรถ
                if (carCamera != null)
                {
                    carCamera.enabled = false;
                } 
            }
        }
    }

    // ฟังก์ชันสั่งให้รถเริ่มเคลื่อนที่
    public void StartMoving()
    {
        isMoving = true;
    }
}

