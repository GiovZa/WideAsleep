using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    private void OnEnable()
    {
        OptionsMenuController.OnAudioMasterVolumeChanged += SetMasterVolume;
        OptionsMenuController.OnAudioMusicVolumeChanged += SetMusicVolume;
        OptionsMenuController.OnAudioSFXVolumeChanged += SetSFXVolume;
    }

    private void OnDisable()
    {
        OptionsMenuController.OnAudioMasterVolumeChanged -= SetMasterVolume;
        OptionsMenuController.OnAudioMusicVolumeChanged -= SetMusicVolume;
        OptionsMenuController.OnAudioSFXVolumeChanged -= SetSFXVolume;
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
}
