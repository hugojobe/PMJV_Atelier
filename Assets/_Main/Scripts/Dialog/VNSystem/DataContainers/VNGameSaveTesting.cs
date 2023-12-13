using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNGameSaveTesting : MonoBehaviour
{
    VNGameSave save;

    private void Start() {
        VNGameSave.activeFile = new VNGameSave();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.S)) {
            VNGameSave.activeFile.Save();
        }
        else if(Input.GetKeyDown(KeyCode.L)) {
            try{
                save = VNGameSave.Load($"{FilePaths.gameSaves}1{VNGameSave.FILE_TYPE}", true);
            } catch(System.Exception e) {
                Debug.Log($"Could not load save file! {e}");
            }
        }
    }
}
