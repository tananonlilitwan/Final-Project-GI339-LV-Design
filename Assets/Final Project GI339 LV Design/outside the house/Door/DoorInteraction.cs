using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class DoorInteraction : MonoBehaviour
{
    public bool isLocked = true;  // สถานะว่าประตูถูกล็อคหรือไม่
    public float messageDuration = 2.5f; // ระยะเวลาแสดงข้อความแต่ละอัน

    public GameObject cutsceneMessageUI;  // UI สำหรับแสดงข้อความแบบคัตซีน
    public TextMeshProUGUI messageText;   // ข้อความใน cutsceneMessageUI
 
    public Transform leftDoor;    // ประตูฝั่งซ้าย
    public Transform rightDoor;   // ประตูฝั่งขวา

    public float openAngle = 90f;  // มุมที่จะเปิดออก
    public float openSpeed = 30f;  // ความเร็วในการเปิดประตู

    private bool isOpening = false;  // กำลังเปิดประตูอยู่หรือไม่
    private float currentAngle = 0f; // มุมปัจจุบันที่เปิดไปแล้ว
    private bool hasInteracted = false; // เพื่อไม่ให้ Interact ซ้ำซ้อน
    
    [Header("📸 Camera")]
    public Camera playerCamera;         // กล้องของ Player
    public Camera cutsceneCamera;       // กล้อง Cutscene ที่ไว้ดูประตู

    [Header("🎮 Scripts to Disable")]
    private MonoBehaviour playerInteractionScript; // สคริปต์ที่ไว้ปิดตอนคัตซีน
    private MonoBehaviour doorRaycasterScript;     // สคริปต์ Raycast ประตู
    
    [Header("📜 Text After Opening")]
    public GameObject reactionTextUI;   // UI Panel ที่มีข้อความ
    public TextMeshProUGUI reactionText;  // สคริปต์ Raycast ประตู
    
    [Header("🎮 Crosshair")]
    public Image crosshairImage;        // Image Component สำหรับ Dot Crosshair (จุดเล็ง)
    
    [Header("👾 Zombie Spawner")]
    public GameObject zombieSpawner;  // ZombieSpawner GameObject



    // เรียกเมื่อผู้เล่นกด E ขณะเล็งประตู
    public void Interact()
    {
        if (!hasInteracted && isLocked)
        {
            hasInteracted = true;
            StartCoroutine(ShowLockedCutsceneDialog());
        }
    }

    private void Start()
    {
        // ซ่อน UI cutscene และข้อความ
        if (cutsceneMessageUI != null)
            cutsceneMessageUI.SetActive(false);

        if (reactionTextUI != null)
            reactionTextUI.SetActive(false);

        // ปิดกล้องคัตซีน และเปิดกล้องผู้เล่น
        if (cutsceneCamera != null)
            cutsceneCamera.gameObject.SetActive(false);  // ปิดกล้อง cutsceneCamera

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(true);    // เปิดกล้อง playerCamera

        // ✅ หา Script ใน Main Camera
        if (playerCamera != null)
        {
            playerInteractionScript = playerCamera.GetComponent<PlayerInteraction>();
            doorRaycasterScript = playerCamera.GetComponent<DoorRaycaster>();
        }
        // เปิด Crosshair ตอนเริ่มเกม
        if (crosshairImage != null)
            crosshairImage.enabled = true;  // เปิด Crosshair เมื่อเริ่ม
    }

    private void Update()
    {
        // ถ้ากำลังเปิดประตูและยังไม่ถึงมุมที่กำหนด
        if (isOpening && currentAngle < openAngle)
        {
            float rotateStep = openSpeed * Time.deltaTime;

            // หมุนประตูซ้ายไปทางซ้าย และขวาไปทางขวา
            if (leftDoor != null)
                leftDoor.Rotate(Vector3.up, -rotateStep);

            if (rightDoor != null)
                rightDoor.Rotate(Vector3.up, rotateStep);

            // บันทึกมุมที่หมุนไปแล้ว
            currentAngle += rotateStep;
        }
    }

    // เรียกเมื่อจะเปิดประตู
    public void OpenDoor()
    {
        //public AudioSource doorOpenSound;
        //if (doorOpenSound != null) doorOpenSound.Play(); //เสียงตอนประตูเปิด
        
        
        isOpening = true;
        StartCoroutine(ShowReactionAfterDoorOpened()); // เริ่มคัตซีนหลังประตูเปิด
        
        // ✅ ปิด Collider เพื่อไม่ให้ Raycast เจออีก
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
        
        // ปิด ZombieSpawner
        if (zombieSpawner != null)
            zombieSpawner.SetActive(false);
    }

    // แสดงข้อความคัตซีนเมื่อประตูล็อค
    IEnumerator ShowLockedCutsceneDialog()
    {
        // แสดงข้อความ “door locked...”
        cutsceneMessageUI.SetActive(true);
        messageText.text = "[Headhunter] : door locked..."; // ประตูถูกล็อค...
        yield return new WaitForSeconds(messageDuration);

        // แสดงข้อความต่อ
        messageText.text = "[Headhunter] : I guess I'll have to try finding something to open it."; // ฉันเดาว่าฉันคงต้องพยายามหาอะไรบางอย่างมาเปิดมัน
        yield return new WaitForSeconds(messageDuration);
        
        // ปิดข้อความ
        cutsceneMessageUI.SetActive(false);
        yield return new WaitForSeconds(2f);

        // เปิดประตูและปลดล็อค
        OpenDoor();
        isLocked = false;
    }
    
    // แสดงคัตซีนหลังประตูเปิด และข้อความ
    IEnumerator ShowReactionAfterDoorOpened()
    {
        // ✅ ปิดกล้อง Player และเปิด Cutscene Camera
        if (playerCamera != null) playerCamera.gameObject.SetActive(false);  // ปิดกล้อง Player
        if (cutsceneCamera != null) cutsceneCamera.gameObject.SetActive(true); // เปิดกล้อง Cutscene
        
        // ✅ ปิด Crosshair
        if (crosshairImage != null)
        {
            crosshairImage.enabled = false;  // ปิด Component Image ของ Crosshair
        }

        // ✅ ปิด script interaction
        if (playerInteractionScript != null) playerInteractionScript.enabled = false;
        if (doorRaycasterScript != null) doorRaycasterScript.enabled = false;

        yield return new WaitForSeconds(2f);

        // ✅ แสดงข้อความ Reaction
        if (reactionTextUI != null)
        {
            reactionTextUI.SetActive(true);
            reactionText.text = "[Headhunter] : What, how did the door open by itself?"; // อะไรนะ ประตูมันเปิดเองได้ยังไง?
        }

        yield return new WaitForSeconds(2f);

        // ✅ ปิดข้อความ Reaction
        if (reactionTextUI != null)
            reactionTextUI.SetActive(false);

        // ✅ กลับกล้อง Player และเปิดสคริปต์
        if (cutsceneCamera != null) cutsceneCamera.gameObject.SetActive(false);  // ปิดกล้อง Cutscene
        if (playerCamera != null) playerCamera.gameObject.SetActive(true);      // เปิดกล้อง Player
        
        // ✅ เปิด Crosshair
        if (crosshairImage != null)
        {
            crosshairImage.enabled = true;  // เปิด Component Image ของ Crosshair
        }

        // เปิดสคริปต์ interaction กลับ
        if (playerInteractionScript != null) playerInteractionScript.enabled = true;
        if (doorRaycasterScript != null) doorRaycasterScript.enabled = true;
    }
    
    // pressEUI DoorRaycaster
    public bool HasInteracted()
    {
        return hasInteracted;
    }
}