using UnityEngine;

public class Throwable : Interactable
{
    // This script will be for objects that the player can pick up and throw.

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);

        // Add a throwable to the central inventory.
        Inventory.Instance.AddThrowable();

        // Destroy the object that was picked up.
        Destroy(gameObject);
    }
}
