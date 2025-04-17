using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemData itemData;  // ฟิลด์เก็บข้อมูลของไอเท็ม
    public string itemID => itemData.ID.ToString(); // ✅ ให้เข้าถึง itemID แบบปลอดภัย
    
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    private Vector2 originalPosition; // ✅ ตำแหน่งเดิมก่อนลาก

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition; // ✅ บันทึกตำแหน่งเดิม
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))  // เพิ่มระยะให้ไกลขึ้น
        {
            DoorController door = hit.collider.GetComponent<DoorController>(); // ✅ ดูว่ามันชนอะไร
            if (door != null)
            {
                Debug.Log("เจอ DoorController: " + door.name);
                Debug.Log("itemID: " + itemID + " vs doorID: " + door.requiredKeyID);

                // 👇 แก้ให้ใช้ TryUnlockDoor (แปลง itemID ให้เป็น int)
                if (int.TryParse(itemID, out int parsedKeyID))
                {
                    door.TryUnlockDoor(parsedKeyID, gameObject);
                    return;
                }
            }
        }
        // ❌ วางไม่สำเร็จ → ย้ายกลับตำแหน่งเดิม
        ResetPosition();
    }
    
    private void ResetPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
    }
}