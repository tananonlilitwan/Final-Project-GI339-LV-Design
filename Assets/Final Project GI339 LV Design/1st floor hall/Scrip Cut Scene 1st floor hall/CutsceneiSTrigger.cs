using System.Collections;
using UnityEngine;
using TMPro;

public class CutsceneiSTrigger : MonoBehaviour
{
    [Header("Camera")] // กล้องของผู้เล่นและกล้องคัตซีน
    public Camera playerCamera;
    public Camera cutsceneCamera;

    // ผู้เล่น และจุดที่ต้องเดินไปในคัตซีน
    public Transform player;
    public Transform walkTarget;
    public float walkSpeed = 2f; // ความเร็วในการเดิน

    [Header("🚪 Door Rotation (No Animator)")]
    public Transform leftDoor; // ประตูฝั่งซ้าย
    public Transform rightDoor; // ประตูฝั่งขวา
    public float closeAngle = 90f; // มุมที่จะปิดประตู
    public float closeSpeed = 30f; // ความเร็วในการปิดประตู

    [Header("💬 Dialogue")]
    public string[] lines; // บทสนทนาที่จะแสดง
    public TextMeshProUGUI dialogText; // ข้อความใน UI
    public GameObject dialogPanel; // Panel UI แสดงบทสนทนา

    private bool isInCutscene = false; // กำลังอยู่ในคัตซีนหรือไม่
    private int dialogIndex = 0; // ดัชนีของบทสนทนาปัจจุบัน

    private bool isClosing = false; // กำลังปิดประตูอยู่หรือไม่
    private float currentAngle = 0f; // มุมที่ปิดประตูไปแล้ว

    // เมื่อตัวละครเข้า Collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isInCutscene)
        {
            StartCoroutine(PlayCutscene()); // เริ่มคัตซีน
        }
    }

    // คัตซีนหลัก
    /*IEnumerator PlayCutscene()
    {
        isInCutscene = true;

        // ปิดกล้องผู้เล่น เปิดกล้องคัตซีน
        playerCamera.gameObject.SetActive(false);
        cutsceneCamera.gameObject.SetActive(true);

        // ปิดการควบคุมผู้เล่น
        PlayerController playerCtrl = player.GetComponent<PlayerController>();
        if (playerCtrl != null)
            playerCtrl.enabled = false;

        // ให้ผู้เล่นเดินไปยังจุดที่กำหนด
        while (Vector3.Distance(player.position, walkTarget.position) > 0.1f)
        {
            player.position = Vector3.MoveTowards(player.position, walkTarget.position, walkSpeed * Time.deltaTime);
            player.LookAt(walkTarget.position); // หันไปทางจุดเป้าหมาย
            yield return null;
        }

        // เริ่มปิดประตู (แบบหมุน)
        isClosing = true;
        yield return new WaitUntil(() => currentAngle >= closeAngle);

        // รอ 1 วิ แล้วเริ่มแสดงบทสนทนา
        yield return new WaitForSeconds(1f);
        StartCoroutine(ShowDialog());
    }*/
    
    IEnumerator PlayCutscene()
    {
        isInCutscene = true;

        // ปิดกล้องผู้เล่น เปิดกล้องคัตซีน
        playerCamera.gameObject.SetActive(false);
        cutsceneCamera.gameObject.SetActive(true);

        // ปิดการควบคุมผู้เล่น
        PlayerController playerCtrl = player.GetComponent<PlayerController>();
        if (playerCtrl != null)
            playerCtrl.enabled = false;

        // เริ่มแสดงบทสนทนา **ทันที**
        StartCoroutine(ShowDialog());

        // ให้ผู้เล่นเดินไปยังจุดที่กำหนดระหว่างบทสนทนา
        while (Vector3.Distance(player.position, walkTarget.position) > 0.1f)
        {
            player.position = Vector3.MoveTowards(player.position, walkTarget.position, walkSpeed * Time.deltaTime);
            player.LookAt(walkTarget.position); // หันไปทางจุดเป้าหมาย
            yield return null;
        }

        // เริ่มปิดประตู (แบบหมุน)
        isClosing = true;
    }


    
    // อัปเดตการหมุนของประตู
    private void Update()
    {
        if (isClosing && currentAngle < closeAngle)
        {
            float rotateStep = closeSpeed * Time.deltaTime;

            if (leftDoor != null)
                leftDoor.Rotate(Vector3.up, rotateStep); // ประตูซ้ายหมุนเข้าด้านใน

            if (rightDoor != null)
                rightDoor.Rotate(Vector3.up, -rotateStep); // ประตูขวาหมุนเข้าด้านใน

            currentAngle += rotateStep; // บันทึกว่าหมุนไปแล้วเท่าไหร่
        }
    }

    // แสดงบทสนทนาแบบทีละประโยค
    /*IEnumerator ShowDialog()
    {
        dialogPanel.SetActive(true); // เปิด Panel แสดงบทสนทนา
        dialogIndex = 0;

        while (dialogIndex < lines.Length)
        {
            dialogText.text = lines[dialogIndex]; // แสดงประโยค
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space)); // รอกด Space เพื่อข้ามไปบรรทัดถัดไป
            dialogIndex++;
        }

        dialogPanel.SetActive(false); // ปิดบทสนทนา

        // กลับกล้อง และเปิดการควบคุมผู้เล่น
        cutsceneCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);

        PlayerController playerCtrl = player.GetComponent<PlayerController>();
        if (playerCtrl != null)
            playerCtrl.enabled = true;

        Destroy(gameObject);  // ทำลาย Trigger ไม่ให้คัตซีนเล่นซ้ำ
    }*/
    
    // แสดงบทสนทนาแบบทีละประโยค
    IEnumerator ShowDialog()
    {
        dialogPanel.SetActive(true); // เปิด Panel แสดงบทสนทนา
        dialogIndex = 0;

        while (dialogIndex < lines.Length)
        {
            dialogText.text = lines[dialogIndex]; // แสดงประโยค
            yield return new WaitForSeconds(3f); // รอ 3 วินาที แล้วเปลี่ยนประโยค
            dialogIndex++;
        }

        dialogPanel.SetActive(false); // ปิดบทสนทนา

        // ✅ กลับกล้อง และเปิดการควบคุมผู้เล่นเมื่อจบบทสนทนา
        cutsceneCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);

        PlayerController playerCtrl = player.GetComponent<PlayerController>();
        if (playerCtrl != null)
            playerCtrl.enabled = true;

        Destroy(gameObject);  // ทำลาย Trigger ไม่ให้คัตซีนเล่นซ้ำ
    }

}
