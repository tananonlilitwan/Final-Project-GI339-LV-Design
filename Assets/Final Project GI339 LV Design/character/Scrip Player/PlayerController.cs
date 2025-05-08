using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float gravity = 20f;
    
    [Header("Gun Settings")]
    public GameObject gun; // ปืนที่จะแสดง/ซ่อน
    private bool isGunVisible = true; // สถานะของปืน
    
    [Header("Health Settings")]
    [SerializeField] public int maxHP;
    private int currentHP;

    [Header("Inventory Settings")]
    [SerializeField] private bool isInventoryOpen = false;
    
    [Header("References")]
    private CharacterController characterController;
    private Camera mainCamera; // กล้องหลัก
    
    [Header("System State")]
    public static PlayerController Instance;
    private Vector3 moveDirection;
    private bool isGrounded;
    private bool isLockedByShowcase = false;
    //private bool isInventoryOpen = false; // เช็คสถานะของกระเป๋า
    
    
    // -------------------- MonoBehaviour Methods --------------------
    
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        
        currentHP = maxHP; // เริ่มด้วยพลังเต็ม
    }

    private void Update()
    {
        if (isLockedByShowcase) return; // 🔒 หยุดควบคุมถ้าโชว์ไอเท็มอยู่
        
        if (!isInventoryOpen) // ถ้ากระเป๋าปิด → ควบคุมตัวละครได้
        {
            HandleMovement();
        }

        // 🟢 กด "B" เพื่อเปิด/ปิดกระเป๋า
        if (Input.GetKeyDown(KeyCode.B))
        {
             ToggleInventory();
        }
        // 🟢 กด "1" เพื่อซ่อน/แสดงปืน
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleGunVisibility();
        }

        HandleInput();
    }
    
    // -------------------- Input Handling --------------------

    private void HandleInput()
    {
        // 🟢 กด "B" เพื่อเปิด/ปิดกระเป๋า
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleInventory();
        }
        // 🟢 กด "1" เพื่อซ่อน/แสดงปืน
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleGunVisibility();
        }
    }

    // -------------------- Movement --------------------
    private void HandleMovement()
    {
        isGrounded = characterController.isGrounded;
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0;
        transform.forward = cameraForward.normalized;

        if (isGrounded)
        {
            moveDirection = transform.forward * verticalInput * moveSpeed + transform.right * horizontalInput * moveSpeed;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDirection.y = jumpForce;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
    }
    private void LockCamera(bool shouldLock)
    {
        if (shouldLock)
        {
            Cursor.lockState = CursorLockMode.None; // ปลดล็อกเมาส์
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // ล็อกเมาส์
            Cursor.visible = false;
        }
    }
    // -------------------- Inventory --------------------
    // ฟังก์ชันเปิด/ปิดกระเป๋า
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        LockCamera(isInventoryOpen); // ปลดล็อก/ล็อกเมาส์

        // ✅ ส่งสถานะไปยัง CameraController และ GunController
        Camera.main.GetComponent<CameraController>().isInventoryOpen = isInventoryOpen;
        gun.GetComponent<GunController>().isInventoryOpen = isInventoryOpen;
    }
    
    public void SetInventoryState(bool state)
    {
        isInventoryOpen = state;
        Camera.main.GetComponent<CameraController>().isInventoryOpen = state; // ส่งค่าไปที่ CameraController
        gun.GetComponent<GunController>().isInventoryOpen = state; // ส่งค่าไปที่ GunController
    }
    // -------------------- Gun --------------------
    // ฟังก์ชันซ่อน/แสดงปืน
    private void ToggleGunVisibility()
    {
        isGunVisible = !isGunVisible;
        if (gun != null)
        {
            gun.SetActive(isGunVisible);
        }
    }
    // -------------------- Showcase Lock --------------------
    public void LockControlByShowcase(bool shouldLock)
    {
        isLockedByShowcase = shouldLock;
        LockCamera(shouldLock); // ใช้ล็อกเมาส์ร่วมกันเลย
        
        // ✅ ส่งสถานะไปยัง CameraController ด้วย
        Camera.main.GetComponent<CameraController>().isLockedByShowcase = shouldLock;
        gun.GetComponent<GunController>().isLockedByShowcase = shouldLock;

    }
    // -------------------- Health --------------------
    public void TakeDamage(int amount)
    {
        if (currentHP <= 0) return; // ถ้า HP เป็น 0 อยู่แล้ว ไม่ต้องทำอะไร

        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP); // ป้องกัน HP ติดลบ

        Debug.Log("Player HP: " + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        Debug.Log("Player has died.");
        gameObject.SetActive(false); // ซ่อนผู้เล่น
    }
    
}
