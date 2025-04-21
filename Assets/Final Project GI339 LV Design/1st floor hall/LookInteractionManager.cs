using UnityEngine;
using TMPro;

public class LookInteractionManager : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float rayDistance = 40f;
    public LayerMask interactableLayer;

    [Header("UI")]
    public Crosshair crosshair;               // ลิงก์ไปยังสคริปต์ Crosshair
    public TMP_Text interactionText;          // UI ที่แสดงคำว่า "กด E เพื่ออ่าน"

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        interactionText.gameObject.SetActive(false); // ซ่อนไว้ตอนเริ่ม
    }

    void Update()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, interactableLayer))
        {
            // ถ้าชน Note ที่มี NoteTrigger.cs
            NoteTrigger note = hit.collider.GetComponent<NoteTrigger>();
            if (note != null && !note.HasRead())
            {
                crosshair.SetColor(Color.green);
                interactionText.gameObject.SetActive(true);
                interactionText.text = "กด E เพื่ออ่าน";

                if (Input.GetKeyDown(KeyCode.E))
                {
                    note.ReadNote(); // สั่งอ่าน Note
                }

                return;
            }
        }

        // ไม่เจอ Note
        crosshair.SetColor(Color.white);
        interactionText.gameObject.SetActive(false);
    }
}