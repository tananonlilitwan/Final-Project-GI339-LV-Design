using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float gravity = 20f;
    public GameObject gun; // ปืนที่จะแสดง/ซ่อน

    private CharacterController characterController;
    private Vector3 moveDirection;
    private bool isGrounded;
    private Camera mainCamera; // กล้องหลัก
    private bool isGunVisible = true; // สถานะของปืน
    //private bool isInventoryOpen = false; // เช็คสถานะของกระเป๋า
    [SerializeField] private bool isInventoryOpen = false;

    
    public static PlayerController Instance; 
    
    private bool isLockedByShowcase = false;

    private void Awake()
    {
        Instance = this;
    }

    
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
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
    }

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
    // ฟังก์ชันซ่อน/แสดงปืน
    private void ToggleGunVisibility()
    {
        isGunVisible = !isGunVisible;
        if (gun != null)
        {
            gun.SetActive(isGunVisible);
        }
    }
    
    // ฟังก์ชันเปิด/ปิดกระเป๋า
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        LockCamera(isInventoryOpen); // ปลดล็อก/ล็อกเมาส์

        // ✅ ส่งสถานะไปยัง CameraController และ GunController
        Camera.main.GetComponent<CameraController>().isInventoryOpen = isInventoryOpen;
        gun.GetComponent<GunController>().isInventoryOpen = isInventoryOpen;
    }
    

    public void LockControlByShowcase(bool shouldLock)
    {
        isLockedByShowcase = shouldLock;
        LockCamera(shouldLock); // ใช้ล็อกเมาส์ร่วมกันเลย
        
        // ✅ ส่งสถานะไปยัง CameraController ด้วย
        Camera.main.GetComponent<CameraController>().isLockedByShowcase = shouldLock;
        gun.GetComponent<GunController>().isLockedByShowcase = shouldLock;

    }
    public void SetInventoryState(bool state)
    {
        isInventoryOpen = state;
        Camera.main.GetComponent<CameraController>().isInventoryOpen = state; // ส่งค่าไปที่ CameraController
        gun.GetComponent<GunController>().isInventoryOpen = state; // ส่งค่าไปที่ GunController
    }



}
