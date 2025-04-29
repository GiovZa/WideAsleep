using UnityEngine;

public class PickupNote : Interactable
{
    public override void Interact()
    {
        base.Interact(); // Calls the base message + UnityEvent
        Debug.Log("Note picked up!");

        NoteManager.Instance.CollectNote();

        gameObject.SetActive(false); // Disables the note
    }
}
