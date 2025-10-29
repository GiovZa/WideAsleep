using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;
using Beautify.Universal;

/// <summary>
/// Manages post-processing and other visual effects.
/// </summary>
public class EffectsManager : MonoBehaviour
{
    public static EffectsManager Instance { get; private set; }

    private Volume postProcessVolume;
    private Coroutine blurCoroutine;
    private Coroutine vignetteCoroutine;
    private Coroutine postBrightnessCoroutine;
    private Coroutine saturationAdjustmentCoroutine;
    private Coroutine filmGrainCoroutine;
    private ColorAdjustments colorAdjustments;

    [Header("Effect Settings")]
    // [Tooltip("The maximum *additional* intensity of the chromatic aberration effect for the stamina feedback.")]
    // [Range(0, 1)]
    // public float maxStaminaChromaticAberration = 0.75f;

    [Tooltip("How quickly the stamina effect fades in and out.")]
    public float staminaEffectSmoothTime = 0.5f;
    private Color baseTintColor;
    private Color targetTintColor;
    private float staminaEffectVelocity;
    [Tooltip("The maximum tint color shift (towards blue) for the stamina feedback.")]
    [Range(-1,1)]
    public float maxStaminaTintColorShift = 0.25f;
    [SerializeField] private float baseChromaticAberrationIntensity;
    [SerializeField] private float targetChromaticAberrationIntensity;
    [Tooltip("The maximum chromatic aberration intensity shift for the stamina feedback.")]
    [Range(-0.1f,0.1f)]
    public float maxChromaticAberrationShift = 0.04f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializeForNewScene();
    }

    public void InitializeForNewScene()
    {
        // Find the post-processing volume in the scene
        postProcessVolume = FindObjectOfType<Volume>();

        if (postProcessVolume != null)
        {
            // Try to get the screen effects from the volume's profile
            postProcessVolume.profile.TryGet(out colorAdjustments);
        }
        else
        {
            // Nullify references if no volume is found
            colorAdjustments = null;
        }

        if (colorAdjustments == null)
        {
            Debug.LogWarning("ColorAdjustments not found on a Post Process Volume in the scene. The black and white effect will not work.");
        }

        if (BeautifySettings.settings.tintColor != null)
        {
            baseTintColor = BeautifySettings.settings.tintColor.value;
            targetTintColor = baseTintColor;
        }
        else
        {
            Debug.LogWarning("TintColor not found on a Post Process Volume in the scene. The stamina tint color effect will not work.");
        }

        if (BeautifySettings.settings.chromaticAberrationIntensity != null)
        {
            baseChromaticAberrationIntensity = BeautifySettings.settings.chromaticAberrationIntensity.value;
            targetChromaticAberrationIntensity = baseChromaticAberrationIntensity;
        }
        else
        {
            Debug.LogWarning("ChromaticAberrationIntensity not found on a Post Process Volume in the scene. The stamina chromatic aberration effect will not work.");
        }
    }

    private void Update()
    {
        if (BeautifySettings.settings.tintColor != null)
        {
            float currentTintColorAlpha = BeautifySettings.settings.tintColor.value.a;
            float newTintColorAlpha = Mathf.SmoothDamp(currentTintColorAlpha, targetTintColor.a, ref staminaEffectVelocity, staminaEffectSmoothTime);
            BeautifySettings.settings.tintColor.Override(new Color(baseTintColor.r, baseTintColor.g, baseTintColor.b, newTintColorAlpha));
        }

        if (BeautifySettings.settings.chromaticAberrationIntensity != null)
        {
            BeautifySettings.settings.chromaticAberrationIntensity.Override(targetChromaticAberrationIntensity);
        }
    }

    public void UpdateStaminaEffect(float staminaPercentage)
    {
        if (baseTintColor != null)
        {
            float tintColorAlphaDelta = (1.0f - staminaPercentage) * maxStaminaTintColorShift;
            targetTintColor.a = baseTintColor.a + tintColorAlphaDelta;
        }

        if (BeautifySettings.settings.chromaticAberrationIntensity != null)
        {
            float chromaticAberrationIntensityDelta = (1.0f - staminaPercentage) * maxChromaticAberrationShift;
            targetChromaticAberrationIntensity = baseChromaticAberrationIntensity + chromaticAberrationIntensityDelta;
        }
    }

    #region Vignette Effect
    /// <summary>
    /// Triggers a vignette pulse effect.
    /// </summary>
    /// <param name="setOuterRing">The outer ring of the vignette (-2 to 1).</param>
    /// <param name="setInnerRing">The inner ring of the vignette (0 to 1).</param>
    /// <param name="fadeTime">Time in seconds for the vignette to fade in and out.</param>
    /// <param name="stayTime">Time in seconds for the vignette to stay at max intensity.</param>
    public void PulseVignette(float fadeTime, float stayTime, float setOuterRing, float setInnerRing)
    {
        if (BeautifySettings.settings.vignettingOuterRing != null && BeautifySettings.settings.vignettingInnerRing != null && vignetteCoroutine == null)
        {
            vignetteCoroutine = StartCoroutine(VignetteCoroutine(fadeTime, stayTime, setOuterRing, setInnerRing));
        }
    }

    private IEnumerator VignetteCoroutine(float fadeTime, float stayTime, float setOuterRing, float setInnerRing)
    {
        // Store original intensity
        float originalOuterRing = BeautifySettings.settings.vignettingOuterRing.value;
        float originalInnerRing = BeautifySettings.settings.vignettingInnerRing.value;

        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            float outerRing = Mathf.Lerp(originalOuterRing, setOuterRing, elapsedTime / fadeTime);
            float innerRing = Mathf.Lerp(originalInnerRing, setInnerRing, elapsedTime / fadeTime);
            BeautifySettings.settings.vignettingOuterRing.Override(outerRing);
            BeautifySettings.settings.vignettingInnerRing.Override(innerRing);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        BeautifySettings.settings.vignettingOuterRing.Override(setOuterRing);
        BeautifySettings.settings.vignettingInnerRing.Override(setInnerRing);

        // Stay at max intensity
        yield return new WaitForSeconds(stayTime);

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            float outerRing = Mathf.Lerp(setOuterRing, originalOuterRing, elapsedTime / fadeTime);
            float innerRing = Mathf.Lerp(setInnerRing, originalInnerRing, elapsedTime / fadeTime);
            BeautifySettings.settings.vignettingOuterRing.Override(outerRing);
            BeautifySettings.settings.vignettingInnerRing.Override(innerRing);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset intensity
        BeautifySettings.settings.vignettingOuterRing.Override(originalOuterRing);
        BeautifySettings.settings.vignettingInnerRing.Override(originalInnerRing);

        vignetteCoroutine = null;
    }
    #endregion

    #region Blink and Blur Effect
    public void TriggerBlur(float duration, float fadeTime, float targetBlurIntensity)
    {
        if (BeautifySettings.settings.blurIntensity != null && blurCoroutine == null)
        {
            BeautifySettings.Blink(0.3f);
            blurCoroutine = StartCoroutine(BlurCoroutine(duration, fadeTime, targetBlurIntensity));
        }
    }

    IEnumerator BlurCoroutine(float duration, float fadeTime, float targetBlurIntensity)
    {
        //Store original blur intensity
        float originalBlurIntensity = BeautifySettings.settings.blurIntensity.value;
        
        //Fade in, transition to target blur intensity
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            float blurIntensity = Mathf.Lerp(originalBlurIntensity, targetBlurIntensity, elapsedTime / fadeTime);
            BeautifySettings.settings.blurIntensity.Override(blurIntensity);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        BeautifySettings.settings.blurIntensity.Override(targetBlurIntensity);
        
        //Hold at target blur intensity
        yield return new WaitForSeconds(duration);
        
        //Fade out, transition to original blur intensity
        elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            float blurIntensity = Mathf.Lerp(targetBlurIntensity, originalBlurIntensity, elapsedTime / fadeTime);
            BeautifySettings.settings.blurIntensity.Override(blurIntensity);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        BeautifySettings.settings.blurIntensity.Override(originalBlurIntensity);

        blurCoroutine = null;
    }
    #endregion

    #region Saturation Adjustment
    /// <summary>
    /// Triggers a saturation adjustment.
    /// </summary>
    /// <param name="duration">How long the effect should last in seconds.</param>
    /// <param name="fadeTime">How long the fade in and fade out should last in seconds.</param>
    /// <param name="changeSaturation">The saturation value to change to the original saturation.</param>
    public void TriggerSaturationAdjustment(float duration, float fadeTime, float changeSaturation)
    {
        if (BeautifySettings.settings.saturate != null && saturationAdjustmentCoroutine == null)
        {
            saturationAdjustmentCoroutine = StartCoroutine(SaturationAdjustmentCoroutine(duration, fadeTime, changeSaturation));
        }
    }

    private IEnumerator SaturationAdjustmentCoroutine(float duration, float fadeTime, float changeSaturation)
    {
        // Store original and target saturation values
        float originalSaturation = BeautifySettings.settings.saturate.value;
        float targetSaturation = originalSaturation + changeSaturation;

        // set fade in and out times
        float fadeInTime = fadeTime;
        float fadeOutTime = fadeTime;

        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < fadeInTime)
        {
            float saturation = Mathf.Lerp(originalSaturation, targetSaturation, elapsedTime / fadeInTime);
            BeautifySettings.settings.saturate.Override(saturation);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        BeautifySettings.settings.saturate.Override(targetSaturation);

        // Hold at saturation
        yield return new WaitForSeconds(duration);

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeOutTime)
        {
            float saturation = Mathf.Lerp(targetSaturation, originalSaturation, elapsedTime / fadeOutTime);
            BeautifySettings.settings.saturate.Override(saturation);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        BeautifySettings.settings.saturate.Override(originalSaturation);
    }
    #endregion

    #region Post Brightness Adjustment
    /// <summary>
    /// Triggers a post brightness adjustment.
    /// </summary>
    /// <param name="duration">How long the effect should last in seconds.</param>
    /// <param name="fadeTime">How long the fade in and fade out should last in seconds.</param>
    /// <param name="changePostBrightness">The post brightness value to change to the original brightness.</param>
    public void TriggerPostBrightnessAdjustment(float duration, float fadeTime, float changePostBrightness)
    {
        if (BeautifySettings.settings.brightness != null && postBrightnessCoroutine == null)
        {
            postBrightnessCoroutine = StartCoroutine(PostBrightnessAdjustmentCoroutine(duration, fadeTime, changePostBrightness));
        }
    }

    private IEnumerator PostBrightnessAdjustmentCoroutine(float duration, float fadeTime, float changePostBrightness)
    {
        // Store original and target brightness values
        float originalBrightness = BeautifySettings.settings.brightness.value;
        float targetBrightness = originalBrightness + changePostBrightness;
        
        // set fade in and out times
        float fadeInTime = fadeTime;
        float fadeOutTime = fadeTime;

        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < fadeInTime)
        {
            float brightness = Mathf.Lerp(originalBrightness, targetBrightness, elapsedTime / fadeInTime);
            BeautifySettings.settings.brightness.Override(brightness);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        BeautifySettings.settings.brightness.Override(targetBrightness);

        // Hold at brightness
        yield return new WaitForSeconds(duration);

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeOutTime)
        {
            float brightness = Mathf.Lerp(targetBrightness, originalBrightness, elapsedTime / fadeOutTime);
            BeautifySettings.settings.brightness.Override(brightness);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        BeautifySettings.settings.brightness.Override(originalBrightness);

        postBrightnessCoroutine = null;
    }
    #endregion

    #region Film Grain Adjustment
    /// <summary>
    /// Toggles the film grain effect.
    /// </summary>
    /// <param name="duration">How long the effect should last in seconds.</param>
    /// <param name="fadeTime">This is temporarily used to extend the effect duration.</param>
    /// <param name="enableFilmGrain">Whether to enable or disable the film grain effect.</param>
    public void ToggleFilmGrain(float duration, float fadeTime, bool enableFilmGrain)
    {
        if (BeautifySettings.settings.filmGrainEnabled != null && filmGrainCoroutine == null)
        {
            filmGrainCoroutine = StartCoroutine(FilmGrainCoroutine(duration, fadeTime, enableFilmGrain));
        }
    }

    private IEnumerator FilmGrainCoroutine(float duration, float fadeTime, bool enableFilmGrain)
    {
        // Store original film grain enabled value
        bool originalFilmGrainEnabled = BeautifySettings.settings.filmGrainEnabled.value;
        
        if (originalFilmGrainEnabled == enableFilmGrain)
        {
            yield break;
        }

        BeautifySettings.settings.filmGrainEnabled.Override(enableFilmGrain);
        yield return new WaitForSeconds(duration + fadeTime);
        BeautifySettings.settings.filmGrainEnabled.Override(!enableFilmGrain);
        
        filmGrainCoroutine = null;
    }
    #endregion

    #region Stun Effect, TODO: Refactor into BeautifySettings
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