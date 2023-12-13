using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveAndLoadMenu : MenuPage
{
    public static SaveAndLoadMenu instance;

    private const int MAX_FILES = 6;
    private string savePath => FilePaths.gameSaves; 
    private bool loadedFilesForFirstTime = false;

    public SaveLoadSlot[] saveSlots;

    public Texture emptyFileImage;

    private void Awake() {
        instance = this;
    }

    public override void Open() {
        base.Open();

        if(!loadedFilesForFirstTime)
            PopulateSaveSlots();
    }

    private void PopulateSaveSlots() {
        for(int i = 0; i < saveSlots.Length; i++) {
            SaveLoadSlot slot = saveSlots[i];

            if(i < MAX_FILES) {
                slot.root.SetActive(true);
                string filePath = $"{FilePaths.gameSaves}{i}{VNGameSave.FILE_TYPE}";
                slot.fileNumber = i;
                slot.filePath = filePath;
                slot.PopulateDetails();
             } else {
                slot.root.SetActive(false);
            }
        }
    }
}
