/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; // Singleton
    public GameObject inventoryPanel; // UI ของกระเป๋า
    public Transform itemContainer; // Grid ช่องเก็บของ
    public GameObject itemPrefab; // Prefab ไอเท็ม

    private List<string> items = new List<string>();

    private void Awake()
    {
        Instance = this; // กำหนด Singleton
        inventoryPanel.SetActive(false); // ซ่อนกระเป๋าตอนเริ่มเกม
    }

    private void Update()
    {
        // กดปุ่ม B เพื่อเปิด/ปิดกระเป๋า
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }

    public void AddItem(string itemName)
    {
        items.Add(itemName);
        UpdateInventoryUI();
    }

    private void UpdateInventoryUI()
    {
        // ลบไอเท็มเก่าออกก่อน
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }

        // แสดงไอเท็มที่เก็บ
        foreach (string item in items)
        {
            GameObject newItem = Instantiate(itemPrefab, itemContainer);
            newItem.GetComponentInChildren<Text>().text = item; // แสดงชื่อไอเท็ม
        }
    }
}*/



using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public GameObject inventoryPanel; // UI ของกระเป๋า
    public Transform itemContainer; // Grid ช่องเก็บของ
    public GameObject itemSlotPrefab; // Prefab ช่องเก็บของ (Slot)
    public GameObject[] itemPrefab; // Prefab ไอเท็ม (หลายๆ ชนิด)

    public int gridWidth = 6; // จำนวนช่องแนวนอน
    public int gridHeight = 4; // จำนวนช่องแนวตั้ง
    private int[,] grid; // เก็บสถานะของแต่ละช่อง

    private List<string> items = new List<string>(); // ไอเท็มที่เก็บเป็นชื่อ

    private void Awake()
    {
        Instance = this;
        inventoryPanel.SetActive(false);
        grid = new int[gridWidth, gridHeight]; // สร้าง Grid Inventory
        CreateInventorySlots(); // สร้างช่องเก็บของ
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }

    private void CreateInventorySlots()
    {
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject slot = Instantiate(itemSlotPrefab, itemContainer);
                slot.name = $"Slot ({x},{y})";
            }
        }
    }

    // ฟังก์ชันสำหรับเพิ่มไอเท็มลงในกระเป๋า
    public bool AddItemToInventory(GameObject itemPrefab)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] == 0) // หาช่องว่าง
                {
                    PlaceItem(itemPrefab, x, y);
                    return true;
                }
            }
        }

        Debug.Log("Inventory เต็ม!");
        return false;
    }

    private void PlaceItem(GameObject itemPrefab, int x, int y)
    {
        GameObject item = Instantiate(itemPrefab, itemContainer);
        item.transform.SetSiblingIndex(y * gridWidth + x);
        grid[x, y] = 1; // บันทึกตำแหน่งที่มีการใช้
        items.Add(itemPrefab.name); // บันทึกชื่อของไอเท็มที่เพิ่ม
    }

    // ฟังก์ชันนี้เพิ่มชื่อของไอเท็มเข้าไปใน List และอัปเดต UI
    public void AddItem(string itemName)
    {
        items.Add(itemName); 
        UpdateInventoryUI();
    }

    // ฟังก์ชันนี้อัปเดต UI ของ Inventory
    private void UpdateInventoryUI()
    {
        // ลบไอเท็มเก่าออกก่อน
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }

        // แสดงไอเท็มที่เก็บ
        for (int i = 0; i < items.Count; i++)
        {
            // เลือก Prefab ตามชื่อใน List
            GameObject newItem = Instantiate(itemPrefab[i % itemPrefab.Length], itemContainer);
            newItem.GetComponentInChildren<Text>().text = items[i]; // แสดงชื่อไอเท็ม
        }
    }
}
