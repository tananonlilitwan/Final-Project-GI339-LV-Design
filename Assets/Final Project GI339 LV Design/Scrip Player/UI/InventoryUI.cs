using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject itemPrefab;
    public Transform itemContainer;

    public void ToggleInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf); // เปิด/ปิด UI กระเป๋า
    }

    public void AddItemToUI(string itemName)
    {
        GameObject newItem = Instantiate(itemPrefab, itemContainer);
        newItem.GetComponentInChildren<Text>().text = itemName; // แสดงชื่อไอเท็ม
    }
}