/*
using UnityEngine;
using TMPro;
using System.Collections;

public class GlassBreakTriggerManager : MonoBehaviour
{
    [Header("🪟 Glass Settings")]
    public GameObject[] glassObjects;         // เก็บวัตถุกระจกที่จะแตก
    public GameObject[] glassShardsObjects;   // เก็บวัตถุเศษกระจกที่จะแสดงหลังจากกระจกแตก

    [Header("👹 Boss Settings")]
    public GameObject bossObject;             // ตัว Boss ที่จะโผล่มาหลังเหตุการณ์
    public GameObject coverLeftPoint;         // จุดสำหรับวิ่งไปหลบด้านซ้าย
    public GameObject coverRightPoint;        // จุดสำหรับวิ่งไปหลบด้านขวา
    public Transform player;                  // ตัวละครผู้เล่น
    public float coverSpeed = 5f;             // ความเร็วในการวิ่งไปยังจุดหลบ

    [Header("🧠 Choice UI")]
    public GameObject choiceUIPanel;          // UI ที่ให้เลือกว่าจะหลบด้านซ้ายหรือขวา
    public TMP_Text dialogText;               // ข้อความแสดงใน UI
    
    [Header("📜 Dialog Lines")]
    public string[] lines; // บทสนทนาที่จะแสดงแบบหลายบรรทัด
    private int currentLineIndex = 0; // เก็บว่าอยู่บรรทัดที่เท่าไร

    // ตัวแปรสำหรับเก็บจำนวน Note ที่อ่าน และศพที่ตรวจ
    private int notesRead = 0;
    private int bodiesChecked = 0;

    // เวลาที่ผู้เล่นอยู่เฉย ๆ โดยไม่กดอะไร
    private float idleTime = 0f;

    // ตัวแปรตรวจสอบว่ากระจกแตกแล้วหรือยัง
    private bool glassBroken = false;

    // ตัวแปรตรวจสอบว่าผู้เล่นได้เลือกหลบซ้ายหรือขวาแล้วหรือยัง
    private bool choiceMade = false;

    
    // แสดง UI ให้เลือกทางซ้ายหรือขวา พร้อมข้อความ
    void ShowChoiceUI()
    {
        choiceUIPanel.SetActive(true);
        currentLineIndex = 0;
        if (lines.Length > 0)
        {
            dialogText.text = lines[currentLineIndex];
        }
        else
        {
            dialogText.text = "Press E (Left) or Q (Right) to take cover.";
        }
    }

    
    void Start()
    {
        // ซ่อน Boss และ UI ตอนเริ่มเกม
        bossObject.SetActive(false);
        choiceUIPanel.SetActive(false);

        // ซ่อนเศษกระจกทั้งหมดไว้ก่อน
        foreach (var shard in glassShardsObjects)
        {
            shard.SetActive(false);
        }
    }

    void Update()
    {
        // ถ้ากระจกแตกไปแล้ว ไม่ต้องทำอะไร
        if (glassBroken) return;

        // ถ้ามีการกดปุ่มใด ๆ จะรีเซ็ต idleTime
        if (Input.anyKey)
        {
            idleTime = 0f;
        }
        else
        {
            idleTime += Time.deltaTime;
        }

        // เงื่อนไขกระจกแตก: อ่าน Note ครบ 2 อัน และตรวจ Body ครบ 2 อัน หรืออยู่เฉย ๆ เกิน 10 วิ
        if ((notesRead >= 2 && bodiesChecked >= 2) || idleTime >= 10f)
        {
            BreakAllGlass();
        }

        // ถ้ากระจกแตกและยังไม่ได้เลือกหลบ
        if (glassBroken && !choiceMade)
        {
            // กด Space เพื่อดูบรรทัดถัดไป
            if (Input.GetKeyDown(KeyCode.Space))
            {
                currentLineIndex++;
                if (currentLineIndex < lines.Length)
                {
                    dialogText.text = lines[currentLineIndex];
                }
                else
                {
                    // หลังจากจบบทสนทนา ให้เลือกทิศทางได้
                    dialogText.text = "Press E (Left) or Q (Right) to take cover.";
                }
            }
            
            // เลือกว่าจะหลบด้านไหน
            if (Input.GetKeyDown(KeyCode.E))
            {
                choiceMade = true;
                StartCoroutine(MoveToCover(coverLeftPoint.transform.position)); // วิ่งไปหลบทางซ้าย
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                choiceMade = true;
                StartCoroutine(MoveToCover(coverRightPoint.transform.position)); // วิ่งไปหลบทางขวา
            }
        }
    }

    // ฟังก์ชันทำให้กระจกแตกทั้งหมด
    void BreakAllGlass()
    {
        glassBroken = true;

        // ปิดการแสดงกระจก และเปิดเศษกระจก
        for (int i = 0; i < glassObjects.Length; i++)
        {
            if (glassObjects[i] != null)
                glassObjects[i].SetActive(false);

            if (i < glassShardsObjects.Length && glassShardsObjects[i] != null)
                glassShardsObjects[i].SetActive(true);
        }

        // แสดงข้อความให้ผู้เล่นเลือกว่าจะหลบทางไหน
        ShowChoiceUI();
    }

    // ฟังก์ชันเรียกเมื่อผู้เล่นอ่าน Note
    public void NoteRead()
    {
        notesRead++;
    }

    // ฟังก์ชันเรียกเมื่อผู้เล่นตรวจศพ
    public void BodyChecked()
    {
        bodiesChecked++;
    }
    

    // ฟังก์ชันเคลื่อนที่ผู้เล่นไปยังตำแหน่งหลบ
    IEnumerator MoveToCover(Vector3 targetPosition)
    {
        // ปิด UI
        choiceUIPanel.SetActive(false);
        dialogText.text = "";

        // ค่อย ๆ เคลื่อนที่ไปยังตำแหน่งหลบจนใกล้
        while (Vector3.Distance(player.position, targetPosition) > 0.2f)
        {
            player.position = Vector3.MoveTowards(player.position, targetPosition, coverSpeed * Time.deltaTime);
            yield return null;
        }

        // หลังจากหลบแล้วให้ Boss ปรากฏตัว
        ShowBoss();
    }

    // แสดง Boss ออกมา
    void ShowBoss()
    {
        bossObject.SetActive(true);
        Debug.Log("👹 Boss Appears!");
    }
}
*/


