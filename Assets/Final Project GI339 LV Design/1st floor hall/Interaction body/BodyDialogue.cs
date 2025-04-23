using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BodyDialog
{
    public string bodyTag; // Tag ของ Body
    public string[] dialogLines; // ข้อความที่จะแสดง
}

public class BodyDialogue : MonoBehaviour
{
    public Camera playerCamera;
    public float rayDistance = 40f;

    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;

    public float displayTime = 2f; // เวลาที่จะแสดงข้อความแต่ละบรรทัด
    private bool isDialogActive = false;

    [Header("👁️‍🗨️ Prompt & Crosshair")]
    public TMP_Text promptText; // TextMeshPro ที่แสดง "Press E"
    public Crosshair crosshairScript; // ตัวแปรเพื่ออ้างอิงไปยัง Crosshair Script
    public Color defaultColor = Color.white; // สี Crosshair ปกติ
    public Color highlightColor = Color.green; // สี Crosshair เมื่อมองไปที่ Body

    // ใช้ List แทน Dictionary
    [SerializeField]
    private List<BodyDialog> bodyDialogList = new List<BodyDialog>();

    private string currentBodyTag; // Tag ของ Body ที่พบในขณะนี้
    
    // ✅ อ้างถึง GlassBreakTriggerManager
    public GlassBreakTriggerManager glassManager;
    private HashSet<string> bodiesChecked = new HashSet<string>();

    void Start()
    {
        if (crosshairScript != null)
        {
            crosshairScript.SetColor(Color.magenta); // สีเริ่มต้นที่ชัดเจน
            Debug.Log("เริ่มต้นสี Crosshair");
        }
    }

    void Update()
    {
        if (!isDialogActive)
        {
            CheckRaycast(); // ตรวจสอบทุกเฟรม
        }

        if (Input.GetKeyDown(KeyCode.E) && !isDialogActive && !string.IsNullOrEmpty(currentBodyTag))
        {
            TryShowDialogue(currentBodyTag); // ส่ง Tag ของ Body ที่ตรวจพบ
        }
        
    }
    void CheckRaycast()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            string hitTag = hit.collider.tag;

            // เช็คเฉพาะ tag ที่มีใน bodyDialogList เท่านั้น
            foreach (var bodyDialog in bodyDialogList)
            {
                if (hitTag == bodyDialog.bodyTag)
                {
                    promptText.enabled = true;
                    promptText.gameObject.SetActive(true);
                    currentBodyTag = hitTag;

                    if (crosshairScript != null)
                    {
                        crosshairScript.SetColor(highlightColor);
                    }

                    return;
                }
            }
        }

        // ไม่พบ Body ที่ตรง tag ใน list
        if (crosshairScript != null)
        {
            crosshairScript.SetColor(defaultColor);
        }

        promptText.enabled = false;
        promptText.gameObject.SetActive(false);
        currentBodyTag = null;
    }


    void TryShowDialogue(string bodyTag)
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            if (hit.collider.CompareTag(bodyTag))
            {
                promptText.enabled = false;
                promptText.gameObject.SetActive(false);
                StartCoroutine(ShowDialogCoroutine(bodyTag)); // ส่ง Tag ของ Body
                
                // ✅ แจ้ง Glass Manager
                if (!bodiesChecked.Contains(bodyTag))
                {
                    bodiesChecked.Add(bodyTag);
                    if (glassManager != null)
                        glassManager.BodyChecked(bodyTag);
                }
            }
        }
    }

    IEnumerator ShowDialogCoroutine(string bodyTag)
    {
        isDialogActive = true;
        dialogPanel.SetActive(true);

        // แสดงข้อความจาก Body ที่ตรงกับ Tag
        foreach (var bodyDialog in bodyDialogList)
        {
            if (bodyDialog.bodyTag == bodyTag)
            {
                /*string[] dialogLines = bodyDialog.dialogLines;
                for (int i = 0; i < dialogLines.Length; i++)
                {
                    dialogText.text = dialogLines[i];
                    yield return new WaitForSeconds(displayTime); // เปลี่ยนข้อความทุกๆ 2 วินาที
                }*/
                
                foreach (string line in bodyDialog.dialogLines)
                {
                    dialogText.text = line;
                    yield return new WaitForSeconds(displayTime);
                }
                break;
            }
        }

        dialogPanel.SetActive(false);
        isDialogActive = false;
    }
}
