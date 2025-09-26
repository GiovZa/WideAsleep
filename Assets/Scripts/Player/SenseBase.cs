using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// Base class for player senses like vision and hearing.
/// Handles common functionality such as activation input and cooldowns.
/// </summary>
public abstract class SenseBase : MonoBehaviour
{
    // Defines the possible states of the sense ability.
    protected enum SenseState { Ready, Active, Cooldown }
    protected SenseState currentState = SenseState.Ready;

    [Header("Cooldown Settings")]
    [Tooltip("How long to wait before this sense can be used again.")]
    [SerializeField] private float cooldownDuration = 5f;
    
    public float FillAmount { get; private set; }
    public bool IsActive { get; private set; }

    protected EffectsManager effectsManager;
    private float effectEndTime;
    private float cooldownEndTime;
    private bool isSensesActive = true;

    protected CustomInput m_Input;

    /// <summary>
    /// The duration of the sense's effect. To be implemented by subclasses.
    /// </summary>
    protected abstract float EffectDuration { get; }

    /// <summary>
    /// The input action to activate the sense. To be implemented by subclasses.
    /// </summary>
    protected abstract InputAction ActivationAction { get; }

    private void Awake()
    {
        m_Input = new CustomInput();
    }
    
    protected virtual void OnEnable()
    {
        m_Input.Player.Enable();
        
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameStateChanged += HandleGameStateChanged;
            // Initialize based on the current state in case this script starts after the initial state is set
            HandleGameStateChanged(GameStateManager.Instance.CurrentState);
        }
    }

    protected virtual void Start()
    {
        effectsManager = EffectsManager.Instance;
        if (effectsManager == null)
        {
            Debug.LogError("EffectsManager instance not found. Make sure an EffectsManager is active in the scene.");
        }

        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameStateManager instance not found. Senses will not respond to game state changes.");
        }

        FillAmount = 1f;
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
        if (isSensesActive && ActivationAction.triggered)
        {
            ActivateSense();
            currentState = SenseState.Active;
            effectEndTime = Time.time + EffectDuration;
            IsActive = true;
        }
    }

    /// <summary>
    /// Handles the "Active" state, updating the UI as the effect duration counts down.
    /// </summary>
    private void HandleActiveState()
    {
        if (Time.time < effectEndTime)
        {
            float remainingTime = effectEndTime - Time.time;
            FillAmount = remainingTime / EffectDuration;
        }
        else
        {
            currentState = SenseState.Cooldown;
            cooldownEndTime = Time.time + cooldownDuration;
            IsActive = false;
            FillAmount = 0f;
        }
    }

    /// <summary>
    /// Handles the "Cooldown" state, updating the UI as the cooldown timer progresses.
    /// </summary>
    private void HandleCooldownState()
    {
        if (Time.time < cooldownEndTime)
        {
            float cooldownStartTime = cooldownEndTime - cooldownDuration;
            float elapsedTime = Time.time - cooldownStartTime;
            FillAmount = elapsedTime / cooldownDuration;
        }
        else
        {
            currentState = SenseState.Ready;
            FillAmount = 1f;
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

    protected virtual void OnDisable()
    {
        m_Input.Player.Disable();

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }
    }
}
