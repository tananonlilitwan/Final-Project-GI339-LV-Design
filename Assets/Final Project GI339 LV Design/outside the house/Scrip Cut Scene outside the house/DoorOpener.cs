using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public Transform leftDoor;         // GameObject หลักที่ต้องหมุน
    public Transform rightDoor;        // GameObject หลักที่ต้องหมุน

    public float openAngle = 90f;  // องศาที่ประตูจะเปิด (90 องศา)
    public float openSpeed = 30f;  // ความเร็วในการเปิดประตู (องศาต่อวินาที)

    private bool isOpening = false; // เช็คว่ากำลังเปิดประตูอยู่หรือไม่
    private float currentAngle = 0f; // เก็บค่ามุมที่เปิดไปแล้ว เพื่อไม่ให้เปิดเกินมุมที่กำหนด

    void Update()
    { 
        // ถ้าอยู่ในสถานะเปิดประตู และยังไม่เปิดครบมุมที่กำหนด
        if (isOpening && currentAngle < openAngle)
        {
            // คำนวณองศาที่จะหมุนในเฟรมนี้
            float rotateStep = openSpeed * Time.deltaTime;

            // ถ้ามีบานซ้ายให้หมุนออกทางซ้าย (ทวนเข็มนาฬิกา)
            if (leftDoor != null)
            {
                leftDoor.Rotate(Vector3.up, -rotateStep);
            }

            // ถ้ามีบานขวาให้หมุนออกทางขวา (ตามเข็มนาฬิกา)
            if (rightDoor != null)
            {
                rightDoor.Rotate(Vector3.up, rotateStep);
            }

            currentAngle += rotateStep; // บวกมุมที่เปิดไปแล้ว เพื่อเช็คว่าจะหยุดเมื่อไหร่
        }
    }

    // ฟังก์ชันเรียกเมื่อจะเริ่มเปิดประตู
    public void OpenDoor()
    {
        isOpening = true;
    }
}