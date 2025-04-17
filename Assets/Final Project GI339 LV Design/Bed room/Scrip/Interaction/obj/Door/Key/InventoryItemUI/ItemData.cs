using UnityEngine;

public class ItemData : MonoBehaviour
{
    public string itemName; // ชื่อไอเท็ม
    [SerializeField] public int width;//1;  // ขนาดของ item ในแนวนอน
    [SerializeField] public int height; // 1; // ขนาดของ item ในแนวตั้ง
    public int ID;
}