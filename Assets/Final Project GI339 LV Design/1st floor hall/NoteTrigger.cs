using UnityEngine;

public class NoteTrigger : MonoBehaviour
{
    public string noteID;
    private bool hasRead = false;

    /*void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E) && !hasRead)
        {
            FindObjectOfType<GlassBreakTriggerManager>().NoteRead(noteID);
            hasRead = true;
            Debug.Log("📖 Read Note: " + noteID);
        }
    }*/
    
    public bool HasRead()
    {
        return hasRead;
    }

    public void ReadNote()
    {
        if (!hasRead)
        {
            FindObjectOfType<GlassBreakTriggerManager>().NoteRead(noteID);
            hasRead = true;
            Debug.Log("📖 Read Note: " + noteID);
        }
    }
}