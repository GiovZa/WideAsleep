using UnityEngine;

public class PickupNote : Interactable
{
    [SerializeField] private AudioClip pickupSound;

    private void Awake()
    {
        pickupSound = GetComponent<AudioSource>().clip;
    }

    public override void Interact()
    {
        base.Interact(); // Calls the base message + UnityEvent
        Debug.Log("Note picked up!");

        NoteManager.Instance.CollectNote();

        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        gameObject.SetActive(false); // Disables the note
    }
}
