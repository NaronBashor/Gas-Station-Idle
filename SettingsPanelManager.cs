using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject settingsPanel; // The panel to show/hide
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Button zoomOnButton;
    [SerializeField] private Button zoomOffButton;

    //[Header("Audio Sources")]
    //[SerializeField] private AudioSource sfxSource;
    //[SerializeField] private AudioSource musicSource;

    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string ZOOM_KEY = "ZoomEnabled";

    private void Start()
    {
        // Load saved settings
        LoadSettings();

        // Add listeners to UI elements
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        zoomOnButton.onClick.AddListener(EnableZoom);
        zoomOffButton.onClick.AddListener(DisableZoom);

        // Initially hide the settings panel
        settingsPanel.SetActive(false);
    }

    // Toggle the visibility of the settings panel
    public void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    // Called when the SFX volume slider value is changed
    private void OnSFXVolumeChanged(float volume)
    {
        //sfxSource.volume = volume;
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume); // Save the SFX volume
    }

    // Called when the Music volume slider value is changed
    private void OnMusicVolumeChanged(float volume)
    {
        //musicSource.volume = volume;
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume); // Save the music volume
    }

    // Enable zoom when the zoom-on button is pressed
    private void EnableZoom()
    {
        PlayerPrefs.SetInt(ZOOM_KEY, 1); // Save the zoom state as enabled
        ToggleZoom(true);
    }

    // Disable zoom when the zoom-off button is pressed
    private void DisableZoom()
    {
        PlayerPrefs.SetInt(ZOOM_KEY, 0); // Save the zoom state as disabled
        ToggleZoom(false);
    }

    // Load settings from PlayerPrefs
    private void LoadSettings()
    {
        // Load SFX Volume
        if (PlayerPrefs.HasKey(SFX_VOLUME_KEY)) {
            float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY);
            sfxVolumeSlider.value = sfxVolume;
            //sfxSource.volume = sfxVolume;
        }

        // Load Music Volume
        if (PlayerPrefs.HasKey(MUSIC_VOLUME_KEY)) {
            float musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY);
            musicVolumeSlider.value = musicVolume;
            //musicSource.volume = musicVolume;
        }

        // Load Zoom Setting
        if (PlayerPrefs.HasKey(ZOOM_KEY)) {
            bool zoomEnabled = PlayerPrefs.GetInt(ZOOM_KEY) == 1;
            ToggleZoom(zoomEnabled);
        }
    }

    public void OnZoomFeatureTrueButtonPress()
    {
        ToggleZoom(true);
    }

    public void OnZoomFeatureFalseButtonPress()
    {
        ToggleZoom(false);
    }

    // Method to enable or disable zoom (implement based on your game's zoom functionality)
    private void ToggleZoom(bool isEnabled)
    {
        PlayerPrefs.SetString("Zoom", isEnabled.ToString());
    }
}
