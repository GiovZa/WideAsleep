using System.Collections.Generic;
using UnityEngine;
using playerChar;
using UnityEngine.InputSystem;

public class HearingSense : SenseBase
{
    [Header("Hearing Settings")]
    public float mutedVolume = 0.1f;
    public float normalVolume = 1.0f;
    public float maxDistance = 10f; // Max distance for visibility and volume adjustment

    public float revealTime = 2f;

    [Header("Vignette Settings")]
    [SerializeField] private float vignetteFadeInTime = 0.5f;
    [SerializeField] private float vignetteStayTime = 2.0f;
    [SerializeField] private float vignetteFadeOutTime = 0.5f;
    [SerializeField] [Range(0, 1)] private float vignetteMaxIntensity = 0.6f;
    [SerializeField] [Range(-100,100)] private int setSaturation = -60;

    private List<SoundEmitterHint> soundEmitters = new List<SoundEmitterHint>();
    private List<SwitchLayer> switchableObjects = new List<SwitchLayer>();
    
    // Lists to track objects affected by the current pulse
    private List<SoundEmitterHint> activeEmitters = new List<SoundEmitterHint>();
    private List<SwitchLayer> activeSwitchableObjects = new List<SwitchLayer>();

    private bool isSenseActive = false;
    private PlayerCharacterController playerController;

    protected override float EffectDuration => revealTime;
    protected override InputAction ActivationAction => m_Input.Player.HearingSense;


    protected override void OnEnable()
    {
        base.OnEnable();
        BossScreamManager.OnScreamStarted += HandleScreamStarted;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        BossScreamManager.OnScreamStarted -= HandleScreamStarted;
    }

    private void HandleScreamStarted()
    {
        if (isSenseActive)
        {
            Debug.Log("Player stunned by scream while using hearing sense!");
            if (playerController != null)
            {
                playerController.Stun(3f);
                effectsManager.TriggerStunEffect(3f, 0.5f, new Color(1, 0.545f, 0.545f));
            }
        }
        else
        {
            Debug.Log("Player affected by scream while not using hearing sense!");
            if (playerController != null)
            {
                playerController.Stun(1f);
                effectsManager.TriggerStunEffect(1f, 0.2f, new Color(0.996f, 0.757f, 0.765f));
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        playerController = GetComponentInParent<PlayerCharacterController>();
        CacheSoundEmitters();
        CacheSwitchableObjects();

        // Initially hide emitters by default
        SetEmittersVisible(false);
    }

    void CacheSoundEmitters()
    {
        soundEmitters.Clear();
        soundEmitters.AddRange(FindObjectsOfType<SoundEmitterHint>());
    }

    void CacheSwitchableObjects()
    {
        switchableObjects.Clear();
        switchableObjects.AddRange(FindObjectsOfType<SwitchLayer>());
    }

    protected override void ActivateSense()
    {
        isSenseActive = true;

        // Trigger screen visual effects
        if (effectsManager != null)
        {
            effectsManager.PulseVignette(vignetteFadeInTime, vignetteStayTime, vignetteFadeOutTime, vignetteMaxIntensity);
            effectsManager.TriggerBlackAndWhite(revealTime, vignetteFadeInTime, setSaturation); 
        }
        
        // Get the position of the player at the moment of activation
        Vector3 playerPosition = transform.position;

        // Clear the lists of active objects for this new pulse
        activeEmitters.Clear();
        activeSwitchableObjects.Clear();

        // Remove any null or inactive objects from the lists before processing
        soundEmitters.RemoveAll(item => item == null || !item.gameObject.activeInHierarchy);
        switchableObjects.RemoveAll(item => item == null || !item.gameObject.activeInHierarchy);

        // Activate effects on sound emitters within range
        foreach (var emitter in soundEmitters)
        {
            float distance = Vector3.Distance(playerPosition, emitter.transform.position);
            if (distance <= maxDistance)
            {
                float adjustedVolume = Mathf.Clamp01(1 - (distance / maxDistance)) * normalVolume;
                emitter.PlaySound(adjustedVolume);
                emitter.SetVisible(true);
                activeEmitters.Add(emitter);
            }
        }

        // Activate X-ray effect on switchable objects within range
        foreach (var obj in switchableObjects)
        {
            float distance = Vector3.Distance(playerPosition, obj.transform.position);
            if (distance <= maxDistance)
            {
                obj.SetXRayActive(true);
                activeSwitchableObjects.Add(obj);
            }
        }

        // Stop sounds and effects after delay
        StartCoroutine(StopEffectsAfterDelay(revealTime));

        Debug.Log("Played pulse sound at emitters");
    }

    System.Collections.IEnumerator StopEffectsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        isSenseActive = false;

        // Deactivate effects on all emitters that were activated by this pulse
        foreach (var emitter in activeEmitters)
        {
            if (emitter != null)
            {
                emitter.SetVisible(false);
            }
        }

        // Revert layers of all objects that were switched by this pulse
        foreach (var obj in activeSwitchableObjects)
        {
            if (obj != null)
            {
                obj.SetXRayActive(false);
            }
        }
    }

    // Call this if you spawn new objects dynamically during gameplay
    public void RegisterSoundEmitter(SoundEmitterHint emitter)
    {
        if (!soundEmitters.Contains(emitter))
        {
            soundEmitters.Add(emitter);
        }
    }

    void SetEmittersVisible(bool isVisible)
    {
        foreach (var emitter in soundEmitters)
        {
            emitter.SetVisible(isVisible);
        }

        Debug.Log(isVisible ? "Enabled emitters visibility." : "Disabled emitters visibility.");
    }
}
