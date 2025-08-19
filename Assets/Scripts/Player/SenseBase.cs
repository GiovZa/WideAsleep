using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Base class for player senses like vision and hearing.
/// Handles common functionality such as activation input and cooldowns.
/// </summary>
public abstract class SenseBase : MonoBehaviour
{
    // Defines the possible states of the sense ability.
    protected enum SenseState { Ready, Active, Cooldown }
    protected SenseState currentState = SenseState.Ready;

    [Header("Input Settings")]
    [Tooltip("The key to press to activate this sense.")]
    public KeyCode activateKey;

    [Header("Cooldown Settings")]
    [Tooltip("How long to wait before this sense can be used again.")]
    [SerializeField] private float cooldownDuration = 5f;

    [Header("UI Settings")]
    [Tooltip("The UI Image to use for the cooldown indicator. Must be a radial fill image.")]
    [SerializeField] private Image cooldownImage;
    [Tooltip("The color of the indicator when the sense is active.")]
    [SerializeField] private Color activeColor = Color.yellow;
    
    private Color originalColor;
    protected EffectsManager effectsManager;
    private float effectEndTime;
    private float cooldownEndTime;
    private bool isSensesActive = true;

    /// <summary>
    /// The duration of the sense's effect. To be implemented by subclasses.
    /// </summary>
    protected abstract float EffectDuration { get; }

    protected virtual void Start()
    {
        effectsManager = EffectsManager.Instance;
        if (effectsManager == null)
        {
            Debug.LogError("EffectsManager instance not found. Make sure an EffectsManager is active in the scene.");
        }

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameStateChanged += HandleGameStateChanged;
            // Initialize based on the current state in case this script starts after the initial state is set
            HandleGameStateChanged(GameStateManager.Instance.CurrentState);
        }
        else
        {
            Debug.LogError("GameStateManager instance not found. Senses will not respond to game state changes.");
        }

        if (cooldownImage != null)
        {
            originalColor = cooldownImage.color;
            cooldownImage.fillAmount = 1; // Start with the indicator full
        }
    }

    protected virtual void Update()
    {
        switch (currentState)
        {
            case SenseState.Ready:
                HandleReadyState();
                break;
            case SenseState.Active:
                HandleActiveState();
                break;
            case SenseState.Cooldown:
                HandleCooldownState();
                break;
        }
    }

    /// <summary>
    /// Handles the "Ready" state, waiting for player input to activate the sense.
    /// </summary>
    private void HandleReadyState()
    {
        if (isSensesActive && Input.GetKeyDown(activateKey))
        {
            ActivateSense();
            currentState = SenseState.Active;
            effectEndTime = Time.time + EffectDuration;

            if (cooldownImage != null)
            {
                cooldownImage.color = activeColor;
            }
        }
    }

    /// <summary>
    /// Handles the "Active" state, updating the UI as the effect duration counts down.
    /// </summary>
    private void HandleActiveState()
    {
        if (Time.time < effectEndTime)
        {
            if (cooldownImage != null)
            {
                float remainingTime = effectEndTime - Time.time;
                cooldownImage.fillAmount = remainingTime / EffectDuration;
            }
        }
        else
        {
            currentState = SenseState.Cooldown;
            cooldownEndTime = Time.time + cooldownDuration;

            if (cooldownImage != null)
            {
                cooldownImage.fillAmount = 0;
                cooldownImage.color = originalColor; // Revert color
            }
        }
    }

    /// <summary>
    /// Handles the "Cooldown" state, updating the UI as the cooldown timer progresses.
    /// </summary>
    private void HandleCooldownState()
    {
        if (Time.time < cooldownEndTime)
        {
            if (cooldownImage != null)
            {
                float cooldownStartTime = cooldownEndTime - cooldownDuration;
                float elapsedTime = Time.time - cooldownStartTime;
                cooldownImage.fillAmount = elapsedTime / cooldownDuration;
            }
        }
        else
        {
            currentState = SenseState.Ready;
            if (cooldownImage != null)
            {
                cooldownImage.fillAmount = 1;
            }
        }
    }

    /// <summary>
    /// Activates the specific sense ability. To be implemented by subclasses.
    /// </summary>
    protected abstract void ActivateSense();

    private void HandleGameStateChanged(GameState newState)
    {
        isSensesActive = newState == GameState.Gameplay;
    }

    private void OnDisable()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }
    }
}
