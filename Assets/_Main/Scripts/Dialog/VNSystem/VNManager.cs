using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VNManager : MonoBehaviour
{
    public static VNManager instance;
    
    [SerializeField] private VisualNovelSO config;
    public Camera mainCamera;

    public TextMeshProUGUI oxText, moneyText;
    public Animator vesselAnimator;

    public bool isInLoseState;

    private void Awake() {
        instance = this;

        VNDatabaseLinksSetup linksSetup = GetComponent<VNDatabaseLinksSetup>();
        linksSetup.SetupExternalLinks();


        if(VNGameSave.activeFile == null)
            VNGameSave.activeFile = new VNGameSave();

    }

    private void Start() {
        LoadGame();
    }

    private void Update() {
        oxText.text = "<color=#2BA7EE>OX</color> " + VNGameSave.activeFile.playerOx;
        moneyText.text = "<color=#F4A015><sprite tint=1 name=money></color> " + VNGameSave.activeFile.playerMoney;

        if(Input.GetKeyDown(KeyCode.F5)) {
            VariableStore.PrintAllVariables();
        }

        if(VNGameSave.activeFile.pPlayerOx <= 0 && !isInLoseState) {
            //Lose();
        }
    }

    private void LoadGame() {
        if(VNGameSave.activeFile.newGame) {
            List<string> lines = FileManager.ReadTextAsset(config.startingFile);
            Conversation start = new Conversation(lines);
            DialogueSystem.instance.Say(start);
        } else {
            VNGameSave.activeFile.Activate();
        }
    }

    /*public void KillSoeur() {
        Lose(Prota.soeur);
    }

    private void Lose(Prota prota) {
        isInLoseState = true;

        switch(prota){
            case Prota.soeur:
                LoadDialogFile("LoseSoeur");
                break;
            case Prota.frere:
                break;
        }
    }*/

    private void LoadDialogFile(string fileName) {
        ExtensionGeneral.LoadNewDialogueFile(new string[] {fileName});
    }

    enum Prota {soeurAller, soeurRetour, frere}
}
