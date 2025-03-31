using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] public float interactDistance; // ระยะตรวจจับวัตถุ
    public LayerMask interactableLayer; // เลเยอร์ที่เป็นวัตถุโต้ตอบได้
    public Crosshair crosshairUI; // อ้างถึง UI จุดตรงกลางหน้าจอ

    void Update()
    {
        CheckForInteractable();
    }

    void CheckForInteractable()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
        {
            // ตรวจจับวัตถุที่สามารถโต้ตอบได้
            crosshairUI.SetColor(Color.green);

            if (Input.GetMouseButtonDown(0)) // กดคลิกซ้าย
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
        else
        {
            crosshairUI.SetColor(Color.white); // ถ้าไม่มีวัตถุ เปลี่ยนเป็นสีขาว
        }
    }
}