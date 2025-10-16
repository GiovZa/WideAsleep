using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OptionsMenuController : MonoBehaviour, IGenericUI
{
    // Event that other scripts can subscribe to
    public static event Action<float> OnSensitivityChanged;
    public static event Action<float> OnAudioMasterVolumeChanged;
    public static event Action<float> OnAudioMusicVolumeChanged;
    public static event Action<float> OnAudioSFXVolumeChanged;

    [Header("UI References")]
    [Header("Graphics")]
    public Button[] qualityButtons; // Assign in order: Low, Medium, High, Ultra
    public Button fullscreenOnButton;
    public Button fullscreenOffButton;
    public TMP_Dropdown resolutionDropdown;
    [Header("Controls")]
    public Slider sensitivitySlider;
    public TMP_Text sensitivityPercent;
    [Header("Audio")]
    public Slider audioMasterSlider;
    public Slider audioSFXSlider;
    public Slider audioMusicSlider;
    public TMP_Text audioMasterPercent;
    public TMP_Text audioSFXPercent;
    public TMP_Text audioMusicPercent;

    [Header("Highlight Colors")]
    public Color selectedColor = new Color(1f, 0.85f, 0.4f);
    public Color normalColor = Color.white;

    private Resolution[] _resolutions;

    // --- IGenericUI Implementation ---
    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }
    // --- End IGenericUI Implementation ---

    void Start()
    {
        // One-time setup for resolution options
        _resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
        System.Array.Reverse(_resolutions); // Reverse the array to show highest resolutions first
        resolutionDropdown.ClearOptions();
        
        List<string> options = new List<string>();
        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + " x " + _resolutions[i].height;
            options.Add(option);
        }
        resolutionDropdown.AddOptions(options);
    }

    /// <summary>
    /// UIManager helper methods
    /// </summary>
    public void OpenSubMenu(GameObject uiPanelObject)
    {
        UIManager.Instance.OpenUIPanel(uiPanelObject);
    }

    public void CloseSubMenu()
    {
        UIManager.Instance.CloseActiveUI();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("masterFullscreen", isFullscreen ? 1 : 0);
        HighlightFullscreenButton(isFullscreen);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("masterQuality", qualityIndex);
        HighlightQualityButton(qualityIndex);
    }

    public void SetSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("mouseSensitivity", sensitivity);
        sensitivityPercent.text = Math.Ceiling(sensitivity/ 4.0f * 100f).ToString() + "%";
        OnSensitivityChanged?.Invoke(sensitivity);
    }

    private void HighlightQualityButton(int qualityIndex)
    {
        for (int i = 0; i < qualityButtons.Length; i++)
        {
            if (qualityButtons[i] != null)
            {
                qualityButtons[i].GetComponent<Image>().color = (i == qualityIndex) ? selectedColor : normalColor;
            }
        }
    }

    private void HighlightFullscreenButton(bool isFullscreen)
    {
        if (fullscreenOnButton != null && fullscreenOffButton != null)
        {
            fullscreenOnButton.GetComponent<Image>().color = isFullscreen ? selectedColor : normalColor;
            fullscreenOffButton.GetComponent<Image>().color = !isFullscreen ? selectedColor : normalColor;
        }
    }
    
    public void SetAudioMasterVolume(float volume)
    {
        PlayerPrefs.SetFloat("masterVolume", volume);
        audioMasterPercent.text = Math.Round(volume * 100f).ToString() + "%";
        OnAudioMasterVolumeChanged?.Invoke(volume);
    }

    public void SetAudioMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("musicVolume", volume);
        audioMusicPercent.text = Math.Round(volume * 100f).ToString() + "%";
        OnAudioMusicVolumeChanged?.Invoke(volume);
    }

    public void SetAudioSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("sfxVolume", volume);
        audioSFXPercent.text = Math.Round(volume * 100f).ToString() + "%";
        OnAudioSFXVolumeChanged?.Invoke(volume);
    }

    #region Load Settings Methods
    private void LoadGraphicsSettings()
    {
        // Load and Highlight Quality
        int qualityLevel = PlayerPrefs.GetInt("masterQuality", QualitySettings.GetQualityLevel());
        HighlightQualityButton(qualityLevel);

        // Load and Highlight Fullscreen
        bool isFullscreen = PlayerPrefs.GetInt("masterFullscreen", Screen.fullScreen ? 1 : 0) == 1;
        HighlightFullscreenButton(isFullscreen);

        // Load Resolution
        int currentResolutionIndex = 0;
        for (int i = 0; i < _resolutions.Length; i++)
        {
            if (_resolutions[i].width == Screen.width && _resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
                break;
            }
        }
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void LoadGameplaySettings()
    {
        // Load and Apply Sensitivity
        float sensitivity = PlayerPrefs.GetFloat("mouseSensitivity");
        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = sensitivity;
            // Also update the percentage text on load
            sensitivityPercent.text = Math.Ceiling(sensitivity / 4.0f * 100f).ToString() + "%";
        }
    }

    private void LoadAudioSettings()
    {
        float masterVolume = PlayerPrefs.GetFloat("masterVolume");
        float musicVolume = PlayerPrefs.GetFloat("musicVolume");
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume");

        if (audioMasterSlider != null)
        {
            audioMasterSlider.value = masterVolume;
            audioMasterPercent.text = Math.Round(masterVolume * 100f).ToString() + "%";
        }
        if (audioMusicSlider != null)
        {
            audioMusicSlider.value = musicVolume;
            audioMusicPercent.text = Math.Round(musicVolume * 100f).ToString() + "%";
        }
        if (audioSFXSlider != null)
        {
            audioSFXSlider.value = sfxVolume;
            audioSFXPercent.text = Math.Round(sfxVolume * 100f).ToString() + "%";
        }
    }
    #endregion

    #region PrepareAndLoad Methods for OnClick Events
    public void PrepareAndLoadGraphicsSettings(GameObject graphicsSubmenu)
    {
        StartCoroutine(LoadSettingsRoutine(graphicsSubmenu, LoadGraphicsSettings));
    }

    public void PrepareAndLoadAudioSettings(GameObject audioSubmenu)
    {
        StartCoroutine(LoadSettingsRoutine(audioSubmenu, LoadAudioSettings));
    }

    public void PrepareAndLoadGameplaySettings(GameObject gameplaySubmenu)
    {
        StartCoroutine(LoadSettingsRoutine(gameplaySubmenu, LoadGameplaySettings));
    }

    private IEnumerator LoadSettingsRoutine(GameObject submenu, Action loadSettingsAction)
    {
        UIManager.Instance.OpenUIPanel(submenu);

        yield return null;

        loadSettingsAction?.Invoke();
    }
    #endregion
}
