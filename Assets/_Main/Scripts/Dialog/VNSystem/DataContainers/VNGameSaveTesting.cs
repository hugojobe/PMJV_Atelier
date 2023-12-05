using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNGameSaveTesting : MonoBehaviour
{
    private void Start() {
        VNGameSave.activeFile = new VNGameSave();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.S)) {
            VNGameSave.activeFile.Save();
        }
        else if(Input.GetKeyDown(KeyCode.L)) {
            VNGameSave.activeFile = FileManager.Load<VNGameSave>($"{FilePaths.gameSaves}1{VNGameSave.FILE_TYPE}");
            VNGameSave.activeFile.Load();
        }
    }
}
