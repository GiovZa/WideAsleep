using System.Collections;
using playerChar;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set;}
    [Header("Menus")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject deathScreen;
    [SerializeField] GameObject HUD;
    private PlayerCharacterController player;
    private PlayerInteraction playerInteraction;
    private CustomInput m_Input;

    [Header("Notes")]
    [SerializeField] int notesCollected = 0;
    [SerializeField] int totalNotes = 5;
    public Sprite paperNote;
    public Sprite emptyPaperNote;
    public Image[] notes;

    [Header("Stamina")]
    private AudioSource staminaAudioSource;
    [SerializeField, Range(0, 1)] private float staminaAudioThreshold = 0.3f;

    private void Awake()
    {
        Instance = this;
        m_Input = new CustomInput();
    }

    private void OnEnable()
    {
        m_Input.UI.Enable();
    }

    private void OnDisable()
    {
        m_Input.UI.Disable();
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

        notesCollected = NoteManager.Instance.GetNoteCount();
        totalNotes = NoteManager.Instance.requiredNotes;
        UpdateNotesHUD();

        player = FindObjectOfType<PlayerCharacterController>();
        if (player != null)
        {
            playerInteraction = player.GetComponent<PlayerInteraction>();
            UpdateStaminaEffects(player.CurrentStamina / player.MaxStamina);
        }
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

        if (player != null)
        {
            float staminaPercentage = player.CurrentStamina / player.MaxStamina;
            UpdateStaminaEffects(staminaPercentage);
        }
    }

    /// <summary>
    /// Notes Collection
    /// </summary>
    public void OnNoteCollected()
    {
        notesCollected ++;
        UpdateNotesHUD();
    }

    void UpdateNotesHUD()
    {
        for (int i = 0; i < notes.Length; i++)
        {
            if (i < notesCollected)
                notes[i].sprite = paperNote;
            else
                notes[i].sprite = emptyPaperNote;
            
            if (i < totalNotes)
                notes[i].enabled = true;
            else
                notes[i].enabled = false;
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
        playerInteraction.SetCrosshairVisible(false);
        DisableHUD();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitPauseMenu()
    {
        GameStateManager.Instance.SetState(GameState.Gameplay);
        Time.timeScale = 1.0f;
        pauseMenu.SetActive(false);
        playerInteraction.SetCrosshairVisible(true);
        EnableHUD();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShowDeathScreen()
    {
        deathScreen.SetActive(true);
        playerInteraction.SetCrosshairVisible(false);
        DisableHUD();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitToMainMenu()
    {
        SceneSwapManager.Instance.ExitToMainMenu();
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
