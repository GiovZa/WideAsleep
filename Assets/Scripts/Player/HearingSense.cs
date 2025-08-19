using System.Collections.Generic;
using UnityEngine;

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

    private List<SoundEmitter> soundEmitters = new List<SoundEmitter>();
    private List<SwitchLayer> switchableObjects = new List<SwitchLayer>();
    
    // Lists to track objects affected by the current pulse
    private List<SoundEmitter> activeEmitters = new List<SoundEmitter>();
    private List<SwitchLayer> activeSwitchableObjects = new List<SwitchLayer>();

    protected override float EffectDuration => revealTime;

    protected override void Start()
    {
        base.Start();
        CacheSoundEmitters();
        CacheSwitchableObjects();

        // Initially hide emitters by default
        SetEmittersVisible(false);
    }

    void CacheSoundEmitters()
    {
        soundEmitters.Clear();
        soundEmitters.AddRange(FindObjectsOfType<SoundEmitter>());
    }

    void CacheSwitchableObjects()
    {
        switchableObjects.Clear();
        switchableObjects.AddRange(FindObjectsOfType<SwitchLayer>());
    }

    protected override void ActivateSense()
    {
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

        // Activate effects on sound emitters within range
        foreach (var emitter in soundEmitters)
        {
            float distance = Vector3.Distance(playerPosition, emitter.transform.position);
            if (distance <= maxDistance)
            {
                float adjustedVolume = Mathf.Clamp01(1 - (distance / maxDistance)) * normalVolume;
                emitter.PlaySound(adjustedVolume);
                emitter.SetVisible(true);
                emitter.ToggleHighlight(true);
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

        // Deactivate effects on all emitters that were activated by this pulse
        foreach (var emitter in activeEmitters)
        {
            if (emitter != null)
            {
                emitter.StopSound();
                emitter.SetVisible(false);
                emitter.ToggleHighlight(false);
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
    public void RegisterSoundEmitter(SoundEmitter emitter)
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
