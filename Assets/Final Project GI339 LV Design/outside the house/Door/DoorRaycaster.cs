using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DoorRaycaster : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] public float interactRange; // ระยะทางที่สามารถตรวจจับการมองเห็นประตูได้
    public LayerMask doorLayer; // กำหนดให้ประตูอยู่ในเลเยอร์นี้ "Door"

    [Header("Crosshair UI")]
    public Image crosshairImage; // 🎯 รูปภาพ Dot Crosshair
    public Color defaultColor = Color.white; // สีปกติของ Crosshair
    public Color highlightColor = Color.green;  // สีเมื่อมองไปที่ประตู
    
    [Header("Press E UI")]
    // ✅ เพิ่มตัวแปรสำหรับข้อความ "Press E"
    public GameObject pressEUI; // GameObject ที่มีข้อความ "Press E"
    public TextMeshProUGUI pressEText;

    void Update()
    {
        // ยิง Ray ออกไปจากตำแหน่งของกล้องไปด้านหน้า
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        bool isLookingAtDoor = false; // ตัวแปรไว้เช็คว่ากำลังมองประตูอยู่หรือไม่

        // ตรวจสอบว่า Ray ไปชนกับวัตถุที่อยู่ในเลเยอร์ประตูภายในระยะ interactRange
        if (Physics.Raycast(ray, out hit, interactRange, doorLayer))
        {
            // ตรวจสอบว่า collider ที่ชนมีคอมโพเนนต์ DoorInteraction หรือไม่
            DoorInteraction door = hit.collider.GetComponent<DoorInteraction>();
            if (door != null)
            {
                isLookingAtDoor = true; // กำลังมองที่ประตู
                
                // ✅ แสดง "Press E" เฉพาะตอนที่ยังไม่ Interact
                if (!door.HasInteracted())
                {
                    if (pressEUI != null) pressEUI.SetActive(true);
                    if (pressEText != null) pressEText.text = "Press E";
                }
                else
                {
                    // ✅ ถ้า interact แล้วให้ซ่อน Press E
                    if (pressEUI != null) pressEUI.SetActive(false);
                }

                // ถ้าผู้เล่นกดปุ่ม E ให้เรียกใช้ method Interact() ของประตู
                if (Input.GetKeyDown(KeyCode.E))
                {
                    door.Interact();
                }
            }
        }
        
        // ✅ ซ่อนข้อความถ้าไม่ได้มองที่ประตู
        if (!isLookingAtDoor && pressEUI != null)
        {
            pressEUI.SetActive(false);
        }

        // เปลี่ยนสี Crosshair ตามว่ากำลังชี้ไปที่ประตูหรือไม่
        if (crosshairImage != null)
        {
            crosshairImage.color = isLookingAtDoor ? highlightColor : defaultColor;
        }
    }
} 