/*
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;  // สำหรับใช้ List

public class GlassBreakTriggerManager : MonoBehaviour
{
    [Header("🪟 Glass Settings")]
    public GameObject[] glassObjects;         // เก็บวัตถุกระจกที่จะแตก
    public GameObject[] glassShardsObjects;   // เก็บวัตถุเศษกระจกที่จะแสดงหลังจากกระจกแตก

    [Header("👹 Boss Settings")]
    public GameObject bossObject;             // ตัว Boss ที่จะโผล่มาหลังเหตุการณ์
    public GameObject coverLeftPoint;         // จุดสำหรับวิ่งไปหลบด้านซ้าย
    public GameObject coverRightPoint;        // จุดสำหรับวิ่งไปหลบด้านขวา
    public Transform player;                  // ตัวละครผู้เล่น
    public float coverSpeed = 5f;             // ความเร็วในการวิ่งไปยังจุดหลบ

    [Header("🧠 Choice UI")]
    public GameObject choiceUIPanel;          // UI ที่ให้เลือกว่าจะหลบด้านซ้ายหรือขวา
    public TMP_Text dialogText;               // ข้อความแสดงใน UI

    [Header("📢 Mid Screen Message")]
    public TMP_Text centerMessageText; // ข้อความกลางจอ

    
    [Header("📜 Dialog Lines")]
    public string[] lines; // บทสนทนาที่จะแสดงแบบหลายบรรทัด
    private int currentLineIndex = 0; // เก็บว่าอยู่บรรทัดที่เท่าไร

    // ตัวแปรสำหรับเก็บจำนวน Note ที่อ่าน และศพที่ตรวจ
    private int notesRead = 0;
    private int bodiesChecked = 0;

    // เวลาที่ผู้เล่นอยู่เฉย ๆ โดยไม่กดอะไร
    private float idleTime = 0f;

    // ตัวแปรตรวจสอบว่ากระจกแตกแล้วหรือยัง
    private bool glassBroken = false;

    // ตัวแปรตรวจสอบว่าผู้เล่นได้เลือกหลบซ้ายหรือขวาแล้วหรือยัง
    private bool choiceMade = false;

    // รายการของ Note ที่เกี่ยวข้องกับการแตกกระจก
    public List<string> requiredNotes = new List<string>();  // รายการ Note ที่ต้องอ่าน
    private List<string> notesReadList = new List<string>(); // รายการ Note ที่ผู้เล่นได้อ่าน
    
    private bool waitingForCoverChoice = false;

    
    // แสดง UI ให้เลือกทางซ้ายหรือขวา พร้อมข้อความ
    void ShowChoiceUI()
    {
        choiceUIPanel.SetActive(true);
        currentLineIndex = 0;
        if (lines.Length > 0)
        {
            //dialogText.text = lines[currentLineIndex];
            StartCoroutine(AutoShowDialog()); // เรียกแสดงข้อความแบบอัตโนมัติ
        }
        else
        {
            dialogText.text = "Press E (Left) or Q (Right) to take cover.";
        }
    }

    
    void Start()
    {
        // ซ่อน Boss และ UI ตอนเริ่มเกม
        bossObject.SetActive(false);
        choiceUIPanel.SetActive(false);

        // ซ่อนเศษกระจกทั้งหมดไว้ก่อน
        foreach (var shard in glassShardsObjects)
        {
            shard.SetActive(false);
        }
        
        if (bossObject != null)
            bossObject.SetActive(false);

        if (choiceUIPanel != null)
            choiceUIPanel.SetActive(false);

    }

    void Update()
    {
        /#1#/ ถ้ากระจกแตกไปแล้ว ไม่ต้องทำอะไร
        if (glassBroken) return;

        // ถ้ามีการกดปุ่มใด ๆ จะรีเซ็ต idleTime
        if (Input.anyKey)
        {
            idleTime = 0f;
        }
        else
        {
            idleTime += Time.deltaTime;
        }

        // เงื่อนไขกระจกแตก: อ่าน Note ครบ 2 อัน และตรวจ Body ครบ 2 อัน หรืออยู่เฉย ๆ เกิน 10 วิ
        if ((notesRead >= 2 && bodiesChecked >= 2) || idleTime >= 10f)
        {
            BreakAllGlass();
        }

        if (glassBroken && !choiceMade)
        {
            HandleDialogAndChoice();
        }#1#
        
        if (glassBroken && waitingForCoverChoice && !choiceMade)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                choiceMade = true;
                waitingForCoverChoice = false;
                dialogText.text = "Dodging to the left...";
                StartCoroutine(MoveToCover(coverLeftPoint.transform.position));
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                choiceMade = true;
                waitingForCoverChoice = false;
                dialogText.text = "Dodging to the right...";
                StartCoroutine(MoveToCover(coverRightPoint.transform.position));
            }
        }

        if (!glassBroken)
        {
            if (Input.anyKey)
                idleTime = 0f;
            else
                idleTime += Time.deltaTime;

            if ((notesRead >= 2 && bodiesChecked >= 2) || idleTime >= 10f)
                BreakAllGlass();
        }
    }

    // ฟังก์ชันทำให้กระจกแตกทั้งหมด
    /*void BreakAllGlass()
    {
        glassBroken = true;

        // ปิดการแสดงกระจก และเปิดเศษกระจก
        for (int i = 0; i < glassObjects.Length; i++)
        {
            if (glassObjects[i] != null)
                glassObjects[i].SetActive(false);

            if (i < glassShardsObjects.Length && glassShardsObjects[i] != null)
                glassShardsObjects[i].SetActive(true);
            
            // ให้เศษกระจกพุ่งออกมา
            if (i < glassShardsObjects.Length && glassShardsObjects[i] != null)
            {
                GameObject shard = glassShardsObjects[i];
                shard.SetActive(true);

                Rigidbody rb = shard.GetComponent<Rigidbody>();

                /*if (rb != null)
                {
                    Vector3 direction = (player.position - shard.transform.position).normalized;
                    rb.AddForce(direction * 300f); // ปรับแรงตามต้องการ
                }#2#

                if (rb != null)
                {
                    Vector3 randomDir = Random.onUnitSphere; // พุ่งออกทุกทิศแบบสุ่ม
                    rb.AddForce(randomDir * 300f); // ปรับแรงได้ตามต้องการ
                }
            }
        }

        // แสดงข้อความให้ผู้เล่นเลือกว่าจะหลบทางไหน
        ShowChoiceUI();
    }#1#
    void BreakAllGlass()
    {
        glassBroken = true;

        // ปิดกระจก / เปิดเศษ
        for (int i = 0; i < glassObjects.Length; i++)
        {
            if (glassObjects[i] != null)
                glassObjects[i].SetActive(false);

            if (i < glassShardsObjects.Length && glassShardsObjects[i] != null)
            {
                glassShardsObjects[i].SetActive(true);
                Rigidbody rb = glassShardsObjects[i].GetComponent<Rigidbody>();
                if (rb != null)
                    rb.AddForce(Random.onUnitSphere * 300f);
            }
        }

        // เริ่มแสดงข้อความจาก lines ก่อน
        choiceUIPanel.SetActive(true);
        StartCoroutine(StartDialogBeforeChoice());
    }


    // ฟังก์ชันเรียกเมื่อผู้เล่นอ่าน Note
    public void NoteRead(string noteID)
    {
        // ตรวจสอบว่า Note ที่อ่านตรงกับ Note ที่เกี่ยวข้องหรือไม่
        if (requiredNotes.Contains(noteID) && !notesReadList.Contains(noteID))
        {
            notesReadList.Add(noteID);
            notesRead++;
            CheckGlassBreakCondition();  // ตรวจสอบเงื่อนไขการแตกของกระจก
        }
    }

    // ฟังก์ชันเรียกเมื่อผู้เล่นตรวจศพ
    public void BodyChecked()
    {
        bodiesChecked++;
        CheckGlassBreakCondition();  // ตรวจสอบเงื่อนไขการแตกของกระจก
    }

    // ฟังก์ชันตรวจสอบเงื่อนไขกระจกแตก
    void CheckGlassBreakCondition()
    {
        if ((notesRead >= 2 && bodiesChecked >= 2) || idleTime >= 10f)
        {
            BreakAllGlass();
        }
    }

    // ฟังก์ชันเคลื่อนที่ผู้เล่นไปยังตำแหน่งหลบ
    /*IEnumerator MoveToCover(Vector3 targetPosition)
    {
        // ปิด UI
        choiceUIPanel.SetActive(false);
        dialogText.text = "";

        // ค่อย ๆ เคลื่อนที่ไปยังตำแหน่งหลบจนใกล้
        while (Vector3.Distance(player.position, targetPosition) > 0.2f)
        {
            player.position = Vector3.MoveTowards(player.position, targetPosition, coverSpeed * Time.deltaTime);
            yield return null;
        }

        // รอ 4 วินาที ก่อนโชว์ Boss
        yield return new WaitForSeconds(4f);
        ShowBoss();
    }#1#
    
    IEnumerator MoveToCover(Vector3 targetPosition)
    {
        choiceUIPanel.SetActive(false);
        dialogText.text = "";

        // แสดงข้อความกลางหน้าจอว่า "หลบใน Cover"
        StartCoroutine(ShowCenterMessage("[Headhunter] : to take cover", 2f));

        while (Vector3.Distance(player.position, targetPosition) > 0.2f)
        {
            player.position = Vector3.MoveTowards(player.position, targetPosition, coverSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(4f);
        ShowBoss();
    }


    // แสดง Boss ออกมา
    void ShowBoss()
    {
        bossObject.SetActive(true);
        Debug.Log("👹 Boss Appears!");
    }
    
    void HandleDialogAndChoice()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            currentLineIndex++;
            if (currentLineIndex < lines.Length)
            {
                dialogText.text = lines[currentLineIndex];
            }
            else
            {
                dialogText.text = "Press E (Left) or Q (Right) to take cover.";
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            choiceMade = true;
            StartCoroutine(MoveToCover(coverLeftPoint.transform.position));
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            choiceMade = true;
            StartCoroutine(MoveToCover(coverRightPoint.transform.position));
        }#1#
        
        /*if (currentLineIndex < lines.Length - 1 && Input.GetKeyDown(KeyCode.Space))
        {
            currentLineIndex++;
            dialogText.text = lines[currentLineIndex];
        }
        else if (currentLineIndex >= lines.Length - 1)
        {
            dialogText.text = "กด E เพื่อหลบซ้าย หรือ Q เพื่อหลบขวา";

            if (Input.GetKeyDown(KeyCode.E))
            {
                choiceMade = true;
                StartCoroutine(MoveToCover(coverLeftPoint.transform.position));
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                choiceMade = true;
                StartCoroutine(MoveToCover(coverRightPoint.transform.position));
            }
        }#1#
        
        if (currentLineIndex >= lines.Length)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                choiceMade = true;
                dialogText.text = "to the left...";
                StartCoroutine(MoveToCover(coverLeftPoint.transform.position));
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                choiceMade = true;
                dialogText.text = "to the right...";
                StartCoroutine(MoveToCover(coverRightPoint.transform.position));
            }
        }
    }
    
    // ฟังก์ชันแสดงข้อความกลางจอชั่วคราว
    IEnumerator ShowCenterMessage(string message, float duration)
    {
        centerMessageText.text = message;
        centerMessageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        centerMessageText.gameObject.SetActive(false);
    }
    
    IEnumerator AutoShowDialog()
    {
        while (currentLineIndex < lines.Length)
        {
            dialogText.text = lines[currentLineIndex];
            currentLineIndex++;
            yield return new WaitForSeconds(3f); // ระยะเวลารอแต่ละบรรทัด (ปรับได้)
        }

        dialogText.text = "Press E to dodge left or Q to dodge right.";
    }

    IEnumerator StartDialogBeforeChoice()
    {
        currentLineIndex = 0;
        while (currentLineIndex < lines.Length)
        {
            dialogText.text = lines[currentLineIndex];
            currentLineIndex++;
            yield return new WaitForSeconds(2.5f); // ระยะเวลาต่อข้อความ
        }

        // แสดงข้อความกลางจอให้เลือก
        yield return StartCoroutine(ShowCenterMessage("Press E to dodge left or Q to dodge right.", 3f));

        // ตอนนี้เริ่มรอการ input แล้ว
        dialogText.text = "Press E to dodge left or Q to dodge right.";
        waitingForCoverChoice = true;
    }
    
}
*/

