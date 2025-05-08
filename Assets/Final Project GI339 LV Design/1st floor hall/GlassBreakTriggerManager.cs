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
    //private int bodiesChecked = 0;
    private float idleTime = 0f;

    private bool glassBroken = false;
    private bool choiceMade = false;
    private bool waitingForCoverChoice = false;

    public List<string> requiredNotes = new List<string>();
    private List<string> notesReadList = new List<string>();
    
    public List<string> requiredBodies = new List<string> { "police body", "Researcher body" };
    private HashSet<string> bodiesChecked = new HashSet<string>();
    
    [Header("📷 Cameras")]
    public Camera playerCamera;
    public Camera glassCamera;
    
    [Header("📜 Scripts to Reactivate")]
    public DoorRaycaster DoorRaycaster;
    public PlayerInteraction PlayerInteraction;
    
    
    void Start()
    {
        bossObject.SetActive(false);
        choiceUIPanel.SetActive(false);

        foreach (var shard in glassShardsObjects)
        {
            shard.SetActive(false);
        }
        
        playerCamera.enabled = true;
        glassCamera.enabled = false;

    }

    void Update()
    {
        if (!glassBroken)
        {
            if (Input.anyKey)
                idleTime = 0f;
            else
                idleTime += Time.deltaTime;

            // ✅ ถ้า idle ครบ 10 วินาที
            if (idleTime >= 10f)
            {
                //BreakAllGlass();
                BreakAllGlassSequence();
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

    /*void BreakAllGlass()
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
    }*/

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
    
    /*public void BodyChecked(string tag)
    {
        if (requiredBodies.Contains(tag) && !bodiesChecked.Contains(tag))
        {
            bodiesChecked.Add(tag);
            Debug.Log($"✅ Checked body: {tag}");

            if (!glassBroken && bodiesChecked.Count >= requiredBodies.Count)
            {
                Debug.Log("✔️ Both required bodies checked. Breaking glass...");
                BreakAllGlass();
            }
        }
    }*/
    public void BodyChecked(string tag)
    {
        if (requiredBodies.Contains(tag) && !bodiesChecked.Contains(tag))
        {
            bodiesChecked.Add(tag);
            Debug.Log($"✅ Checked body: {tag}");

            CheckGlassBreakCondition();
        }
    }

    void CheckGlassBreakCondition()
    {
        /*Debug.Log($"Checking conditions - NotesRead: {notesRead}, BodiesChecked: {bodiesChecked.Count}");

        if (!glassBroken && (notesRead >= 2 && bodiesChecked.Count >= 2))  
        {
            Debug.Log("✔️ Glass should break now due to note or body check!");
            //BreakAllGlass();
            StartCoroutine(BreakAllGlassSequence());
        }*/
        if (glassBroken) return;

        Debug.Log($"Checking conditions - NotesRead: {notesRead}, BodiesChecked: {bodiesChecked.Count}");

        // เงื่อนไข 1: อ่านโน้ตครบ 2 ชิ้น
        if (notesRead >= 2)
        {
            Debug.Log("✔️ Glass should break now due to notes read!");
            StartCoroutine(BreakAllGlassSequence());
            return;
        }

        // เงื่อนไข 2: ตรวจ body ครบ 2 จุด
        if (bodiesChecked.Count >= requiredBodies.Count)
        {
            Debug.Log("✔️ Glass should break now due to body checks!");
            StartCoroutine(BreakAllGlassSequence());
            return;
        }
    }


    IEnumerator MoveToCover(Vector3 targetPosition)
    {
        // ปรับตำแหน่งเป้าหมายให้อยู่ห่างจาก cover จริงนิดนึง
        Vector3 safePosition = targetPosition - (player.forward * 0f); // ห่าง 0.5 หน่วย

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
        
        SwitchToPlayerCamera();
        
        dialogText.text = "Press (E) or (Q) to take cover."; //Press E (Left) or Q (Right) to take cover.
        yield return new WaitForSeconds(0.5f); // เพิ่ม delay เล็กน้อยให้ผู้เล่นทันเห็นข้อความ
        waitingForCoverChoice = true; // ✅ ป้องกันเงื่อนไขไม่ทำงาน
    }
    
    void SwitchToGlassCamera()
    {
        playerCamera.enabled = false;
        glassCamera.enabled = true;
        
        // ❌ ปิดสคริปต์ชั่วคราว
        if (DoorRaycaster != null)
            DoorRaycaster.enabled = false;

        if (PlayerInteraction != null)
            PlayerInteraction.enabled = false;
    }

    void SwitchToPlayerCamera()
    {
        glassCamera.enabled = false;
        playerCamera.enabled = true;
        
        // ✅ เปิดสคริปต์กลับ
        if (DoorRaycaster != null)
            DoorRaycaster.enabled = true;

        if (PlayerInteraction != null)
            PlayerInteraction.enabled = true;
    }
    
    IEnumerator BreakAllGlassSequence()
    {
        Debug.Log("🎥 Switching to Glass Camera...");
        SwitchToGlassCamera();

        yield return new WaitForSeconds(2f);

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

        choiceUIPanel.SetActive(true);
        StartCoroutine(AutoShowDialog());
    }

    
}
