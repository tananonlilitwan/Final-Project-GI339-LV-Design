using UnityEngine;

public class NoteComponent : MonoBehaviour
{
    [TextArea(5, 10)]
    public string noteContent = "This is a test note.";
}