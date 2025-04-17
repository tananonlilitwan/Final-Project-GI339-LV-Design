using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraFocusTrigger : MonoBehaviour
{
    public Transform doorTarget;          // เป้าหมายที่กล้องจะหันไปหา (เช่น ประตู)
    public float focusDuration = 2f;      // ระยะเวลาที่จะมองประตู
    public Camera cameraToFocus;          // กล้องที่จะหมุน (และเปิดชั่วคราว)
    public Camera playerCamera;           // กล้องของผู้เล่น
    public Image dotHitImage;               // รูปภาพ UI จุดเล็งยิง (Image component โดยตรง)
    
    private Quaternion originalRotation;  // มุมกล้องเดิม
    private bool hasTriggered = false;      // ป้องกันชนซ้ำหลายครั้ง
    
    private PlayerInteraction playerInteractionScript;
    private DoorRaycaster doorRaycasterScript; // สคริปต์ DoorRaycaster ของ PlayerCamera
    
    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return; // ชนได้ครั้งเดียว
        
        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            Debug.Log("Player เข้าสู่เซ็นเซอร์");
            
            // ค้นหา PlayerInteraction จาก Player (Main Camera)
            playerInteractionScript = other.GetComponentInChildren<PlayerInteraction>();
            if (playerInteractionScript != null)
            {
                playerInteractionScript.enabled = false; // ปิดการโต้ตอบผู้เล่น
                Debug.Log("ปิด PlayerInteraction แล้ว");
            }
            else
            {
                Debug.LogWarning("ไม่พบ PlayerInteraction ใต้ Player");
            }
            
            // ค้นหา DoorRaycaster จาก PlayerCamera
            doorRaycasterScript = playerCamera.GetComponent<DoorRaycaster>();
            if (doorRaycasterScript != null)
            {
                doorRaycasterScript.enabled = false; // ปิด DoorRaycaster
                Debug.Log("ปิด DoorRaycaster แล้ว");
            }
            else
            {
                Debug.LogWarning("ไม่พบ DoorRaycaster ใน PlayerCamera");
            }
            
            StartCoroutine(FocusOnDoor());
        }
    }

    IEnumerator FocusOnDoor()
    {
        if (cameraToFocus == null || doorTarget == null || playerCamera == null)
        {
            yield break;
        }
        
        // ปิด UI จุดเล็ง
        if (dotHitImage != null)
        {
            dotHitImage.enabled = false;
        }

        // บันทึกมุมเดิมของกล้อง cameraToFocus
        originalRotation = cameraToFocus.transform.rotation;

        // ปิดกล้องผู้เล่น
        playerCamera.enabled = false;

        // เปิดกล้องโฟกัส
        cameraToFocus.enabled = true;

        // หมุนกล้องไปยังประตู
        Vector3 direction = (doorTarget.position - cameraToFocus.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        cameraToFocus.transform.rotation = lookRotation;

        // รอเวลา
        yield return new WaitForSeconds(focusDuration);

        // ปิดกล้องโฟกัส
        cameraToFocus.enabled = false;

        // เปิดกล้องผู้เล่นกลับมา
        playerCamera.enabled = true;

        // รีเซ็ตมุมกล้องของกล้องโฟกัสให้กลับมาเหมือนเดิม
        cameraToFocus.transform.rotation = originalRotation;
        
        // เปิด UI จุดเล็งกลับมา
        if (dotHitImage != null)
        {
            dotHitImage.enabled = true;
        }
        
        // เปิด PlayerInteraction กลับมา
        if (playerInteractionScript != null)
        {
            playerInteractionScript.enabled = true;
            Debug.Log("เปิด PlayerInteraction กลับแล้ว");
        }
        
        // เปิด DoorRaycaster กลับมา
        if (doorRaycasterScript != null)
        {
            doorRaycasterScript.enabled = true; // เปิด DoorRaycaster
            Debug.Log("เปิด DoorRaycaster กลับแล้ว");
        }
    }
}