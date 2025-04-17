using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public int keyID; // หมายเลขของกุ
    
    [Header("3D Item Showcase")]
    public GameObject item3DPrefab; // ✅ 3DItem

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddKey(keyID); // เพิ่มกุญแจให้ผู้เล่น
            Destroy(gameObject); // ทำลายกุญแจหลังเก็บ
        }
    }
}