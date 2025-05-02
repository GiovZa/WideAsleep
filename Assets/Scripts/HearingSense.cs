using System.Collections.Generic;
using UnityEngine;

public class HearingSense : MonoBehaviour
{
    [Header("Input Settings")]
    public KeyCode toggleKey = KeyCode.Q;

    [Header("Hearing Settings")]
    public string soundEmitterTag = "Interactable"; // Tag to look for
    public float mutedVolume = 0.1f;
    public float normalVolume = 1.0f;
    public float maxDistance = 10f; // Max distance for visibility and volume adjustment

    private List<GameObject> soundEmitters = new List<GameObject>();
    private List<AudioSource> emitterAudioSources = new List<AudioSource>();

    void Start()
    {
        CacheSoundEmitters();

        // Initially hide emitters by default
        SetEmittersVisible(false);
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
        // Get the layer index of the "Interactable" layer
        int interactableLayer = LayerMask.NameToLayer(soundEmitterTag);

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            // Check if the object's layer matches the "Interactable" layer
            if (obj.layer == interactableLayer)
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

    void PlayPulseForOneSecond()
    {
        // Get the position of the player or the object triggering the pulse
        Vector3 playerPosition = transform.position; // Assuming this script is on the player object

        // Enable visibility of emitters for 1 second based on distance
        SetEmittersVisible(true, playerPosition);

        // Play pulse sound for the location (main player or the source triggering this)
        AudioSource pulseSource = GetComponent<AudioSource>();
        if (pulseSource != null)
        {
            pulseSource.Play(); // Play pulse sound from the script itself (optional)
        }

        // Play the sound from each emitter's AudioSource for 1 second, adjusting volume by distance
        foreach (var audioSource in emitterAudioSources)
        {
            if (audioSource != null)
            {
                // Adjust volume based on distance from player
                float distance = Vector3.Distance(playerPosition, audioSource.transform.position);
                float adjustedVolume = Mathf.Clamp01(1 - (distance / maxDistance)); // Volume decreases with distance

                audioSource.volume = adjustedVolume * normalVolume; // Set the volume based on distance
                audioSource.Play(); // Play the sound from the emitter's AudioSource

                // Stop the sound after 1 second
                StartCoroutine(StopSoundAfterDelay(audioSource, 1f));
            }
        }

        Debug.Log("Played pulse sound at emitters");
    }

    System.Collections.IEnumerator StopSoundAfterDelay(AudioSource audioSource, float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified duration
        audioSource.Stop(); // Stop the sound after 1 second

        // Re-disable visibility of emitters after 1 second
        SetEmittersVisible(false, transform.position);
    }

    void SetEmittersVisible(bool isVisible, Vector3 playerPosition)
    {
        foreach (var emitter in soundEmitters)
        {
            Renderer renderer = emitter.GetComponent<Renderer>();
            if (renderer != null)
            {
                float distance = Vector3.Distance(playerPosition, emitter.transform.position);
                if (distance <= maxDistance)
                {
                    renderer.enabled = isVisible; // Set visibility based on distance
                }
                else
                {
                    renderer.enabled = false; // Hide if beyond max distance
                }
            }
            else
            {
                Debug.LogWarning("No Renderer found on " + emitter.name);
            }
        }

        Debug.Log(isVisible ? "Enabled emitters visibility." : "Disabled emitters visibility.");
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

    void SetEmittersVisible(bool isVisible)
    {
        foreach (var emitter in soundEmitters)
        {
            Renderer renderer = emitter.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = isVisible; // Set visibility of the object
            }
            else
            {
                Debug.LogWarning("No Renderer found on " + emitter.name);
            }
        }

        Debug.Log(isVisible ? "Enabled emitters visibility." : "Disabled emitters visibility.");
    }
}
