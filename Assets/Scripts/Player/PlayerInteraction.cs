using playerChar;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float playerReach = 3f;
    public Camera PlayerCamera;

    [Header("Crosshair UI")]
    [SerializeField] private Image crosshairImage;

    [Header("Crosshair Sprite")]
    [SerializeField] private Sprite defaultCrosshair;
    [SerializeField] private Sprite interactCrosshair;
    [SerializeField] private Sprite pickupCrosshair;
    [SerializeField] private Sprite doorCrosshair;

    [Header("Crosshair Color")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color interactColor = Color.red;

    Interactable currentInteractable;
    bool isInteracting = false;
    CrosshairType currentCrosshairType = CrosshairType.Default;
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
        SetCrosshairType(CrosshairType.Default);
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
            SetCrosshairType(currentInteractable.GetCrosshairType());
        }
        else
        {
            SetCrosshairType(CrosshairType.Default);
        }
    }
    
    /// <summary>
    /// Crosshair related methods
    /// </summary>
    /// <param name="type"></param>
    void SetCrosshairType(CrosshairType type)
    {
        if (currentCrosshairType == type) return;
        
        currentCrosshairType = type;
        
        switch (type)
        {
            case CrosshairType.Default:
                UpdateCrosshair(defaultCrosshair, defaultColor);
                break;
            case CrosshairType.Interact:
                UpdateCrosshair(interactCrosshair, interactColor);
                break;
            case CrosshairType.Pickup:
                UpdateCrosshair(pickupCrosshair, interactColor);
                break;
            case CrosshairType.Door:
                UpdateCrosshair(doorCrosshair, interactColor);
                break;
        }
    }
    
    void UpdateCrosshair(Sprite sprite, Color color)
    {
        if (crosshairImage != null)
        {
            if (sprite != null)
            {
                crosshairImage.sprite = sprite;
            }
            crosshairImage.color = color;
        }
    }
    
    private void HandleGameStateChanged(GameState newState)
    {
        bool isGameplay = newState == GameState.Gameplay;
        SetCrosshairVisible(isGameplay);

        if (!isGameplay && currentInteractable)
        {
            currentInteractable.ReleaseOutline();
            currentInteractable = null;
            SetCrosshairType(CrosshairType.Default);
        }
    }

    public void SetCrosshairVisible(bool visible)
    {
        if (crosshairImage != null)
        {
            crosshairImage.enabled = visible;
        }
    }
    
    // void EnsureCrosshairCentered()
    // {
    //     if (crosshairImage != null)
    //     {
    //         RectTransform rect = crosshairImage.GetComponent<RectTransform>();
    //         rect.anchorMin = new Vector2(0.5f, 0.5f);
    //         rect.anchorMax = new Vector2(0.5f, 0.5f);
    //         rect.anchoredPosition = Vector2.zero;
    //         crosshairImage.gameObject.SetActive(true);
    //     }
    // }
}
