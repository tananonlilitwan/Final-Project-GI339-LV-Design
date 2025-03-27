using UnityEngine;

public class ToggleObjectVisibility : MonoBehaviour
{
    public GameObject targetObject; // Obj ที่ต้องการซ่อน/แสดง

    private bool isVisible = true; // สถานะปัจจุบัน

    public void ToggleVisibility()
    {
        if (targetObject != null)
        {
            isVisible = !isVisible;
            targetObject.SetActive(isVisible);
        }
    }
}