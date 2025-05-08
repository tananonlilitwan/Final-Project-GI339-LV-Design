using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Singleton Instance")]
    public static PlayerInventory Instance; // ใช้ Singleton เพื่อเรียกใช้งานจากที่ไหนก็ได้
    
    [Header("Singleton Instance")]
    private List<int> keys = new List<int>(); // รายการกุญแจที่ผู้เล่นมี

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // กำหนด Instance ในการเริ่มต้น
            DontDestroyOnLoad(gameObject); // 🔒 ป้องกันถูกลบเมื่อลากฉากใหม่
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // ป้องกัน duplicate
        }
    }

    // ฟังก์ชันเพิ่มกุญแจ
    public void AddKey(int keyID)
    {
        if (!keys.Contains(keyID)) // ตรวจสอบว่ามีกุญแจนี้อยู่แล้วหรือไม่
        {
            keys.Add(keyID); // เพิ่มกุญแจ
            Debug.Log("เก็บกุญแจหมายเลข: " + keyID);
        }
        else
        {
            Debug.Log("กุญแจหมายเลข " + keyID + " มีอยู่แล้ว");
        }
    }

    // ฟังก์ชันตรวจสอบว่ามีกุญแจที่มี keyID หรือไม่
    public bool HasKey(int keyID)
    {
        return keys.Contains(keyID); // คืนค่าจริงถ้ามีกุญแจ
    }

    // ฟังก์ชันใช้กุญแจ
    public void UseKey(int keyID)
    {
        if (keys.Contains(keyID)) // ตรวจสอบว่ามีกุญแจนี้ใน inventory
        {
            keys.Remove(keyID); // ลบกุญแจออกจาก inventory
            Debug.Log("ใช้กุญแจหมายเลข: " + keyID);
        }
        else
        {
            Debug.Log("ไม่มีกุญแจหมายเลข " + keyID + " ใน inventory");
        }
    }

    // ฟังก์ชันลบกุญแจจาก inventory
    public void RemoveKey(int keyID)
    {
        if (keys.Contains(keyID)) // ตรวจสอบว่ามีกุญแจนี้ใน inventory
        {
            keys.Remove(keyID); // ลบกุญแจ
            Debug.Log("ลบกุญแจหมายเลข: " + keyID);
        }
        else
        {
            Debug.Log("ไม่มีกุญแจหมายเลข " + keyID + " ใน inventory");
        }
    }
}
