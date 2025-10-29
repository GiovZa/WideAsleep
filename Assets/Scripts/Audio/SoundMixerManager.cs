using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    public static SoundMixerManager Instance;
    [SerializeField] private AudioMixer audioMixer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(level) * 20f);
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("Music", Mathf.Log10(level) * 20f);
    }

    public void SetSFXVolume(float level)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(level) * 20f);
    }

    public float GetMixerParameterValue(string parameterName)
    {
        if (audioMixer.GetFloat(parameterName, out float value))
        {
            return value;
        }
        else
        {
            Debug.LogError($"Mixer parameter {parameterName} not found");
            return 0f;
        }
    }
    
    /// <summary>
    /// Starts a coroutine to fade a mixer parameter from its current value to a target value over a given duration.
    /// </summary>
    /// <param name="parameterName">The name of the mixer parameter to fade.</param>
    /// <param name="targetValue">The target value to fade to.</param>
    /// <param name="duration">The duration of the fade in seconds.</param>
    public void FadeMixerParameter(string parameterName, float targetValue, float duration)
    {
        StartCoroutine(FadeParameterRoutine(parameterName, targetValue, duration));
    }

    /// <summary>
    /// Fades a mixer parameter from its current value to a target value over a given duration.
    /// </summary>
    /// <param name="parameterName">The name of the mixer parameter to fade.</param>
    /// <param name="targetValue">The target value to fade to.</param>
    /// <param name="duration">The duration of the fade in seconds.</param>
    /// <returns>An enumerator that can be used to wait for the fade to complete.</returns>
    private IEnumerator FadeParameterRoutine(string parameterName, float targetValue, float duration)
    {
        if (duration <= 0)
        {
            audioMixer.SetFloat(parameterName, targetValue);
            yield break;
        }

        audioMixer.GetFloat(parameterName, out float startValue);
        float time = 0;

        while (time < duration)
        {
            float newValue = Mathf.Lerp(startValue, targetValue, time / duration);
            audioMixer.SetFloat(parameterName, newValue);
            time += Time.deltaTime;
            yield return null;
        }

        audioMixer.SetFloat(parameterName, targetValue);
    }
}
