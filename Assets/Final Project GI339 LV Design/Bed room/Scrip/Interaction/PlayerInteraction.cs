using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] public float interactDistance; // ระยะตรวจจับวัตถุ
    public LayerMask interactableLayer; // เลเยอร์ที่เป็นวัตถุโต้ตอบได้
    public Crosshair crosshairUI; // อ้างถึง UI จุดตรงกลางหน้าจอ
    
    public TextMeshProUGUI interactionText; // <- ✅ ลาก Text UI จาก Canvas มาวางใน Inspector
    
    private DoorController currentDoor; // ประตูที่กำลังโต้ตอบอยู่
    public static PlayerInteraction Instance;

    void Awake()
    {
        Instance = this; // ตั้งค่าตัวแปร Static Instance ให้เป็นตัวเอง
    }

    public DoorController GetCurrentDoor()
    {
        return currentDoor; // คืนค่า DoorController ที่กำลังโต้ตอบอยู่
    }

    
    void Update()
    {
        CheckForInteractable(); // ตรวจสอบว่าวัตถุที่ผู้เล่นสามารถโต้ตอบได้หรือไม่

        if (Input.GetKeyDown(KeyCode.E)) // กด E เพื่อเก็บไอเท็ม
        {
            TryPickUpItem();  // ลองเก็บไอเท็ม
        }
    }
    
    void CheckForInteractable()
    {
        interactionText.gameObject.SetActive(false); // ซ่อนข้อความ interaction ก่อนทุกครั้ง

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward); // สร้าง Ray สำหรับการตรวจจับ
        RaycastHit hit;  // เก็บข้อมูลการชนของ Ray กับวัตถุ

        // ตรวจสอบว่ามีการชนกับวัตถุที่สามารถโต้ตอบได้หรือไม่
        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
        {
            crosshairUI.SetColor(Color.green); // เปลี่ยนสีของ Crosshair เป็นสีเขียวเมื่อสามารถโต้ตอบได้

            // ถ้าวัตถุที่ชนเป็น KeyPickup ให้แสดงข้อความแนะนำ
            if (hit.collider.GetComponent<KeyPickup>() != null)
            {
                interactionText.text = "Press E to collect items."; // ข้อความแนะนำการเก็บไอเท็ม //กด E เพื่อเก็บไอเท็ม
                interactionText.gameObject.SetActive(true); // แสดงข้อความ
            }
            
            // ถ้าวัตถุที่ชนเป็น DoorController ให้แสดงข้อความแนะนำ
            else if (hit.collider.GetComponent<DoorController>() != null)
            {
                interactionText.text = "Press E to use the key."; // ข้อความแนะนำการใช้กุญแจ //กด E เพื่อใช้กุญแจ
                interactionText.gameObject.SetActive(true); // แสดงข้อความ
            }

            // ตรวจสอบการกดปุ่ม E เพื่อเปิดประตูหรือใช้กุญแจ
            if (Input.GetKeyDown(KeyCode.E))
            {
                DoorController door = hit.collider.GetComponent<DoorController>(); // ถ้าวัตถุที่ชนเป็นประตู
                if (door != null)
                {
                    currentDoor = door; //currentDoor เป็นประตูที่กำลังโต้ตอบ
                    
                    // เปิดระบบ Inventory เมื่อพบประตู
                    if (InventoryManager.Instance != null)
                    {
                        InventoryManager.Instance.ToggleInventory();
                        if (PlayerController.Instance != null)
                        {
                            PlayerController.Instance.ToggleInventory();
                        }
                    }
                }
            }
            //เช็คการกดปุ่ม Mouse Button 0 (คลิกซ้าย) เพื่อโต้ตอบกับวัตถุ
            if (Input.GetMouseButtonDown(0))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();  // เช็คว่าเป็นวัตถุที่สามารถโต้ตอบได้หรือไม่
                if (interactable != null)
                {
                    interactable.Interact(); // เรียกใช้งานฟังก์ชัน Interact()
                }
            }
        }
        else
        {
            crosshairUI.SetColor(Color.white); // ถ้าไม่เจอวัตถุที่สามารถโต้ตอบได้ เปลี่ยนสี Crosshair เป็นสีขาว
        }
    }
    
    // ฟังก์ชันสำหรับการเก็บไอเท็ม
    void TryPickUpItem()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward); // สร้าง Ray สำหรับตรวจจับ
        RaycastHit hit; // เก็บข้อมูลการชนของ Ray กับวัตถุ

        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer)) // ตรวจสอบว่าชนกับวัตถุที่สามารถเก็บได้หรือไม่
        {
            KeyPickup key = hit.collider.GetComponent<KeyPickup>(); // ถ้าวัตถุที่ชนเป็น KeyPickup
            if (key != null && PlayerInventory.Instance != null)
            {
                PlayerInventory.Instance.AddKey(key.keyID); // เพิ่มกุญแจไปยัง Inventory ของผู้เล่น

                // ✅ เพิ่มไอเท็มใน UI ของ Inventory
                string keyName = $"Key {key.keyID}"; // สร้างชื่อของไอเท็ม
                if (InventoryManager.Instance != null)
                {
                    InventoryManager.Instance.AddItem(keyName); // เพิ่มไอเท็มในระบบ Inventory UI
                }

                // ✅ โชว์ 3D Preview UI (แบบ RE4)
                ItemShowcaseUI.Instance.ShowItem(key.item3DPrefab); // แสดงตัวอย่าง 3D ของไอเท็ม

                Destroy(hit.collider.gameObject); // ลบวัตถุที่เก็บแล้วออกจากฉาก
            }
        }
    }
}
