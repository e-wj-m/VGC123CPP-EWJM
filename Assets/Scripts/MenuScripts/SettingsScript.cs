using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;   

public class SettingsScript : MonoBehaviour
{
    [Header("Buttons")]
    public Button settingsButton;
    public Button backButton;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    [Header("Audio Mixer")]
    public AudioMixer mixer;                
    [Tooltip("Expose these in the AudioMixer and match the names exactly")]
    public string masterParam = "MasterVol";
    public string musicParam = "MusicVol";
    public string sfxParam = "SFXVol";

    [Header("Master Volume")]
    public TMP_Text masterVolText;
    public Slider masterVolSlider;

    [Header("Music Volume")]
    public TMP_Text musicVolText;
    public Slider musicVolSlider;

    [Header("SFX Volume")]
    public TMP_Text sfxVolText;
    public Slider sfxVolSlider;

   
    const string PREF_MASTER = "vol_master";
    const string PREF_MUSIC = "vol_music";
    const string PREF_SFX = "vol_sfx";

    void Awake()
    {
       
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
        if (settingsPanel) settingsPanel.SetActive(false);

        if (settingsButton) settingsButton.onClick.AddListener(OpenSettings);
        if (backButton) backButton.onClick.AddListener(CloseSettings);

        SetupSlider(masterVolSlider, masterVolText, masterParam, PREF_MASTER, 0.8f);
        SetupSlider(musicVolSlider, musicVolText, musicParam, PREF_MUSIC, 0.8f);
        SetupSlider(sfxVolSlider, sfxVolText, sfxParam, PREF_SFX, 0.8f);
    }

    void OnDisable()
    {
        if (masterVolSlider) PlayerPrefs.SetFloat(PREF_MASTER, masterVolSlider.value);
        if (musicVolSlider) PlayerPrefs.SetFloat(PREF_MUSIC, musicVolSlider.value);
        if (sfxVolSlider) PlayerPrefs.SetFloat(PREF_SFX, sfxVolSlider.value);
        PlayerPrefs.Save();
    }

    private void OpenSettings() => SetMenus(settingsPanel, mainMenuPanel);
    private void CloseSettings() => SetMenus(mainMenuPanel, settingsPanel);

    private void SetMenus(GameObject show, GameObject hide)
    {
        if (show) show.SetActive(true);
        if (hide) hide.SetActive(false);
    }

    private void SetupSlider(Slider slider, TMP_Text label, string mixerParam, string prefKey, float default01)
    {
        if (!slider) return;

        slider.minValue = 0f;
        slider.maxValue = 1f;

        float v01 = PlayerPrefs.GetFloat(prefKey, default01);
        slider.value = v01;

        ApplyVolume(mixerParam, v01);
        UpdatePercentLabel(label, v01);

        slider.onValueChanged.AddListener(v =>
        {
            ApplyVolume(mixerParam, v);
            UpdatePercentLabel(label, v);
            PlayerPrefs.SetFloat(prefKey, v);
        });
    }

    private void ApplyVolume(string mixerParam, float value01)
    {
        if (!mixer || string.IsNullOrEmpty(mixerParam)) return;

        float v = Mathf.Clamp(value01, 0.0001f, 1f);
        float dB = Mathf.Log10(v) * 20f;

        bool ok = mixer.SetFloat(mixerParam, dB);   
        if (!ok)
            Debug.LogWarning($"[Settings] Mixer parameter '{mixerParam}' not found. Expose it in the AudioMixer.");
    }

    private static void UpdatePercentLabel(TMP_Text label, float value01)
    {
        if (label) label.text = $"{Mathf.RoundToInt(value01 * 100f)}%";
    }
}