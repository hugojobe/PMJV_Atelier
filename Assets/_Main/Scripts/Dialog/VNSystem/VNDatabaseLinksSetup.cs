using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNDatabaseLinksSetup : MonoBehaviour
{
    public void SetupExternalLinks() {
        VariableStore.CreateVariable("playerMoney", 7, () => VNGameSave.activeFile.playerMoney, value => VNGameSave.activeFile.playerMoney = (int)value);
        VariableStore.CreateVariable("playerOx", 10, () => VNGameSave.activeFile.playerOx, value => VNGameSave.activeFile.playerOx = (int)value);
        VariableStore.CreateVariable("helpedAutostop", false, () => VNGameSave.activeFile.helpedAutostop, value => VNGameSave.activeFile.helpedAutostop = (bool)value);
        VariableStore.CreateVariable("estAuRetour", false, () => VNGameSave.activeFile.estAuRetour, value => VNGameSave.activeFile.estAuRetour = (bool)value);
        VariableStore.CreateVariable("trouveSoeur", false, () => VNGameSave.activeFile.trouveSoeur, value => VNGameSave.activeFile.trouveSoeur = (bool)value);
        VariableStore.CreateVariable("playingSoeur", true, () => VNGameSave.activeFile.playingSoeur, value => VNGameSave.activeFile.playingSoeur = (bool)value);
        VariableStore.CreateVariable("hasObjetRare", false, () => VNGameSave.activeFile.hasObjetRare, value => VNGameSave.activeFile.hasObjetRare = (bool)value);
        VariableStore.CreateVariable("hasWeapon", false, () => VNGameSave.activeFile.hasWeapon, value => VNGameSave.activeFile.hasWeapon = (bool)value);
        VariableStore.CreateVariable("hasArgentMilicien", false, () => VNGameSave.activeFile.hasArgentMilicien, value => VNGameSave.activeFile.hasArgentMilicien = (bool)value);
        VariableStore.CreateVariable("BDKilledSoeur", false, () => VNGameSave.activeFile.BDKillSoeur, value => VNGameSave.activeFile.BDKillSoeur = (bool)value);
        VariableStore.CreateVariable("vieuxMort", false, () => VNGameSave.activeFile.vieuxMort, value => VNGameSave.activeFile.vieuxMort = (bool)value);    
        VariableStore.CreateVariable("soeurMorteAuRetour", false, () => VNGameSave.activeFile.soeurMorteAuRetour, value => VNGameSave.activeFile.soeurMorteAuRetour = (bool)value);
        VariableStore.CreateVariable("milicienMort", false, () => VNGameSave.activeFile.milicienMort, value => VNGameSave.activeFile.milicienMort = (bool)value);
    }
}
