using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNDatabaseLinksSetup : MonoBehaviour
{
    public void SetupExternalLinks() {
        VariableStore.CreateVariable("playerMoney", 7, () => VNGameSave.activeFile.playerMoney, value => VNGameSave.activeFile.playerMoney = (int)value);
        VariableStore.CreateVariable("playerOx", 10, () => VNGameSave.activeFile.playerOx, value => VNGameSave.activeFile.playerOx = (int)value);
        VariableStore.CreateVariable("helpedAutostop", false, () => VNGameSave.activeFile.helpedAutostop, value => VNGameSave.activeFile.helpedAutostop = (bool)value);
    }
}
