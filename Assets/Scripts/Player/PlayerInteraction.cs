using playerChar;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float playerReach = 3f;
    public Camera PlayerCamera;

    public CrosshairType CurrentCrosshairType { get; private set; } = CrosshairType.Default;
    
    Interactable currentInteractable;
    bool isInteracting = false;
    private PlayerCharacterController m_PlayerCharacterController;
    private CustomInput m_Input;

    private void Awake()
    {
        m_Input = new CustomInput();
    }

    private void OnEnable()
    {
        m_Input.Player.Enable();
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        }
    }

    private void OnDisable()
    {
        m_Input.Player.Disable();
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }
    }

    void Start()
    {
        m_PlayerCharacterController = GetComponent<PlayerCharacterController>(); 
        if (m_PlayerCharacterController != null)
        {
            PlayerCamera = m_PlayerCharacterController.PlayerCamera;
        }

        // Initialize Crosshair
        CurrentCrosshairType = CrosshairType.Default;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStateManager.Instance != null && GameStateManager.Instance.CurrentState != GameState.Gameplay)
        {
            return;
        }

        // Only check for new interactables if the player is not hiding
        if (m_PlayerCharacterController == null || !m_PlayerCharacterController.IsHiding)
        {
            CheckInteraction();
        }

        // Allow interaction if the key is pressed and there is a current interactable
        if (m_Input.Player.Interact.triggered && currentInteractable != null)
        {
            currentInteractable.Interact(this.gameObject);
        }
    }

    private void HandleGameStateChanged(GameState newState)
    {
        // UIManager will handle crosshair visibility based on player spawn/destroy events
        // and its own game state logic. This script no longer needs to manage it directly.
    }

    public void SetInteracting(bool interacting)
    {
        isInteracting = interacting;
    }

    void ExitInteraction()
    {
        isInteracting = false;
    }

    void CheckInteraction()
    {
        if (PlayerCamera == null)
        {
            Debug.LogWarning("[PlayerInteraction] Player camera is not set!");
            return;
        }

        RaycastHit hit;
        Ray ray = new Ray(PlayerCamera.transform.position, PlayerCamera.transform.forward);
        Interactable newInteractable = null;

        if (Physics.Raycast(ray, out hit, playerReach))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                var interactableComponent = hit.collider.GetComponent<Interactable>();
                if (interactableComponent != null && interactableComponent.enabled)
                {
                    newInteractable = interactableComponent;
                }
            }
        }

        // If the object we're looking at is different from the one we were looking at last frame.
        if (newInteractable != currentInteractable)
        {
            // Disable the outline on the old object.
            if (currentInteractable != null)
            {
                currentInteractable.ReleaseOutline();
            }

            // Set the new object and enable its outline.
            currentInteractable = newInteractable;
            if (currentInteractable != null)
            {
                currentInteractable.RequestOutline();
            }
        }
        
        // Update crosshair based on whether we are looking at an interactable.
        if (currentInteractable != null)
        {
            CurrentCrosshairType = currentInteractable.GetCrosshairType();
        }
        else
        {
            CurrentCrosshairType = CrosshairType.Default;
        }
    }
    
    public void ResetInteraction()
    {
        if (currentInteractable)
        {
            currentInteractable.ReleaseOutline();
            currentInteractable = null;
        }
        CurrentCrosshairType = CrosshairType.Default;
    }
}
