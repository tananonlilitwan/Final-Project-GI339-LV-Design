using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    private Image crosshairImage;

    void Start()
    {
        crosshairImage = GetComponent<Image>();
    }

    public void SetColor(Color color)
    {
        crosshairImage.color = color; // เปลี่ยนสี Crosshair
    }
}