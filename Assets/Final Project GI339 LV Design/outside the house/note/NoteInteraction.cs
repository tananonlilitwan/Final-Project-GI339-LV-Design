using UnityEngine;
using UnityEngine.UI; // ← ✅ สำคัญ! เพื่อให้ใช้ Image ได้
using TMPro;

public class NoteInteraction : MonoBehaviour
{
    public float interactRange = 30f;
    public LayerMask interactLayer;
    public Image crosshair; // ปรับสีเวลาเจอ Note
    public TextMeshProUGUI interactPrompt;
    public GameObject noteUI;
    public TextMeshProUGUI noteText;

    private Camera cam;
    private NoteComponent currentNote;
    
    public CameraController playerLook;              // Drag ตัวควบคุมกล้องมาที่ Inspector
    public PlayerController playerMovement;      // Drag ตัวควบคุมการเดินมาที่ Inspector


    void Start()
    {
        cam = Camera.main;
        interactPrompt.enabled = false;
        noteUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        cam = Camera.main;
        if (cam == null)
        {
            //Debug.LogError("Camera.main not found! Make sure your player camera has the 'MainCamera' tag.");
        }
    }

    void Update()
    {
        if (noteUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                CloseNote();
            return;
        }

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        //Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.green); // 👈 ดูเส้นยิง
        
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayer))
        {
            //Debug.Log("Raycast hit: " + hit.collider.name); // 👈 แสดงชื่อที่ยิงโดน
            
            NoteComponent note = hit.collider.GetComponent<NoteComponent>();
            if (note != null)
            {
                //Debug.Log("NoteComponent found: " + note.noteContent); // 👈 เช็คว่ามีคอมโพเนนต์
                
                crosshair.color = Color.green;
                interactPrompt.text = "Press E to Read";
                interactPrompt.enabled = true;
                currentNote = note;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    //Debug.Log("E Pressed"); // 👈 ลองดูว่าได้กด E จริงไหม
                    OpenNote(note.noteContent);
                }

                return;
            }
        }

        crosshair.color = Color.white;
        interactPrompt.enabled = false;
        currentNote = null;
    }

    void OpenNote(string content)
    {
        noteText.text = content;
        noteUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        
        // 🔒 ปิดการหมุนกล้องและเดิน
        if (playerLook != null) playerLook.enabled = false;
        if (playerMovement != null) playerMovement.enabled = false;
        if (crosshair != null) crosshair.enabled = false; // 👈 ซ่อน crosshair

    }

    void CloseNote()
    {
        noteUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        
        // 🔓 เปิดการหมุนกล้องและเดินกลับมา
        if (playerLook != null) playerLook.enabled = true;
        if (playerMovement != null) playerMovement.enabled = true;
        if (crosshair != null) crosshair.enabled = true; // 👈 แสดง crosshair กลับมา
    }
}