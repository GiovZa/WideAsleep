using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

/// <summary>
/// Manages post-processing and other visual effects.
/// </summary>
public class EffectsManager : MonoBehaviour
{
    public static EffectsManager Instance { get; private set; }

    private Volume postProcessVolume;
    private Vignette vignette;
    private float currentIntensity;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Find the post-processing volume in the scene
        postProcessVolume = FindObjectOfType<Volume>();

        if (postProcessVolume != null)
        {
            // Try to get the Vignette effect from the volume's profile
            postProcessVolume.profile.TryGet(out vignette);
        }

        if (vignette != null)
        {
            currentIntensity = vignette.intensity.value;
        }
        else
        {
            Debug.LogWarning("Vignette not found on a Post Process Volume in the scene. The vignette effect will not work.");
        }
    }

    /// <summary>
    /// Triggers a vignette pulse effect.
    /// </summary>
    /// <param name="fadeInTime">Time in seconds for the vignette to fade in.</param>
    /// <param name="stayTime">Time in seconds for the vignette to stay at max intensity.</param>
    /// <param name="fadeOutTime">Time in seconds for the vignette to fade out.</param>
    /// <param name="maxIntensity">The maximum intensity of the vignette (0 to 1).</param>
    public void PulseVignette(float fadeInTime, float stayTime, float fadeOutTime, float maxIntensity)
    {
        if (vignette != null)
        {
            StartCoroutine(VignetteCoroutine(fadeInTime, stayTime, fadeOutTime, maxIntensity));
        }
    }

    private IEnumerator VignetteCoroutine(float fadeInTime, float stayTime, float fadeOutTime, float maxIntensity)
    {
        // Ensure the vignette is active
        vignette.active = true;

        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < fadeInTime)
        {
            float intensity = Mathf.Lerp(0, maxIntensity, elapsedTime / fadeInTime);
            vignette.intensity.Override(intensity);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        vignette.intensity.Override(maxIntensity);

        // Stay at max intensity
        yield return new WaitForSeconds(stayTime);

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeOutTime)
        {
            float intensity = Mathf.Lerp(maxIntensity, 0, elapsedTime / fadeOutTime);
            vignette.intensity.Override(intensity);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset and deactivate
        vignette.intensity.Override(currentIntensity);
        vignette.active = false;
    }
}
