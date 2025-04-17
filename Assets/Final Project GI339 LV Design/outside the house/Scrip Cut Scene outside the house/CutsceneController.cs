using UnityEngine;
using System.Collections;

public class CutsceneController : MonoBehaviour
{
    public DoorOpener door;              // ใส่ DoorOpener (ที่แปะอยู่บน DoorGroup)
    public CarMover car;                 // ใส่ CarMover (ที่แปะอยู่บนรถ)
    public DialogueDisplay dialogue;    // ใส่ DialogueDisplay (ไว้แสดงข้อความ)

    public float doorOpenWaitTime = 3f;  // รอหลังประตูเปิดก่อนให้รถเคลื่อน (สามารถปรับให้เข้ากับความเร็วเปิดประตู)

    void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        // 1. เปิดประตู
        door.OpenDoor();
        yield return new WaitForSeconds(doorOpenWaitTime);  // รอจนกว่าประตูจะเปิดได้พอ

        // 2. เริ่มขยับรถ
        car.StartMoving();

        // 3. แสดงไดอะล็อกระหว่างรถวิ่ง (หรือจะให้แสดงตั้งแต่ตอนประตูเปิดก็ได้)
        dialogue.StartDialogue();
    }
}