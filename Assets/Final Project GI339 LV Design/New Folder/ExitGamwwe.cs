using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitGamwwe : MonoBehaviour
{
    public GameObject panel; // ใส่ Panel ที่ต้องการควบคุม

    private bool isPanelActive = false;

    void Update()
    {
        // กดปุ่ม ESC เพื่อเปิด/ปิด Panel
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPanelActive = !isPanelActive;
            panel.SetActive(isPanelActive);
        }

        // กดปุ่ม M เพื่อซ่อน/แสดง Panel
        if (Input.GetKeyDown(KeyCode.M))
        {
            panel.SetActive(!panel.activeSelf);
        }
    }

    public void ExitGame()
    {
        Debug.Log("Game is closing...");
        Application.Quit(); // ใช้ได้จริงเมื่อ Build เป็น .exe หรือ .apk
        
    }

    public void ResetScene()
    {
        Debug.Log("Resetting Scene...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
