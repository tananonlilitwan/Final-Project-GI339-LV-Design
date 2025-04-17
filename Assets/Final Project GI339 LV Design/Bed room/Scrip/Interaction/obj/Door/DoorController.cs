using UnityEngine;
using UnityEngine.EventSystems;

public class DoorController : MonoBehaviour, IDropHandler
{
    public int requiredKeyID; // หมายเลขกุญแจที่ใช้ไข
    public Transform leftDoor;  // ประตูบานซ้าย
    public Transform rightDoor; // ประตูบานขวา
    [SerializeField] public float openAngle; //= 90f; // มุมเปิด
    [SerializeField] public float openSpeed; //= 2f;  // ความเร็วเปิด

    private bool isOpen = false;
    private Quaternion leftInitialRotation, rightInitialRotation;
    private Quaternion leftTargetRotation, rightTargetRotation;

    void Start()
    {
        if (leftDoor == null || rightDoor == null)
        {
            Debug.LogError("❌ กรุณาใส่ประตูซ้าย/ขวาใน Inspector ให้ครบ");
            enabled = false;
            return;
        }
        
        // บันทึกตำแหน่งเริ่มต้นของประตู
        leftInitialRotation = leftDoor.rotation;
        rightInitialRotation = rightDoor.rotation;

        // ตั้งค่ามุมเป้าหมาย (ซ้ายหมุนออก - ขวาหมุนออก)
        leftTargetRotation = Quaternion.Euler(0, -openAngle, 0) * leftInitialRotation;
        rightTargetRotation = Quaternion.Euler(0, openAngle, 0) * rightInitialRotation;
    }

    void Update()
    {
        if (isOpen)
        {
            // หมุนประตูไปที่ตำแหน่งเปิด  // Animation สำหรับหมุนประตู
            leftDoor.rotation = Quaternion.Slerp(leftDoor.rotation, leftTargetRotation, Time.deltaTime * openSpeed);
            rightDoor.rotation = Quaternion.Slerp(rightDoor.rotation, rightTargetRotation, Time.deltaTime * openSpeed);
        }
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        var draggedItem = eventData.pointerDrag?.GetComponent<InventoryItemUI>();

        if (draggedItem != null)
        {
            int itemKeyID = draggedItem.itemData.ID;

            TryUnlockDoor(itemKeyID, draggedItem.gameObject); // ✅ ใช้อันนี้แทนการเปิดโดยตรง
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        Debug.Log("ประตูเปิด!");
        
        
        /*AudioSource audio = GetComponent<AudioSource>();
        if (audio != null)
        {
            audio.Play(); // เล่นเสียงเปิดประตู ถ้ามี AudioSource
        }*/
    }
    
    public void TryUnlockDoor(int usedKeyID, GameObject draggedItem)
    {
        if (usedKeyID == requiredKeyID)
        {
            if (PlayerInventory.Instance.HasKey(usedKeyID))
            {
                OpenDoor();
                PlayerInventory.Instance.RemoveKey(usedKeyID);
                Destroy(draggedItem); // ← ลบไอเท็มที่ใช้ถูกต้อง
            }
            else
            {
                Debug.Log("ไม่มีคีย์ใน inventory");
                UIManager.Instance.ShowMessage("You don't have this key."); // คุณไม่มีคีย์นี้
            }
        }
        else
        {
            Debug.Log("กุญแจไม่ถูกต้อง (ID ไม่ตรง)");
            UIManager.Instance.ShowMessage("Invalid key");  //กุญแจไม่ถูกต้อง
        }
    }
    
}
