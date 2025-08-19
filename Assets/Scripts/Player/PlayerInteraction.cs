using playerChar;
using UnityEngine;
using UnityEngine.UI;

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

    private void OnEnable()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        }
    }

    private void OnDisable()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }
    }

    void Start()
    {
        PlayerCharacterController playerController = GetComponent<PlayerCharacterController>(); // Replace with the actual script name
        if (playerController != null)
        {
            PlayerCamera = playerController.PlayerCamera;
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

        CheckInteraction();

        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.Interact();
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
            Debug.Log("[Piano] No player camera!");
            return;
        }

        RaycastHit hit;
        Ray ray = new Ray(PlayerCamera.transform.position, PlayerCamera.transform.forward);

        // If colliding with anything within player reach
        if (Physics.Raycast(ray, out hit, playerReach))
        {
            if (hit.collider.CompareTag("Interactable")) // Check if looking at an interactable
            {
                Interactable newInteractable = hit.collider.GetComponent<Interactable>();

                if (currentInteractable && newInteractable != currentInteractable)
                {
                    currentInteractable.DisableOutline();
                }

                if (newInteractable != null && newInteractable.enabled)
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
        SetCrosshairType(currentInteractable.GetCrosshairType());
    }

    void DisableCurrentInteractable()
    {
        if (currentInteractable)
        {
            currentInteractable.DisableOutline();
            currentInteractable = null;
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
            DisableCurrentInteractable();
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
