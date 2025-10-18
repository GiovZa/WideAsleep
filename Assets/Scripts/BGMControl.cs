using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMControl : MonoBehaviour
{
    private AudioSource bgmPlayer;
    [SerializeField] private float originalVolume = 0.5f;
    private Coroutine activeFade; // To keep track of the currently running fade coroutine

    void Start()
    {
        bgmPlayer = GetComponent<AudioSource>();
        bgmPlayer.volume = originalVolume;
    }

    void OnEnable()
    {
        HearingSense.OnHearingSenseActivated += HandleBGMVolumeChange;
        HearingSense.OnHearingSenseDeactivated += HandleBGMVolumeBack;
        Piano.OnPianoActivated += HandleBGMVolumeChange;
        Piano.OnPianoDeactivated += HandleBGMVolumeBack;
    }

    void OnDisable()
    {
        HearingSense.OnHearingSenseActivated -= HandleBGMVolumeChange;
        HearingSense.OnHearingSenseDeactivated -= HandleBGMVolumeBack;
        Piano.OnPianoActivated -= HandleBGMVolumeChange;
        Piano.OnPianoDeactivated -= HandleBGMVolumeBack;
    }

    void HandleBGMVolumeChange(float targetVolumePercent, float fadeDuration)
    {
        if (activeFade != null)
        {
            StopCoroutine(activeFade);
        }
        float targetVolume = originalVolume * targetVolumePercent;
        activeFade = StartCoroutine(FadeVolume(targetVolume, fadeDuration));
    }

    void HandleBGMVolumeBack(float fadeDuration)
    {
        if (activeFade != null)
        {
            StopCoroutine(activeFade);
        }
        activeFade = StartCoroutine(FadeVolume(originalVolume, fadeDuration));
    }

    private IEnumerator FadeVolume(float targetVolume, float duration)
    {
        float startVolume = bgmPlayer.volume;
        float time = 0;

        while (time < duration)
        {
            bgmPlayer.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        bgmPlayer.volume = targetVolume; // Ensure the volume is set to the exact target value at the end
    }
}
