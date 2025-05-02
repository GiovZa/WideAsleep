using System.Collections;
using System.Collections.Generic;
using playerChar;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float playerReach = 3f;
    public Camera PlayerCamera;
    Interactable currentInteractable;
    bool isInteracting = false;

    void Start()
    {
        PlayerCharacterController playerController = GetComponent<PlayerCharacterController>(); // Replace with the actual script name
        if (playerController != null)
        {
            PlayerCamera = playerController.PlayerCamera;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!isInteracting)
        {
            CheckInteraction();

            if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
            {
                if (currentInteractable is Piano)
                {
                    if (NoteManager.Instance.HasEnoughNotes()) {
                        isInteracting = true;
                    } else {
                        Debug.Log("[Piano] Not enough notes to play the piano!");
                        return;
                    }
                }

                currentInteractable.Interact();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitInteraction();
            }
        }
    }

    void ExitInteraction()
    {
        isInteracting = false;
    }

    void CheckInteraction()
    {
        if (PlayerCamera == null)
        {
            Debug.Log("[Piano] No player camera!");
            return;
        }

        RaycastHit hit;
        Ray ray = new Ray(PlayerCamera.transform.position, PlayerCamera.transform.forward);

        // If colliding with anything within player reach
        if (Physics.Raycast(ray, out hit, playerReach))
        {
            if (hit.collider.tag == "Interactable") // Check if looking at an interactable
            {
                Interactable newInteractable = hit.collider.GetComponent<Interactable>();

                if (currentInteractable &&  newInteractable != currentInteractable)
                {
                    currentInteractable.DisableOutline();
                }

                if (newInteractable.enabled)
                {
                    SetNewCurrentInteractable(newInteractable);
                }
                else
                {
                    DisableCurrentInteractable();
                }
            }
            else
            {
                DisableCurrentInteractable();
            }
        } 
        else
        {
            DisableCurrentInteractable();
        }
    }

    void SetNewCurrentInteractable(Interactable newInteractable)
    {
        currentInteractable = newInteractable;
        currentInteractable.EnableOutline();
    }

    void DisableCurrentInteractable()
    {
        if (currentInteractable)
        {
            currentInteractable.DisableOutline();
            currentInteractable = null;
        }
    }
}