using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GlassBreakTriggerManager : MonoBehaviour
{
    [Header("🪟 Glass Settings")]
    public GameObject[] glassObjects;
    public GameObject[] glassShardsObjects;

    [Header("👹 Boss Settings")]
    public GameObject bossObject;
    public GameObject coverLeftPoint;
    public GameObject coverRightPoint;
    public Transform player;
    public float coverSpeed = 5f;

    [Header("🧠 Choice UI")]
    public GameObject choiceUIPanel;
    public TMP_Text dialogText;

    [Header("📢 Mid Screen Message")]
    public TMP_Text centerMessageText;

    [Header("📜 Dialog Lines")]
    public string[] lines;
    private int currentLineIndex = 0;

    private int notesRead = 0;
    private int bodiesChecked = 0;
    private float idleTime = 0f;

    private bool glassBroken = false;
    private bool choiceMade = false;
    private bool waitingForCoverChoice = false;

    public List<string> requiredNotes = new List<string>();
    private List<string> notesReadList = new List<string>();
    
    public List<string> requiredBodies = new List<string>();
    private List<string> bodiesCheckedList = new List<string>();


    void Start()
    {
        bossObject.SetActive(false);
        choiceUIPanel.SetActive(false);

        foreach (var shard in glassShardsObjects)
        {
            shard.SetActive(false);
        }
    }

    void Update()
    {
        /*if (!glassBroken)
        {
            if (Input.anyKey)
                idleTime = 0f;
            else
                idleTime += Time.deltaTime;

            if ((notesRead >= 2 && bodiesChecked >= 2) || idleTime >= 10f)
                BreakAllGlass();
        }

        if (glassBroken && waitingForCoverChoice && !choiceMade)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                choiceMade = true;
                dialogText.text = "Dodging to the left...";
                StartCoroutine(MoveToCover(coverLeftPoint.transform.position));
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                choiceMade = true;
                dialogText.text = "Dodging to the right...";
                StartCoroutine(MoveToCover(coverRightPoint.transform.position));
            }
        }*/
        
        if (!glassBroken)
        {
            if (Input.anyKey)
                idleTime = 0f;
            else
                idleTime += Time.deltaTime;

            // ✅ ถ้า idle ครบ 10 วินาที
            if (idleTime >= 10f)
            {
                BreakAllGlass();
            }
        }

        if (glassBroken && waitingForCoverChoice && !choiceMade)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                choiceMade = true;
                dialogText.text = "Dodging to the left...";
                StartCoroutine(MoveToCover(coverLeftPoint.transform.position));
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                choiceMade = true;
                dialogText.text = "Dodging to the right...";
                StartCoroutine(MoveToCover(coverRightPoint.transform.position));
            }
        }
    }

    void BreakAllGlass()
    {
        Debug.Log("💥 Breaking Glass now!");
        
        glassBroken = true;

        for (int i = 0; i < glassObjects.Length; i++)
        {
            if (glassObjects[i] != null)
                glassObjects[i].SetActive(false);

            if (i < glassShardsObjects.Length && glassShardsObjects[i] != null)
            {
                glassShardsObjects[i].SetActive(true);
                Rigidbody rb = glassShardsObjects[i].GetComponent<Rigidbody>();
                if (rb != null)
                    rb.AddForce(Random.onUnitSphere * 300f);
            }
        }

        // เริ่มแสดง dialog ทีละบรรทัด
        choiceUIPanel.SetActive(true);
        StartCoroutine(AutoShowDialog());
    }

    public void NoteRead(string noteID)
    {
        Debug.Log($"🔎 Trying to read note: {noteID}");

        if (requiredNotes.Contains(noteID) && !notesReadList.Contains(noteID))
        {
            notesReadList.Add(noteID);
            notesRead++;
            Debug.Log($"✅ Note Read: {noteID} (Total: {notesRead})");
            CheckGlassBreakCondition();
        }
        else
        {
            Debug.Log($"❌ Note {noteID} not required or already read.");
        }
    }


    public void BodyChecked()
    {
        bodiesChecked++;
        Debug.Log($"✅ Body Checked (Total: {bodiesChecked})");
        CheckGlassBreakCondition();
    }

    void CheckGlassBreakCondition()
    {
        /*Debug.Log($"Checking if all conditions met: {notesRead} / {requiredNotes.Count}");

        if (notesRead >= requiredNotes.Count)
        {
            Debug.Log("✔️ Glass should break now!");
            BreakAllGlass();
        }*/
        
        Debug.Log($"Checking conditions - NotesRead: {notesRead}, BodiesChecked: {bodiesChecked}");

        if (!glassBroken && (notesRead >= 2 || bodiesChecked >= 2))
        {
            Debug.Log("✔️ Glass should break now due to note or body check!");
            BreakAllGlass();
        }
    }


    IEnumerator MoveToCover(Vector3 targetPosition)
    {
        // ปรับตำแหน่งเป้าหมายให้อยู่ห่างจาก cover จริงนิดนึง
        Vector3 safePosition = targetPosition - (player.forward * 5f); // ห่าง 0.5 หน่วย

        choiceUIPanel.SetActive(false);
        dialogText.text = "";
        if (player.TryGetComponent(out PlayerCoverMovement coverMovement))
            coverMovement.canMove = false;

        StartCoroutine(ShowCenterMessage("[Headhunter] : to take cover", 2f));

        while (Vector3.Distance(player.position, safePosition) > 0.2f)
        {
            player.position = Vector3.MoveTowards(player.position, safePosition, coverSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(4f);
        ShowBoss();
    }




    void ShowBoss()
    {
        bossObject.SetActive(true);
        Debug.Log("👹 Boss Appears!");
    }

    IEnumerator ShowCenterMessage(string message, float duration)
    {
        centerMessageText.text = message;
        centerMessageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        centerMessageText.gameObject.SetActive(false);
    }

    IEnumerator AutoShowDialog()
    {
        while (currentLineIndex < lines.Length)
        {
            dialogText.text = lines[currentLineIndex];
            currentLineIndex++;
            yield return new WaitForSeconds(3f);
        }

        dialogText.text = "Press E (Left) or Q (Right) to take cover.";
        yield return new WaitForSeconds(0.5f); // เพิ่ม delay เล็กน้อยให้ผู้เล่นทันเห็นข้อความ
        waitingForCoverChoice = true; // ✅ ป้องกันเงื่อนไขไม่ทำงาน
    }
    
}
