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
}
