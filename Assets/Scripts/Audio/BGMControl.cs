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
        EventCenter.AddListener<float, float>(EventDefine.HEARING_SENSE_ACTIVATED, HandleBGMVolumeChange);
        EventCenter.AddListener<float>(EventDefine.HEARING_SENSE_DEACTIVATED, HandleBGMVolumeBack);
        EventCenter.AddListener<float, float>(EventDefine.PIANO_ACTIVATED, HandleBGMVolumeChange);
        EventCenter.AddListener<float>(EventDefine.PIANO_DEACTIVATED, HandleBGMVolumeBack);
    }

    void OnDisable()
    {
        EventCenter.RemoveListener<float, float>(EventDefine.HEARING_SENSE_ACTIVATED, HandleBGMVolumeChange);
        EventCenter.RemoveListener<float>(EventDefine.HEARING_SENSE_DEACTIVATED, HandleBGMVolumeBack);
        EventCenter.RemoveListener<float, float>(EventDefine.PIANO_ACTIVATED, HandleBGMVolumeChange);
        EventCenter.RemoveListener<float>(EventDefine.PIANO_DEACTIVATED, HandleBGMVolumeBack);
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
