using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent; // บันทึกตำแหน่งเดิม
        canvasGroup.alpha = 0.6f; // ทำให้โปร่งใส
        canvasGroup.blocksRaycasts = false; // ปิดการตรวจจับการคลิก
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position; // เคลื่อนที่ตามเมาส์
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f; // กลับมาทึบ
        canvasGroup.blocksRaycasts = true; // เปิดการตรวจจับการคลิก
        if (transform.parent == originalParent)
        {
            // ถ้ายังอยู่ที่เดิม ไม่ทำอะไร
            transform.position = originalParent.position;
        }
    }
}
