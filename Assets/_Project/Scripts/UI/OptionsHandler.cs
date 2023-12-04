using System;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsHandler : MonoBehaviour
{
    private const string MASTER = "Master";
    private const string MUSIC = "Music";
    private const string SFX = "SFX";
    private const string ENVIRONMENT_SFX = "EnvironmentSFX";
    private const string PLAYER_SFX = "PlayerSFX";
    private const string ENEMY_SFX = "EnemySFX";
    
    private const string ON = "ON";
    private const string OFF = "OFF";
    private const string PERCENTAGE_SYMBOL = "%";

    [SerializeField] 
    private AudioMixer mainAudioMixer;
    
    [Space]
    
    [SerializeField]
    private TMP_Dropdown resolutionsDropdown;

    [SerializeField]
    private TextMeshProUGUI fullScreenValueText;

    [Space]
    
    [SerializeField]
    private TextMeshProUGUI masterVolumeValueText;
    [SerializeField] 
    private Slider masterVolumeSlider;

    [SerializeField]
    private TextMeshProUGUI musicVolumeValueText;
    [SerializeField] 
    private Slider musicVolumeSlider;

    [SerializeField]
    private TextMeshProUGUI sfxVolumeValueText;
    [SerializeField] 
    private Slider sfxVolumeSlider;
    
    [SerializeField]
    private TextMeshProUGUI environmentSfxVolumeValueText;
    [SerializeField] 
    private Slider environmentSfxVolumeSlider;

    [SerializeField]
    private TextMeshProUGUI playerSfxVolumeValueText;
    [SerializeField] 
    private Slider playerSfxVolumeSlider;

    [SerializeField]
    private TextMeshProUGUI enemySfxVolumeValueText;
    [SerializeField] 
    private Slider enemySfxVolumeSlider;

    public void Start()
    {
        SetResolutions();
        SetAllSliders();
    }

    private void SetResolutions()
    {
        if (resolutionsDropdown == null)
        {
            Debug.LogWarning("Dropdown is empty!");
            return;
        }
        
        resolutionsDropdown.options.Clear(); 
        
        foreach (var resolution in Screen.resolutions)
        {
            resolutionsDropdown.options.Add(new TMP_Dropdown.OptionData($"{resolution.width} x {resolution.height}"));
        }
        
        resolutionsDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        var resolution = Screen.resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width,resolution.height,Screen.fullScreen);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        fullScreenValueText.text = isFullScreen ? ON : OFF;
        Screen.fullScreen = isFullScreen;
    }

    private void SetAllSliders()
    {
        masterVolumeSlider.value = GetVolume(MASTER);
        musicVolumeSlider.value = GetVolume(MUSIC);
        sfxVolumeSlider.value = GetVolume(SFX);
        environmentSfxVolumeSlider.value = GetVolume(ENVIRONMENT_SFX);
        playerSfxVolumeSlider.value = GetVolume(PLAYER_SFX);
        enemySfxVolumeSlider.value = GetVolume(ENEMY_SFX);
    }

    public void SetMasterVolume(float volume)
    {
        SetVolumeAndText(volume, MASTER, masterVolumeValueText);
    }

    public void SetMusicVolume(float volume)
    {
        SetVolumeAndText(volume, MUSIC, musicVolumeValueText);
    }

    public void SetSfxVolume(float volume)
    {
        SetVolumeAndText(volume, SFX, sfxVolumeValueText);
    }
    
    public void SetEnvironmentSfxVolume(float volume)
    {
        SetVolumeAndText(volume, ENVIRONMENT_SFX, environmentSfxVolumeValueText);
    }
    
    public void SetPlayerSfxVolume(float volume)
    {
        SetVolumeAndText(volume, PLAYER_SFX, playerSfxVolumeValueText);
    }
    
    public void SetEnemySfxVolume(float volume)
    {
        SetVolumeAndText(volume, ENEMY_SFX, enemySfxVolumeValueText);
    }

    private void SetVolumeAndText(float volume, string mixerName, TextMeshProUGUI textMeshProRef)
    {
        textMeshProRef.text = GetVolumeString(volume);
        mainAudioMixer.SetFloat(mixerName, MathF.Log10(volume) * 20);
    }

    private float GetVolume(string mixerName)
    {
        mainAudioMixer.GetFloat(mixerName, out float volume);
        return MathF.Pow(10, volume / 20);
    }
    
    private string GetVolumeString(float volume)
    {
        volume *= 100;
        return $"{volume.ToString("N0")}{PERCENTAGE_SYMBOL}";
    }
}
