using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class HearingSense : MonoBehaviour
{
    [Header("Input Settings")]
    public KeyCode toggleKey = KeyCode.Q;

    [Header("Hearing Settings")]
    public string soundEmitterTag = "SonarTargets"; // Tag to look for
    public float mutedVolume = 0.1f;
    public float normalVolume = 1.0f;
    public AudioClip pulseSoundClip;

    private List<GameObject> soundEmitters = new List<GameObject>();
    private List<AudioSource> emitterAudioSources = new List<AudioSource>();

    // Reference to the Universal Renderer Data asset
    public UniversalRendererData universalRendererData;
    private RenderObjects xRayRenderFeature;

    void Start()
    {
        CacheSoundEmitters();
        CacheRenderObjectsFeatures();

        // Initially disable XRay feature
        SetRenderObjectsActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            PlayPulseForOneSecond();
        }
    }

    void CacheSoundEmitters()
    {
        // Get the layer index of the "SonarTargets" layer
        int sonarTargetsLayer = LayerMask.NameToLayer(soundEmitterTag);

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            // Check if the object's layer matches the "SonarTargets" layer
            if (obj.layer == sonarTargetsLayer)
            {
                soundEmitters.Add(obj);

                // Create or get an AudioSource at the emitter location
                AudioSource audioSource = obj.GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = obj.AddComponent<AudioSource>(); // Add an AudioSource if none exists
                }

                emitterAudioSources.Add(audioSource);
            }
        }
    }

    void CacheRenderObjectsFeatures()
    {
        // Access UniversalRendererData and find the XRay feature
        if (universalRendererData != null)
        {
            foreach (var feature in universalRendererData.rendererFeatures)
            {
                if (feature is RenderObjects renderObjectsFeature)
                {
                    if (renderObjectsFeature.name == "Xray")
                    {
                        xRayRenderFeature = renderObjectsFeature;
                        Debug.Log("XRay RenderFeature found.");
                    }
                }
            }

            if (xRayRenderFeature == null)
            {
                Debug.LogWarning("XRay RenderFeature not found.");
            }
        }
        else
        {
            Debug.LogError("UniversalRendererData is not assigned!");
        }
    }

    void PlayPulseForOneSecond()
    {
        AudioListener.volume = mutedVolume; // Muffle the rest of the world

        // Enable XRay render object for 1 second
        SetRenderObjectsActive(true);

        // Play pulse sound at each emitter for 1 second
        foreach (var audioSource in emitterAudioSources)
        {
            // Play pulse sound for 1 second at full volume
            audioSource.clip = pulseSoundClip; // Use your pulse sound or another clip here
            audioSource.volume = 1.0f; // Full volume
            audioSource.loop = false; // Ensure it doesn't loop
            audioSource.Play(); // Start playing the sound

            // Stop the sound after 1 second
            StartCoroutine(StopSoundAfterDelay(audioSource, 1f));
        }

        Debug.Log("Played pulse sound at emitters");
    }

    System.Collections.IEnumerator StopSoundAfterDelay(AudioSource audioSource, float delay)
    {
        Debug.Log("Waiting " + delay + "seconds");
        yield return new WaitForSeconds(delay); // Wait for the specified duration
        audioSource.Stop(); // Stop the sound after 1 second
        AudioListener.volume = normalVolume; // Return volume to normal

        Debug.Log("Disabling XRay render");
        // Re-disable XRay render object after 1 second
        SetRenderObjectsActive(false);
    }

    void SetRenderObjectsActive(bool isActive)
    {
        // Ensure that only the XRay RenderObjects feature is enabled/disabled
        if (xRayRenderFeature != null)
        {
            Debug.Log($"Setting XRay RenderObjects to {isActive}");
            xRayRenderFeature.SetActive(isActive); // Enable/Disable the XRay RenderFeature

            // Double-check that the feature is actually toggling
            Debug.Log(isActive ? "Enabled XRay RenderObjects." : "Disabled XRay RenderObjects.");
        }
        else
        {
            Debug.LogWarning("XRay RenderFeature not found, cannot toggle.");
        }
    }

    // Call this if you spawn new objects dynamically during gameplay
    public void RegisterSoundEmitter(GameObject obj)
    {
        if (obj.CompareTag(soundEmitterTag))
        {
            soundEmitters.Add(obj);

            // Create or get an AudioSource for the new emitter
            AudioSource audioSource = obj.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = obj.AddComponent<AudioSource>(); // Add an AudioSource if none exists
            }

            emitterAudioSources.Add(audioSource);
        }
    }
}
