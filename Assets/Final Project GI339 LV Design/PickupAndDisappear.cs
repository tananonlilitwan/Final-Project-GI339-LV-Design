using UnityEngine;

public class PickupAndDisappear : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Object picked up and removed from scene.");
        Destroy(gameObject); // ลบวัตถุออกจาก Scene ทันที
    }
}
