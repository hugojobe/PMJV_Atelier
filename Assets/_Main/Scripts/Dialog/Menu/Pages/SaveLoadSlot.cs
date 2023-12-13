using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveLoadSlot : MonoBehaviour
{
    public GameObject root;
    public RawImage previewImage;
    public TextMeshProUGUI titleText;
    public Button deleteButton;
    public Button saveButton;
    public Button loadButton;

    [HideInInspector] public int fileNumber = 0;
    [HideInInspector] public string filePath;

    public void PopulateDetails() {
        if(File.Exists(filePath)) {
            VNGameSave file = VNGameSave.Load(filePath);
            PopulateDetailsFromFile(file);
        } else {
            PopulateDetailsFromFile(null);
        }
    }

    public void PopulateDetailsFromFile(VNGameSave file) {
        if(file == null) {
            titleText.text = "Empty slot";
            
            deleteButton.gameObject.SetActive(false);
            loadButton.gameObject.SetActive(false);

            if(VNMenuManager.instance.menuType == VNMenuManager.MenuType.MainMenu)
                saveButton.gameObject.SetActive(false);
            else
                saveButton.gameObject.SetActive(true);
            
            previewImage.texture = SaveAndLoadMenu.instance.emptyFileImage;
        } else {
            titleText.text = file.timestamp;

            deleteButton.gameObject.SetActive(true);
            loadButton.gameObject.SetActive(true);

            if(VNMenuManager.instance.menuType == VNMenuManager.MenuType.MainMenu)
                saveButton.gameObject.SetActive(false);
            else
                saveButton.gameObject.SetActive(true);

            byte[] data = File.ReadAllBytes(file.screenshotPath);
            Texture2D screenshotPreview = new Texture2D(1, 1);
            ImageConversion.LoadImage(screenshotPreview, data);
            previewImage.texture = screenshotPreview;
        }
    }

    public void Delete() {
        File.Delete(filePath);
        PopulateDetails();
    }

    public void Load() {
        VNGameSave file = VNGameSave.Load(filePath, false);
        SaveAndLoadMenu.instance.Close(closeAllMenus: true);

        if(SceneManager.GetActiveScene().name == "Menu") {
            MainMenu.instance.LoadGame(file);
        } else {
            file.Activate();
        }
    }

    public void Save() {
        var activeSave = VNGameSave.activeFile;
        activeSave.slotNumber = fileNumber;

        activeSave.Save();

        PopulateDetailsFromFile(activeSave);
    }
}
