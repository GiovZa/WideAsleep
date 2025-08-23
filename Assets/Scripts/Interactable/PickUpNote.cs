using UnityEngine;

public class PickupNote : Interactable
{
    [SerializeField] private AudioClip pickupSound;

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor); // Calls the base message + UnityEvent
        Debug.Log("Note picked up!");

        NoteManager.Instance.CollectNote();

        if (pickupSound != null)
        {
            AudioManager.Instance.PlaySoundForPlayerOnly(pickupSound, transform.position, 1f, true);
        }

        gameObject.SetActive(false); // Disables the note
    }
}
