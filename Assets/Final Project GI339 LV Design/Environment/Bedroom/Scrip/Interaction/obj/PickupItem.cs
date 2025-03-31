using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    public string itemName;

    public void Interact()
    {
        Debug.Log("เก็บไอเท็ม: " + itemName);
        InventoryManager.Instance.AddItem(itemName); // ส่งไอเท็มเข้า Inventory
        Destroy(gameObject); // ลบไอเท็มออกจากฉาก (จำลองการเก็บ)
    }
}