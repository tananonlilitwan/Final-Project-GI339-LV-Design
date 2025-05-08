using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TeleportTrigger : MonoBehaviour
{
    [SerializeField] private Transform teleportTarget;  // จุดที่ต้องการให้วาร์ปไป
    [SerializeField] private GameObject gameEndPanel;   // Panel จบเกม (ลากจาก Inspector)
    [SerializeField] private string sceneName;          // ชื่อของฉากที่จะรีสตาร์ท (ในกรณีที่รีสตาร์ทเกม)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = teleportTarget.position; // วาร์ปไปยังจุดเป้าหมาย
            StartCoroutine(ShowEndPanelAfterDelay());
        }
    }

    private IEnumerator ShowEndPanelAfterDelay()
    {
        yield return new WaitForSeconds(1f); // รอ 1 วินาที
        if (gameEndPanel != null)
        {
            gameEndPanel.SetActive(true); // เปิด Panel จบเกม
            Debug.Log("เกมจบแล้ว");
        }
    }

    // ฟังก์ชันสำหรับออกเกม
    public void ExitGame()
    {
        Debug.Log("ออกจากเกม...");
        Application.Quit(); // ออกจากเกม
    }

    // ฟังก์ชันสำหรับรีสตาร์ทเกม
    public void RestartGame()
    {
        Debug.Log("เริ่มเกมใหม่...");
        SceneManager.LoadScene(sceneName); // โหลดฉากใหม่ (รีสตาร์ทเกม)
    }
}