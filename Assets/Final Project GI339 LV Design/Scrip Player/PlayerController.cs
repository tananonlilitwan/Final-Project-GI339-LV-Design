/*using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // ความเร็วการเดิน
    public float rotationSpeed = 200f; // ความเร็วการหมุนตัวละคร
    public float jumpForce = 8f; // แรงกระโดด
    public float gravity = 20f; // ค่าความเร่งของแรงโน้มถ่วง

    private CharacterController characterController;
    private Vector3 moveDirection;
    private bool isGrounded;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // ตรวจสอบว่าตัวละครสัมผัสพื้นหรือไม่
        isGrounded = characterController.isGrounded;

        // รับค่าปุ่มเดินหน้า-ถอยหลัง
        float verticalInput = Input.GetAxis("Vertical");

        // รับค่าปุ่มหมุนซ้าย-ขวา
        float horizontalInput = Input.GetAxis("Horizontal");

        // หมุนตัวละคร
        transform.Rotate(Vector3.up * horizontalInput * rotationSpeed * Time.deltaTime);

        // ถ้าตัวละครอยู่บนพื้นให้เคลื่อนที่ตามทิศที่หันหน้าไป
        if (isGrounded)
        {
            moveDirection = transform.forward * verticalInput * moveSpeed;

            // กด Spacebar เพื่อกระโดด
            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDirection.y = jumpForce;
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = 8f; // เร็วขึ้นเมื่อกด Shift
        }
        else
        {
            moveSpeed = 5f;
        }


        // ใช้ Gravity ให้ตัวละครตกลง
        moveDirection.y -= gravity * Time.deltaTime;

        // ใช้ Character Controller เคลื่อนที่
        characterController.Move(moveDirection * Time.deltaTime);
    }
}*/

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

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main; // ดึงกล้องหลักมาใช้
    }

    void Update()
    {
        isGrounded = characterController.isGrounded;

        // รับค่าปุ่มเดินหน้า-ถอยหลัง
        float verticalInput = Input.GetAxis("Vertical");

        // หมุนตัวละครให้หันตามกล้อง
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0; // ล็อกแกน Y ไม่ให้ตัวละครก้ม/เงย
        transform.forward = cameraForward.normalized;

        // ถ้าตัวละครอยู่บนพื้นให้เคลื่อนที่ตามทิศที่หันหน้าไป
        if (isGrounded)
        {
            moveDirection = transform.forward * verticalInput * moveSpeed;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDirection.y = jumpForce;
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = 8f;
        }
        else
        {
            moveSpeed = 5f;
        }

        moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
        
        // 🟢 กด "1" เพื่อซ่อน/แสดงปืน
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleGunVisibility();
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
}
