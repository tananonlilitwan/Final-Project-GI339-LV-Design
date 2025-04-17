using UnityEngine;
using TMPro;
using System.Collections;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TextMeshProUGUI messageText;

    void Awake()
    {
        Instance = this;
    }

    public void ShowMessage(string message, float duration = 2f)
    {
        StopAllCoroutines();
        StartCoroutine(ShowMessageRoutine(message, duration));
    }

    IEnumerator ShowMessageRoutine(string message, float duration)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(duration);
        messageText.gameObject.SetActive(false);
    }
}

