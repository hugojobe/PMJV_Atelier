using SOHNE.Accessibility.Colorblindness;
using UnityEngine;

[System.Serializable]
public class VNConfiguration
{
    public static VNConfiguration activeConfig;
    public const bool ENCRYPT_CONFIG = true;

    public static string filePath => $"{FilePaths.root}userprefs.cfg";

    public float dialogTextSpeed = 1f;

    public float musicVolume = 1;
    public float sfxVolume = 1;

    public int colorblindSettings = 0;

    public void Load() {
        var ui = ConfigMenu.instance.ui;
        
        ui.typewriterSpeed.value = dialogTextSpeed;
        ui.typewritterText.text = (Mathf.Round(dialogTextSpeed * 10f) / 10f).ToString();
    
        ui.musicVolume.value = musicVolume;
        ConfigMenu.instance.SetMusicVolume();

        ui.sfxVolume.value = sfxVolume;
        ConfigMenu.instance.SetSFXVolume();
        
        ui.colorblindMode.SetValueWithoutNotify(colorblindSettings);
        ui.colorblindMode.captionText.text = ((ColorblindTypes)colorblindSettings).ToString();
        ConfigMenu.FindObjectOfType<Colorblindness>().SetMode(colorblindSettings);
    }

    public void Save() {
        FileManager.Save(filePath, JsonUtility.ToJson(this), ENCRYPT_CONFIG);
    }

}
