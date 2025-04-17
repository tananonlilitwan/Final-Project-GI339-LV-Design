using TMPro;
using UnityEngine;
using System.Collections;

public class DialogueDisplay : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;   // ตัวแปรอ้างถึง TextMeshPro ที่ใช้แสดงบทพูดบนหน้าจอ
    public string[] lines;   // ข้อความที่จะแสดงทีละบรรทัด (บทพูด)
    public float delayBetweenLines = 3f;  // ระยะเวลาหน่วงระหว่างแต่ละบรรทัด

    // ฟังก์ชันเริ่มต้นบทสนทนา
    public void StartDialogue() 
    {
        StartCoroutine(ShowDialogue());  // เริ่มทำงาน Coroutine ที่ชื่อว่า ShowDialogue
    }

    // Coroutine สำหรับแสดงบทพูดทีละบรรทัด
    IEnumerator ShowDialogue()  
    {
        // วนลูปผ่านทุกข้อความใน lines
        foreach (var line in lines)
        {
            dialogueText.text = line; // แสดงข้อความปัจจุบันบนหน้าจอ
            yield return new WaitForSeconds(delayBetweenLines); // รอเวลาตามที่กำหนดไว้ก่อนแสดงบรรทัดถัดไป
        }

        // ล้างข้อความหลังแสดงครบทุกบรรทัด
        dialogueText.text = "";
    }
}