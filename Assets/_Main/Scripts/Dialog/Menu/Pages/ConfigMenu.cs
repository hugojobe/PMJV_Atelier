using SOHNE.Accessibility.Colorblindness;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ConfigMenu : MenuPage {
    public UI_ITEMS ui;
    public Colorblindness colorblindnessManager;

    [SerializeField] private AnimationCurve audioFalloffCurve;

    public static ConfigMenu instance;

    private VNConfiguration config => VNConfiguration.activeConfig;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        LoadConfig();
    }

    private void LoadConfig() {
        if(File.Exists(VNConfiguration.filePath))
            VNConfiguration.activeConfig = FileManager.Load<VNConfiguration>(VNConfiguration.filePath, VNConfiguration.ENCRYPT_CONFIG);
        else
            VNConfiguration.activeConfig = new VNConfiguration();

        VNConfiguration.activeConfig.Load();
    }

    private void OnApplicationQuit() {
        VNConfiguration.activeConfig.Save();
        VNConfiguration.activeConfig = null;
    }

    // UI CALLABLE FUNCTIONS

    public void SetTypewritterEffectSpeed() {
        config.dialogTextSpeed = ui.typewriterSpeed.value;
        ui.typewritterText.text = (Mathf.Round(config.dialogTextSpeed * 10f) / 10f).ToString();
        
        if(DialogueSystem.instance != null)
            DialogueSystem.instance.conversationManager.architect.speed = config.dialogTextSpeed;
    }

    public void SetMusicVolume() {
        config.musicVolume = ui.musicVolume.value;
        AudioMixerGroup mixer = AudioManager.instance.musicMixer;
        AnimationCurve audioFalloff = AudioManager.instance.audioFalloffCurve;

        ui.musicText.text = Mathf.RoundToInt(config.musicVolume * 100).ToString();
        
        mixer.audioMixer.SetFloat(AudioManager.MUSIC_VOLUME_PARAMETER_NAME, audioFalloff.Evaluate(config.musicVolume));
    }
    public void SetSFXVolume() {
        config.sfxVolume = ui.sfxVolume.value;
        AudioMixerGroup mixer = AudioManager.instance.sfxMixer;
        AnimationCurve audioFalloff = AudioManager.instance.audioFalloffCurve;

        ui.sfxText.text = Mathf.RoundToInt(config.sfxVolume * 100).ToString();
        
        mixer.audioMixer.SetFloat(AudioManager.SFX_VOLUME_PARAMETER_NAME, audioFalloff.Evaluate(config.sfxVolume));
    }

    public void SetBlindMode() {
        config.colorblindSettings = ui.colorblindMode.value;
        ui.colorblindMode.value = config.colorblindSettings;
        ui.colorblindMode.captionText.text = ((ColorblindTypes)config.colorblindSettings).ToString();
        Debug.Log("Set value to " + config.colorblindSettings);
        colorblindnessManager.SetMode(config.colorblindSettings);
    }


}

[System.Serializable]
public class UI_ITEMS {
    public Toggle fullscreenToggle;
    public Slider typewriterSpeed;
    public Slider musicVolume;
    public Slider sfxVolume;
    public TMP_Dropdown colorblindMode;

    public TextMeshProUGUI typewritterText;
    public TextMeshProUGUI musicText;
    public TextMeshProUGUI sfxText;
}
