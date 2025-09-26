using System.Collections;
using playerChar;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set;}
    [Header("Menus")]
    [SerializeField] GameObject pauseMenu;
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

    [Header("Senses UI Settings")]
    [SerializeField] private Image visionCooldownImage;
    [SerializeField] private Image hearingCooldownImage;
    [SerializeField] private Color senseActiveColor = Color.yellow;
    private Color visionOriginalColor;
    private Color hearingOriginalColor;
    
    [Header("Player Warning")]
    [SerializeField] private Image detectionIcon;

    [Header("Stamina")]
    private AudioSource staminaAudioSource;
    [SerializeField, Range(0, 1)] private float staminaAudioThreshold = 0.3f;

    private PlayerCharacterController playerChar;
    private PlayerInteraction playerInteraction;
    private VisionSense visionSense;
    private HearingSense hearingSense;
    private PlayerWarningSystem playerWarningSystem;
    private CustomInput m_Input;

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
        // Find the stamina audio source by tag
        GameObject staminaAudioObject = GameObject.FindWithTag("StaminaAudioSource");
        if (staminaAudioObject != null)
        {
            staminaAudioSource = staminaAudioObject.GetComponent<AudioSource>();
            if (staminaAudioSource == null)
            {
                Debug.LogError("[UIManager] GameObject with tag 'StaminaAudioSource' is missing an AudioSource component.");
            }
        }
        else
        {
            Debug.LogWarning("[UIManager] Could not find GameObject with tag 'StaminaAudioSource'. Stamina audio will not play.");
        }

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
            
            UpdateStaminaEffects(playerChar.CurrentStamina / playerChar.MaxStamina);
            EnableHUD();
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
            if (GameStateManager.Instance.CurrentState == GameState.Paused)
            {
                ExitPauseMenu();
            }
            else if (GameStateManager.Instance.CurrentState == GameState.Gameplay && !GameStateManager.Instance.IsEscapeKeyConsumed())
            {
                CallPauseMenu();
            }
        }

        if (playerChar != null)
        {
            float staminaPercentage = playerChar.CurrentStamina / playerChar.MaxStamina;
            UpdateStaminaEffects(staminaPercentage);

            UpdateSensesUI();
            UpdateCrosshair();
            UpdateWarningUI();
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

        switch (playerInteraction.CurrentCrosshairType)
        {
            case CrosshairType.Interact:
                newSprite = interactCrosshair;
                newColor = interactColor;
                break;
            case CrosshairType.Pickup:
                newSprite = pickupCrosshair;
                newColor = interactColor;
                break;

            case CrosshairType.Door:
                newSprite = doorCrosshair;
                newColor = interactColor;
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
    }

    /// <summary>
    /// Pause Menu
    /// </summary>
    public void CallPauseMenu()
    {
        GameStateManager.Instance.SetState(GameState.Paused);
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        DisableHUD();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitPauseMenu()
    {
        GameStateManager.Instance.SetState(GameState.Gameplay);
        Time.timeScale = 1.0f;
        pauseMenu.SetActive(false);
        EnableHUD();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
    }

    public void EnableHUD()
    {
        HUD.SetActive(true);
    }

    public void DisablePauseMenu()
    {
        pauseMenu.SetActive(false);
    }

    private void UpdateStaminaEffects(float staminaPercentage)
    {
        // Update visual effect
        if (EffectsManager.Instance != null)
        {
            EffectsManager.Instance.UpdateStaminaEffect(staminaPercentage);
        }

        // Update audio effect
        if (staminaAudioSource != null && staminaAudioSource.clip != null)
        {
            if (staminaPercentage <= staminaAudioThreshold)
            {
                if (!staminaAudioSource.isPlaying)
                {
                    staminaAudioSource.loop = true;
                    staminaAudioSource.Play();
                }

                // As stamina decreases towards 0, volume increases towards 1.
                float volume = 1.0f - (staminaPercentage / staminaAudioThreshold);
                staminaAudioSource.volume = Mathf.Clamp01(volume);
            }
            else
            {
                if (staminaAudioSource.isPlaying)
                {
                    staminaAudioSource.Stop();
                }
            }
        }
    }
}
