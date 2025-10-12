using UnityEngine;

public class Throwable : Interactable
{
    [SerializeField] private AudioClip[] pickupSounds;

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);

        // Add a throwable to the central inventory.
        Inventory.Instance.AddThrowable();

        if (pickupSounds != null)
        {
            AudioManager.Instance.PlayRandomSoundForPlayerOnly(pickupSounds, transform.position, 1f, true, AudioManager.Instance.SFXMixerGroup);
        }

        // Destroy the object that was picked up.
        Destroy(gameObject);
    }
}
