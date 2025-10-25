using UnityEngine;
using playerChar;

public class Closet : Interactable
{
    [Header("Hiding Spot")]
    [Tooltip("The transform where the player will be moved to when hiding.")]
    public Transform hidingSpot;

    [Tooltip("The transform where the player will be moved to when exiting the closet.")]
    public Transform exitSpot;

    private bool isPlayerHiding = false;
    private GameObject player;

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        player = interactor;

        if (!isPlayerHiding)
        {
            HidePlayer();
            animator.SetBool("IsOpen", false);
        }
        else
        {
            UnhidePlayer();
            animator.SetBool("IsOpen", true);
        }
    }

    private void HidePlayer()
    {
        PlayerCharacterController playerController = player.GetComponent<PlayerCharacterController>();
        if (playerController != null)
        {
            playerController.SetHidingState(true);
        }

        player.transform.position = hidingSpot.position;
        player.transform.rotation = hidingSpot.rotation;
        playerController.animator.SetBool("IsHiding", true);
        
        Player playerState = player.GetComponent<Player>();
        if (playerState != null)
        {
            playerState.IsHiding = true;
        }

        isPlayerHiding = true;
    }

    private void UnhidePlayer()
    {

        
        PlayerCharacterController playerController = player.GetComponent<PlayerCharacterController>();
        if (playerController != null)
        {
            playerController.SetHidingState(false);
        }

        player.transform.position = exitSpot.position;
        playerController.animator.SetBool("IsHiding", false);

        Player playerState = player.GetComponent<Player>();
        if (playerState != null)
        {
            playerState.IsHiding = false;
        }

        isPlayerHiding = false;
    }
}
