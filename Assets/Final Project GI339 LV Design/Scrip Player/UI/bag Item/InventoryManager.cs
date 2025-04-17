using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Inventory UI")]
    public GameObject inventoryPanel;
    public Transform itemContainer; // ใช้ GridLayoutGroup
    public GameObject[] itemPrefab;
    public TextMeshProUGUI capacityText; // แสดงความจุของกระเป๋า

    [Header("Grid Settings")]
    [SerializeField] public int gridWidth;
    [SerializeField] public int gridHeight;
    [SerializeField] public int slotSize;

    private int[,] grid;
    private List<string> items = new List<string>();

    private void Awake()
    {
        Instance = this;
        inventoryPanel.SetActive(false);
        grid = new int[gridWidth, gridHeight];
        UpdateCapacityText();
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

    public bool AddItemToInventory(GameObject itemPrefab)
    {
        ItemData data = itemPrefab.GetComponent<ItemData>();
        if (data == null)
        {
            Debug.LogWarning("❗ Item ไม่มี ItemData component!");
            return false;
        }

        for (int y = 0; y <= gridHeight - data.height; y++)
        {
            for (int x = 0; x <= gridWidth - data.width; x++)
            {
                if (CheckSpaceAvailable(x, y, data.width, data.height))
                {
                    PlaceItem(itemPrefab, x, y, data.width, data.height);
                    items.Add(itemPrefab.name);
                    UpdateCapacityText();
                    return true;
                }
            }
        }

        Debug.Log("❌ Inventory เต็ม หรือไม่มีช่องว่างขนาดพอ!");
        return false;
    }

    private bool CheckSpaceAvailable(int startX, int startY, int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[startX + x, startY + y] != 0)
                    return false;
            }
        }
        return true;
    }

    private void PlaceItem(GameObject itemPrefab, int x, int y, int width, int height)
    {
        GameObject itemGO = Instantiate(itemPrefab, itemContainer);
        RectTransform itemRT = itemGO.GetComponent<RectTransform>();

        if (itemRT != null)
        {
            itemRT.anchorMin = new Vector2(0, 1);
            itemRT.anchorMax = new Vector2(0, 1);
            itemRT.pivot = new Vector2(0, 1);
            itemRT.anchoredPosition = new Vector2(x * slotSize, -y * slotSize);
            itemRT.sizeDelta = new Vector2(width * slotSize, height * slotSize);
        }

        for (int offsetY = 0; offsetY < height; offsetY++)
        {
            for (int offsetX = 0; offsetX < width; offsetX++)
            {
                grid[x + offsetX, y + offsetY] = 1;
            }
        }
    }

    private void UpdateCapacityText()
    {
        int used = 0;
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] == 1) used++;
            }
        }

        int total = gridWidth * gridHeight;
        capacityText.text = $"\uD83D\uDCBC ความจุ: {used}/{total} ช่อง";
    }
    
    // ฟังก์ชันนี้เพิ่มชื่อของไอเท็มเข้าไปใน List และอัปเดต UI
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
        for (int i = 0; i < items.Count; i++)
        {
            GameObject newItem = Instantiate(itemPrefab[i % itemPrefab.Length], itemContainer);
            
            // ✅ รองรับทั้ง Text และ TextMeshProUGUI
            var legacyText = newItem.GetComponentInChildren<Text>();
            var tmpText = newItem.GetComponentInChildren<TextMeshProUGUI>();

            if (tmpText != null)
            {
                tmpText.text = items[i];
            }
            else if (legacyText != null)
            {
                legacyText.text = items[i];
            }
            else
            {
                Debug.LogWarning($"⚠️ Item '{items[i]}' ไม่มี Text หรือ TextMeshPro อยู่ใน Prefab!");
            }
        }
    }
    
    public void RemoveItem(string itemID)
    {
        // ค้นหาไอเท็มใน UI ที่ตรงกับ itemID แล้วลบออก
        foreach (Transform item in itemContainer.transform)
        {
            InventoryItemUI itemUI = item.GetComponent<InventoryItemUI>();
            if (itemUI != null && itemUI.itemID == itemID)
            {
                Destroy(item.gameObject);
                break; // ลบแค่อันแรกที่เจอ
            }
        }
    }
}
