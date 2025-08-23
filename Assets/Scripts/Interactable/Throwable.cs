using UnityEngine;

public class Throwable : Interactable
{
    // This script will be for objects that the player can pick up and throw.

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);

        // Try to get the ObjectThrower component from the interactor (the player).
        ObjectThrower objectThrower = interactor.GetComponent<ObjectThrower>();

        if (objectThrower != null)
        {
            // Add a throwable to the player's inventory.
            objectThrower.AddThrowable();

            // Destroy the object that was picked up.
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("The object interacting with the throwable does not have an ObjectThrower component.");
        }
    }
}
