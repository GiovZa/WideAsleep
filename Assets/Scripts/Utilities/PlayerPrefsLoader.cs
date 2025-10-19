using UnityEngine;

public class PlayerPrefsLoader : MonoBehaviour
{
    void Start()
    {
        LoadAndApplySettings();
    }

    private void LoadAndApplySettings()
    {
        // --- Audio Settings ---
        // We call the SoundMixerManager directly to apply the settings.
        if (SoundMixerManager.Instance != null)
        {
            float masterVolume = PlayerPrefs.GetFloat("masterVolume", 1f);
            SoundMixerManager.Instance.SetMasterVolume(masterVolume);

            float musicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
            SoundMixerManager.Instance.SetMusicVolume(musicVolume);

            float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 1f);
            SoundMixerManager.Instance.SetSFXVolume(sfxVolume);
        }
        else
        {
            Debug.LogError("SoundMixerManager not found");
        }

        // --- Controls Settings ---
        // We invoke the event. The PlayerCharacterController will hear this and update itself.
        float sensitivity = PlayerPrefs.GetFloat("mouseSensitivity", 4.0f);
        OptionsMenuController.TriggerSensitivityChanged(sensitivity);

        // --- Graphics Settings ---
        int qualityLevel = PlayerPrefs.GetInt("masterQuality", QualitySettings.GetQualityLevel());
        QualitySettings.SetQualityLevel(qualityLevel);

        bool isFullscreen = PlayerPrefs.GetInt("masterFullscreen", Screen.fullScreen ? 1 : 0) == 1;
        Screen.fullScreen = isFullscreen;
        
        Debug.Log("PlayerPrefsLoader: All settings loaded and applied.");
        
        // This loader has done its job, so we can disable it.
        gameObject.SetActive(false);
    }
}
