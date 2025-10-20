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
            AudioManager.Instance.PlaySoundForPlayerOnly(pickupSound, transform.position, 1f, true, AudioManager.Instance.SFXMixerGroup);
        }

        gameObject.SetActive(false); // Disables the note
    }

    public override CrosshairType GetCrosshairType()
    {
        return CrosshairType.Pickup;
    }
}
