/*
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class BodyDialogue : MonoBehaviour
{
    public Camera playerCamera;
    public float rayDistance = 40f;

    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;

    [TextArea(2, 5)]
    public string[] dialogLines;

    public float displayTime = 2f; // เวลาที่จะแสดงข้อความแต่ละบรรทัด
    private bool isDialogActive = false;
    
    [Header("👁️‍🗨️ Prompt & Crosshair")]
    public TMP_Text promptText; // TextMeshPro ที่แสดง "Press E"
    public Crosshair crosshairScript; // ตัวแปรเพื่ออ้างอิงไปยัง Crosshair Script
    public Color defaultColor = Color.white; // สี Crosshair ปกติ
    public Color highlightColor = Color.green; // สี Crosshair เมื่อมองไปที่ Body

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
        
        if (Input.GetKeyDown(KeyCode.E) && !isDialogActive)
        {
            TryShowDialogue();
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (crosshairScript != null)
            {
                crosshairScript.SetColor(Color.red); // เปลี่ยนสีเป็นแดงเมื่อกด T
                Debug.Log("กด T แล้วเปลี่ยนสี Crosshair");
            }
        }
    }
    
    void CheckRaycast()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            if (hit.collider.CompareTag("Body"))
            {
                promptText.enabled = true;
                if (crosshairScript != null)
                {
                    Debug.Log("ก่อนเปลี่ยนสี: " + crosshairScript); 
                    crosshairScript.SetColor(highlightColor); // เปลี่ยนสีเมื่อมองไปที่ Body
                    Debug.Log("พบ Body แล้ว เปลี่ยนสี Crosshair");
                }
                return;
            }
        }

        // ถ้าไม่เจอให้รีเซ็ตสี
        if (crosshairScript != null)
        {
            crosshairScript.SetColor(defaultColor); // รีเซ็ตสีเป็นปกติ
            Debug.Log("ไม่ได้พบ Body เปลี่ยนสีเป็น: " + defaultColor);
        }
        
        promptText.enabled = false;
    }

    void TryShowDialogue()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            if (hit.collider.CompareTag("Body"))
            {
                promptText.enabled = false;
                StartCoroutine(ShowDialogCoroutine());
            }
        }
    }

    IEnumerator ShowDialogCoroutine()
    {
        isDialogActive = true;
        dialogPanel.SetActive(true);

        for (int i = 0; i < dialogLines.Length; i++)
        {
            dialogText.text = dialogLines[i];
            yield return new WaitForSeconds(displayTime);
        }

        dialogPanel.SetActive(false);
        isDialogActive = false;
    }
}
*/


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

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (crosshairScript != null)
            {
                crosshairScript.SetColor(Color.red); // เปลี่ยนสีเป็นแดงเมื่อกด T
                Debug.Log("กด T แล้วเปลี่ยนสี Crosshair");
            }
        }
    }

    /*void CheckRaycast()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            if (hit.collider.CompareTag("Body"))
            {
                promptText.enabled = true;
                currentBodyTag = hit.collider.tag; // บันทึก Tag ของ Body ที่ตรวจพบ
                if (crosshairScript != null)
                {
                    crosshairScript.SetColor(highlightColor); // เปลี่ยนสีเมื่อมองไปที่ Body
                }
                return;
            }
        }

        // ถ้าไม่เจอให้รีเซ็ตสี
        if (crosshairScript != null)
        {
            crosshairScript.SetColor(defaultColor); // รีเซ็ตสีเป็นปกติ
        }

        promptText.enabled = false;
        currentBodyTag = null; // รีเซ็ต Tag เมื่อไม่พบ Body
    }*/
    
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
                StartCoroutine(ShowDialogCoroutine(bodyTag)); // ส่ง Tag ของ Body
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
                string[] dialogLines = bodyDialog.dialogLines;
                for (int i = 0; i < dialogLines.Length; i++)
                {
                    dialogText.text = dialogLines[i];
                    yield return new WaitForSeconds(displayTime); // เปลี่ยนข้อความทุกๆ 2 วินาที
                }
            }
        }

        dialogPanel.SetActive(false);
        isDialogActive = false;
    }
}
