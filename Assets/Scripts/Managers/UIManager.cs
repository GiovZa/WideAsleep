using playerChar;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set;}
    [Header("Menus")]
    [SerializeField] GameObject pauseMenuObject;
    [SerializeField] GameObject deathScreen;
    [SerializeField] GameObject HUD;
    
    [Header("Crosshair Settings")]
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Sprite defaultCrosshair;
    [SerializeField] private Sprite interactCrosshair;
    [SerializeField] private Sprite pickupCrosshair;
    [SerializeField] private Sprite doorCrosshair;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color interactColor = Color.red;
    [SerializeField] private Color pickupColor = Color.white;
    [SerializeField] private Color doorColor = Color.white;
    private float defaultCrosshairWidth = 10f;
    private float defaultCrosshairHeight = 10f;
    private float interactCrosshairWidth = 10f;
    private float interactCrosshairHeight = 10f;
    private float pickupCrosshairWidth = 80f;
    private float pickupCrosshairHeight = 80f;
    private float doorCrosshairWidth = 80f;
    private float doorCrosshairHeight = 80f;

    [Header("Senses UI Settings")]
    [SerializeField] private Image visionCooldownImage;
    [SerializeField] private Image hearingCooldownImage;
    [SerializeField] private Color senseActiveColor = Color.yellow;
    private Color visionOriginalColor;
    private Color hearingOriginalColor;
    
    [Header("Player Warning")]
    [SerializeField] private Image detectionIcon;

    [Header("Inventory")]
    [SerializeField] private TextMeshProUGUI throwableCountText;
    
    
    private PlayerCharacterController playerChar;
    private PlayerInteraction playerInteraction;
    private VisionSense visionSense;
    private HearingSense hearingSense;
    private PlayerWarningSystem playerWarningSystem;
    private CustomInput m_Input;
    private Stack<IGenericUI> uiStack = new Stack<IGenericUI>();

    public event Action OnHUDEnabled;
    public event Action OnHUDDisabled;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
        m_Input = new CustomInput();
    }

    private void OnEnable()
    {
        m_Input.UI.Enable();
        SceneSwapManager.OnPlayerSpawned += HandlePlayerSpawned;
        SceneSwapManager.OnPlayerWillBeDestroyed += HandlePlayerDestroyed;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        m_Input.UI.Disable();
        SceneSwapManager.OnPlayerSpawned -= HandlePlayerSpawned;
        SceneSwapManager.OnPlayerWillBeDestroyed -= HandlePlayerDestroyed;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (Inventory.Instance != null)
        {
            Inventory.Instance.OnThrowableCountChanged -= UpdateThrowableCountUI;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ensure HUD is disabled in menu scenes.
        if (SceneSwapManager.Instance.MenuSceneNames.Contains(scene.name))
        {
            HandlePlayerDestroyed();
        }
    }

    void Start()
    {
        // Subscribe to events here to ensure other singletons have been initialized in their Awake() methods.
        Inventory.Instance.OnThrowableCountChanged += UpdateThrowableCountUI;

        if (visionCooldownImage != null) visionOriginalColor = visionCooldownImage.color;
        if (hearingCooldownImage != null) hearingOriginalColor = hearingCooldownImage.color;
        if (detectionIcon != null) detectionIcon.enabled = false;
    }

    private void HandlePlayerSpawned(GameObject playerObject)
    {
        playerChar = playerObject.GetComponent<PlayerCharacterController>();
        if (playerChar != null)
        {
            playerInteraction = playerChar.GetComponent<PlayerInteraction>();
            visionSense = playerChar.GetComponent<VisionSense>();
            hearingSense = playerChar.GetComponent<HearingSense>();
            playerWarningSystem = playerChar.GetComponent<PlayerWarningSystem>();
            
            EnableHUD();

            // When the player spawns, update the UI with the current throwable count.
            UpdateThrowableCountUI(Inventory.Instance.ThrowableCount);
        }
    }

    private void HandlePlayerDestroyed()
    {
        playerChar = null;
        playerInteraction = null;
        visionSense = null;
        hearingSense = null;
        playerWarningSystem = null;

        if (detectionIcon != null)
        {
            detectionIcon.enabled = false;
        }
        
        DisableHUD();
    }

    private void Update() 
    {
        if(m_Input.UI.Cancel.triggered)
        {
            // If the stack has any UI open, close the top one.
            if (uiStack.Count > 0)
            {
                CloseActiveUI();
            }
            // If the stack is empty and we are in gameplay, open the pause menu.
            else if (GameStateManager.Instance.CurrentState == GameState.Gameplay)
            {
                CallPauseMenu();
            }
        }

        if (playerChar != null)
        {
            UpdateSensesUI();
            UpdateCrosshair();
            UpdateWarningUI();
        }
    }

    private void UpdateThrowableCountUI(int count)
    {
        if (throwableCountText != null)
        {
            throwableCountText.text = count.ToString();
        }
    }

    private void UpdateSensesUI()
    {
        if (visionSense != null && visionCooldownImage != null)
        {
            visionCooldownImage.fillAmount = visionSense.FillAmount;
            visionCooldownImage.color = visionSense.IsActive ? senseActiveColor : visionOriginalColor;
        }

        if (hearingSense != null && hearingCooldownImage != null)
        {
            hearingCooldownImage.fillAmount = hearingSense.FillAmount;
            hearingCooldownImage.color = hearingSense.IsActive ? senseActiveColor : hearingOriginalColor;
        }
    }

    private void UpdateWarningUI()
    {
        if (playerWarningSystem != null && detectionIcon != null)
        {
            detectionIcon.enabled = playerWarningSystem.IsPlayerSpotted;
        }
    }

    private void UpdateCrosshair()
    {
        if (playerInteraction == null || crosshairImage == null) return;

        bool isGameplay = GameStateManager.Instance.CurrentState == GameState.Gameplay;
        crosshairImage.enabled = isGameplay;

        if (!isGameplay) return;

        Sprite newSprite = defaultCrosshair;
        Color newColor = defaultColor;
        float newWidth = defaultCrosshairWidth;
        float newHeight = defaultCrosshairHeight;

        switch (playerInteraction.CurrentCrosshairType)
        {
            case CrosshairType.Interact:
                newSprite = interactCrosshair;
                newColor = interactColor;
                newWidth = interactCrosshairWidth;
                newHeight = interactCrosshairHeight;
                break;
            case CrosshairType.Pickup:
                newSprite = pickupCrosshair;
                newColor = pickupColor;
                newWidth = pickupCrosshairWidth;
                newHeight = pickupCrosshairHeight;
                break;

            case CrosshairType.Door:
                newSprite = doorCrosshair;
                newColor = doorColor;
                newWidth = doorCrosshairWidth;
                newHeight = doorCrosshairHeight;
                break;
        }
        
        if (crosshairImage.sprite != newSprite)
        {
            crosshairImage.sprite = newSprite;
        }
        if (crosshairImage.color != newColor)
        {
            crosshairImage.color = newColor;
        }
        if (crosshairImage.rectTransform.sizeDelta != new Vector2(newWidth, newHeight))
        {
            crosshairImage.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
        }
    }

    /// <summary>
    /// Pause Menu
    /// </summary>
    public void CallPauseMenu()
    {
        if (pauseMenuObject != null)
        {
            IGenericUI pauseMenu = pauseMenuObject.GetComponent<IGenericUI>();
            if (pauseMenu != null)
            {
                OpenUI(pauseMenu);
                GameStateManager.Instance.SetState(GameState.Paused);
                Time.timeScale = 0f;
                DisableHUD();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Debug.LogError("Pause Menu GameObject is missing a script that implements IGenericUI.");
            }
        }
    }

    public void ExitPauseMenu()
    {
        // This button is now redundant, but we'll have it call the main closing logic.
        CloseActiveUI();
    }
    
    public void ShowDeathScreen()
    {
        deathScreen.SetActive(true);
        DisableHUD();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Handling Interacting with UI
    /// </summary>
    public void DisableHUD()
    {
        HUD.SetActive(false);
        OnHUDDisabled?.Invoke();
    }

    public void EnableHUD()
    {
        HUD.SetActive(true);
        OnHUDEnabled?.Invoke();
    }

    public void DisablePauseMenu()
    {
        // This method is no longer needed as CallPauseMenu and ExitPauseMenu handle the stack.
        // Keeping it for now, but it might be removed if not used elsewhere.
    }    

    // --- New UI Stack Management ---

    /// <summary>
    /// A helper method that is visible in the Unity Inspector for OnClick events.
    /// It finds the IGenericUI component on the passed GameObject and opens it.
    /// </summary>
    public void OpenUIPanel(GameObject uiPanelObject)
    {
        if (uiPanelObject != null)
        {
            IGenericUI uiPanel = uiPanelObject.GetComponent<IGenericUI>();
            if (uiPanel != null)
            {
                OpenUI(uiPanel);
            }
            else
            {
                Debug.LogError($"[UIManager] The GameObject '{uiPanelObject.name}' does not have a component that implements the IGenericUI interface.", uiPanelObject);
            }
        }
    }
    
    public void OpenUI(IGenericUI uiToOpen)
    {
        if (uiToOpen == null)
        {
            Debug.LogError("Attempted to open a null UI.");
            return;
        }

        // If another UI is already open, you might want to hide it.
        // For a stack-based system, we'll assume the new UI overlays the old one.
        
        uiStack.Push(uiToOpen);
        uiToOpen.Open();

        // Opening any UI should pause the game or put it into a UI state.
        //GameStateManager.Instance.SetState(GameState.Paused);

#if UNITY_EDITOR
        Debug_PrintUIStack();
#endif
    }
    
    public void CloseActiveUI()
    {
        if (uiStack.Count > 0)
        {
            IGenericUI uiToClose = uiStack.Pop();
            uiToClose.Close();
            GameStateManager.Instance.ConsumeEscapeKeyForThisFrame();
        }

        if (GameStateManager.Instance.CurrentState == GameState.MainMenu)
        {
            return;
        }

        // If the stack is now empty, we are no longer in a menu.
        if (uiStack.Count == 0 && GameStateManager.Instance.CurrentState != GameState.Gameplay)
        {
            // Resume gameplay
            GameStateManager.Instance.SetState(GameState.Gameplay);
            Time.timeScale = 1.0f;
            EnableHUD();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

#if UNITY_EDITOR
        Debug_PrintUIStack();
#endif
    }
    
    // --- End New UI Stack Management ---

    // --- Debug ---
#if UNITY_EDITOR
    private void Debug_PrintUIStack()
    {
        int index = 0;
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("--- UI Stack State ---");
        if (uiStack.Count == 0)
        {
            sb.AppendLine("(Stack is empty)");
        }
        else
        {
            foreach (var ui in uiStack)
            {
                // Since IGenericUI is implemented on MonoBehaviours, we can cast to get the GameObject name.
                var monoBehaviour = ui as MonoBehaviour;
                string gameObjectName = monoBehaviour != null ? monoBehaviour.gameObject.name : "Unknown GameObject";
                sb.AppendLine($"[{index}] {ui.GetType().Name} on '{gameObjectName}'");
                index++;
            }
        }
        sb.AppendLine("----------------------");
        Debug.Log(sb.ToString());
    }
#endif
    // --- End Debug ---
}
