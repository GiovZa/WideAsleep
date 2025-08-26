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
    private DepthOfField depthOfField;
    private Coroutine visionSenseCoroutine;
    private ColorAdjustments colorAdjustments;
    private ChromaticAberration chromaticAberration;
    private WhiteBalance whiteBalance;

    [Header("Effect Settings")]
    // [Tooltip("The maximum *additional* intensity of the chromatic aberration effect for the stamina feedback.")]
    // [Range(0, 1)]
    // public float maxStaminaChromaticAberration = 0.75f;

    [Tooltip("The maximum temperature shift (towards blue) for the stamina feedback.")]
    [Range(-100, 0)]
    public float maxStaminaTemperatureShift = -40f;

    [Tooltip("How quickly the stamina effect fades in and out.")]
    public float staminaEffectSmoothTime = 0.5f;
    
    // private float baseChromaticAberrationIntensity;
    // private float targetChromaticAberrationIntensity;
    // private float chromaticAberrationVelocity;

    private float baseTemperature;
    private float targetTemperature;
    private float temperatureVelocity;

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
            // Try to get the screen effects from the volume's profile
            postProcessVolume.profile.TryGet(out vignette);
            postProcessVolume.profile.TryGet(out depthOfField);
            postProcessVolume.profile.TryGet(out colorAdjustments);
            // postProcessVolume.profile.TryGet(out chromaticAberration);
            postProcessVolume.profile.TryGet(out whiteBalance);
        }

        if (vignette == null)
        {
            Debug.LogWarning("Vignette not found on a Post Process Volume in the scene. The vignette effect will not work.");
        }

        if (depthOfField == null)
        {
            Debug.LogWarning("DepthOfField not found on a Post Process Volume in the scene. The vision sense effect will not work.");
        }

        if (colorAdjustments == null)
        {
            Debug.LogWarning("ColorAdjustments not found on a Post Process Volume in the scene. The black and white effect will not work.");
        }

        // if (chromaticAberration != null)
        // {
        //     baseChromaticAberrationIntensity = chromaticAberration.intensity.value;
        //     targetChromaticAberrationIntensity = baseChromaticAberrationIntensity;
        // }
        // else
        // {
        //     Debug.LogWarning("ChromaticAberration not found on a Post Process Volume in the scene. The stamina effect will not work.");
        // }

        if (whiteBalance != null)
        {
            baseTemperature = whiteBalance.temperature.value;
            targetTemperature = baseTemperature;
        }
        else
        {
            Debug.LogWarning("WhiteBalance not found on a Post Process Volume in the scene. The stamina temperature effect will not work.");
        }
    }

    private void Update()
    {
        // if (chromaticAberration != null)
        // {
        //     float currentIntensity = chromaticAberration.intensity.value;
        //     float newIntensity = Mathf.SmoothDamp(currentIntensity, targetChromaticAberrationIntensity, ref chromaticAberrationVelocity, staminaEffectSmoothTime);
        //     chromaticAberration.intensity.Override(newIntensity);
        // }

        if (whiteBalance != null)
        {
            float currentTemperature = whiteBalance.temperature.value;
            float newTemperature = Mathf.SmoothDamp(currentTemperature, targetTemperature, ref temperatureVelocity, staminaEffectSmoothTime);
            whiteBalance.temperature.Override(newTemperature);
        }
    }

    public void UpdateStaminaEffect(float staminaPercentage)
    {
        // if (chromaticAberration != null)
        // {
        //     // Invert the percentage: low stamina = high effect intensity
        //     // Calculate the *additional* intensity based on stamina drain
        //     float intensityDelta = (1.0f - staminaPercentage) * maxStaminaChromaticAberration;
        //     targetChromaticAberrationIntensity = baseChromaticAberrationIntensity + intensityDelta;
        // }
        
        if (whiteBalance != null)
        {
            // Invert the percentage: low stamina = colder temperature
            float temperatureDelta = (1.0f - staminaPercentage) * maxStaminaTemperatureShift;
            targetTemperature = baseTemperature + temperatureDelta;
        }
    }

    #region Vignette Effect
    /// <summary>
    /// Triggers a vignette pulse effect.
    /// </summary>
    /// <param name="duration">Time in seconds for the vignette to pulse.</param>
    /// <param name="setIntensity">The intensity of the vignette (0 to 1).</param>
    /// <param name="fadeInTime">Time in seconds for the vignette to fade in.</param>
    /// <param name="stayTime">Time in seconds for the vignette to stay at max intensity.</param>
    /// <param name="fadeOutTime">Time in seconds for the vignette to fade out.</param>
    public void PulseVignette(float fadeInTime, float stayTime, float fadeOutTime, float setIntensity)
    {
        if (vignette != null)
        {
            StartCoroutine(VignetteCoroutine(fadeInTime, stayTime, fadeOutTime, setIntensity));
        }
    }

    private IEnumerator VignetteCoroutine(float fadeInTime, float stayTime, float fadeOutTime, float setIntensity)
    {
        // Store original intensity
        float originalIntensity = vignette.intensity.value;

        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < fadeInTime)
        {
            float intensity = Mathf.Lerp(originalIntensity, setIntensity, elapsedTime / fadeInTime);
            vignette.intensity.Override(intensity);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        vignette.intensity.Override(setIntensity);

        // Stay at max intensity
        yield return new WaitForSeconds(stayTime);

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeOutTime)
        {
            float intensity = Mathf.Lerp(setIntensity, originalIntensity, elapsedTime / fadeOutTime);
            vignette.intensity.Override(intensity);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset intensity
        vignette.intensity.Override(originalIntensity);
    }
    #endregion
    
    #region Blurry Vision Effect
    /// <summary>
    /// Temporarily adjusts Depth of Field to make the scene clear.
    /// </summary>
    /// <param name="duration">How long the effect should last in seconds.</param>
    /// <param name="fadeTime">How long the fade in and fade out should last in seconds.</param>
    /// <param name="targetFocalLength">The focal length to use for clear vision (lower is clearer).</param>
    /// <param name="targetAperture">The aperture to use for clear vision (higher is clearer).</param>
    /// <param name="targetFocusDistance">The focus distance to use for clear vision (lower is closer).</param>
    public void TriggerVisionSense(float duration, float fadeTime, float targetStart, float targetEnd, float targetMaxRadius)
    {
        if (depthOfField != null && visionSenseCoroutine == null)
        {
            visionSenseCoroutine = StartCoroutine(VisionSenseCoroutine(duration, fadeTime, targetStart, targetEnd, targetMaxRadius));
        }
    }

    private IEnumerator VisionSenseCoroutine(float duration, float fadeTime, float targetStart, float targetEnd, float targetMaxRadius)
    {
        // Store original values
        bool wasActive = depthOfField.active;
        float originalStart = depthOfField.gaussianStart.value;
        float originalEnd = depthOfField.gaussianEnd.value;
        float originalMaxRadius = depthOfField.gaussianMaxRadius.value;
        
        // set fade in and out times
        float fadeInTime = fadeTime;
        float fadeOutTime = fadeTime;
        
        // Fade in - transition to clear vision
        float elapsedTime = 0f;
        while (elapsedTime < fadeInTime)
        {
            float t = elapsedTime / fadeInTime;
            float currentStart = Mathf.Lerp(originalStart, targetStart, t);
            float currentEnd = Mathf.Lerp(originalEnd, targetEnd, t);
            float currentMaxRadius = Mathf.Lerp(originalMaxRadius, targetMaxRadius, t);
            
            depthOfField.gaussianStart.Override(currentStart);
            depthOfField.gaussianEnd.Override(currentEnd);
            depthOfField.gaussianMaxRadius.Override(currentMaxRadius);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Set final clear values
        depthOfField.gaussianStart.Override(targetStart);
        depthOfField.gaussianEnd.Override(targetEnd);
        depthOfField.gaussianMaxRadius.Override(targetMaxRadius);
        // Hold at clear vision
        yield return new WaitForSeconds(duration);
        
        // Fade out - transition back to original
        elapsedTime = 0f;
        while (elapsedTime < fadeOutTime)
        {
            float t = elapsedTime / fadeOutTime;
            float currentStart = Mathf.Lerp(targetStart, originalStart, t);
            float currentEnd = Mathf.Lerp(targetEnd, originalEnd, t);
            float currentMaxRadius = Mathf.Lerp(targetMaxRadius, originalMaxRadius, t);
            
            depthOfField.gaussianStart.Override(currentStart);
            depthOfField.gaussianEnd.Override(currentEnd);
            depthOfField.gaussianMaxRadius.Override(currentMaxRadius);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Restore the previous state
        depthOfField.gaussianStart.Override(originalStart);
        depthOfField.gaussianEnd.Override(originalEnd);
        depthOfField.gaussianMaxRadius.Override(originalMaxRadius);
        depthOfField.active = wasActive;

        visionSenseCoroutine = null;
    }
    #endregion

    #region Black and White Effect
    /// <summary>
    /// Triggers a black and white screen effect.
    /// </summary>
    /// <param name="duration">How long the effect should last in seconds.</param>
    /// <param name="fadeTime">How long the fade in and fade out should last in seconds.</param>
    /// <param name="setSaturation">The saturation to use for the black and white effect (0 is black and white, 1 is original).</param>
    public void TriggerBlackAndWhite(float duration, float fadeTime, float setSaturation)
    {
        if (colorAdjustments != null)
        {
            StartCoroutine(BlackAndWhiteCoroutine(duration, fadeTime, setSaturation));
        }
    }

    private IEnumerator BlackAndWhiteCoroutine(float duration, float fadeTime, float setSaturation)
    {
        // Store original saturation
        float originalSaturation = colorAdjustments.saturation.value;

        // set fade in and out times
        float fadeInTime = fadeTime;
        float fadeOutTime = fadeTime;

        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < fadeInTime)
        {
            float saturation = Mathf.Lerp(originalSaturation, setSaturation, elapsedTime / fadeInTime);
            colorAdjustments.saturation.Override(saturation);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        colorAdjustments.saturation.Override(setSaturation);

        // Hold at black and white
        yield return new WaitForSeconds(duration);

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeOutTime)
        {
            float saturation = Mathf.Lerp(setSaturation, originalSaturation, elapsedTime / fadeOutTime);
            colorAdjustments.saturation.Override(saturation);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        colorAdjustments.saturation.Override(originalSaturation);
    }
    #endregion

    #region Stun Effect
    /// <summary>
    /// The effect when player get stunned by SCREAM's scream.
    /// </summary>
    /// <param name="duration">How long the effect should last in seconds.</param>
    /// <param name="fadeTime">How long the fade in and fade out should last in seconds.</param>
    /// <param name="setColorFilter">The color filter to use for the stun effect.</param>
    public void TriggerStunEffect(float duration, float fadeTime, Color setColorFilter)
    {
        if (colorAdjustments != null)
        {
            StartCoroutine(StunEffectCoroutine(duration, fadeTime, setColorFilter));
        }
    }

    private IEnumerator StunEffectCoroutine(float duration, float fadeTime, Color setColorFilter)
    {
        // Store original Color Filter
        Color originalColorFilter = colorAdjustments.colorFilter.value;

        // set fade in and out times
        float fadeInTime = fadeTime;
        float fadeOutTime = fadeTime;

        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < fadeInTime)
        {
            float t = elapsedTime / fadeInTime;
            Color currentColorFilter = Color.Lerp(originalColorFilter, setColorFilter, t);
            colorAdjustments.colorFilter.Override(currentColorFilter);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        colorAdjustments.colorFilter.Override(setColorFilter);

        // Hold at black and white
        yield return new WaitForSeconds(duration);

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeOutTime)
        {
            float t = elapsedTime / fadeOutTime;
            Color currentColorFilter = Color.Lerp(setColorFilter, originalColorFilter, t);
            colorAdjustments.colorFilter.Override(currentColorFilter);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        colorAdjustments.colorFilter.Override(originalColorFilter);
    }
    #endregion
}