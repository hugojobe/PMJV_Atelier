using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VNManager : MonoBehaviour
{
    public static VNManager instance;
    
    [SerializeField] private VisualNovelSO config;
    public Camera mainCamera;

    public TextMeshProUGUI oxText, moneyText;
    public Animator vesselAnimator;

    public bool isInLoseState;
    public CanvasGroup loseUiCG;
    private CanvasGroupManager loseCg => new CanvasGroupManager(loseUiCG, this);
    public TextMeshProUGUI loseSubtitle;
    public string loseSoeur, loseFrere;
    public Button loseContinueButton;

    public CanvasGroup winUiCG;
    private CanvasGroupManager winCg => new CanvasGroupManager(winUiCG, this);
    public TextMeshProUGUI winSubtitle;
    public TextMeshProUGUI winTitle;

    public static Prota prota = Prota.soeurAller;

    private void Awake() {
        instance = this;

        VNDatabaseLinksSetup linksSetup = GetComponent<VNDatabaseLinksSetup>();
        linksSetup.SetupExternalLinks();


        if(VNGameSave.activeFile == null)
            VNGameSave.activeFile = new VNGameSave();

        loseCg.Hide();
        winCg.Hide();
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

        VNGameSave saveFile = VNGameSave.activeFile;
        if(saveFile.pPlayerOx <= 0 && !isInLoseState) {
            Kill();
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

    public static void Kill() {
        instance.isInLoseState = true;
        VNGameSave saveFile = VNGameSave.activeFile;

        if(saveFile.playingSoeur && saveFile.estAuRetour) {
            saveFile.soeurMorteAuRetour = true;
            prota = Prota.soeurRetour;
        }
        else if(saveFile.playingSoeur && saveFile.estAuRetour == false) {
            saveFile.soeurMorteAuRetour = false;
            prota = Prota.soeurAller;
        }
        else if(saveFile.playingSoeur == false && saveFile.trouveSoeur){
            prota = Prota.frereAvecSoeur;
        }
        else
            prota = Prota.frereSansSoeur;

        instance.Lose(prota);
    }

    private void Lose(Prota prota) {

        loseSubtitle.text = (VNGameSave.activeFile.playingSoeur)? loseSoeur : loseFrere;

        LoadDialogFile("LoseDialog");
        
        loseContinueButton.GetComponentInChildren<TextMeshProUGUI>().text = (VNGameSave.activeFile.playingSoeur)? "Continuer" : "Quitter";

        Invoke("ShowCG", 1f);
    }

    public static void Win(string subTitle, bool died) {
        instance.winSubtitle.text = subTitle;
        instance.winTitle.text = died? "Vous avez succombé" : "Vous avez réussi";
        instance.winCg.Show();
    }

    private void ShowCG() {
        loseCg.Show();
    }

    public void Restart() {
        switch(prota) {
            case Prota.soeurAller:
            case Prota.soeurRetour:
                RestartFrere();
                break;
            case Prota.frereSansSoeur:
            case Prota.frereAvecSoeur:
                SceneManager.LoadScene("Menu");
                break;
        }
    }

    private void RestartFrere() {
        VNGameSave.activeFile.playerOx = 1;
        VNGameSave.activeFile.playingSoeur = false;

        instance.isInLoseState = false;

        loseCg.Hide();
        LoadDialogFile("IntroFrere");
    }

    private void LoadDialogFile(string fileName) {
        ExtensionGeneral.LoadNewDialogueFile(new string[] {fileName});
    }

    public enum Prota {soeurAller, soeurRetour, frereSansSoeur, frereAvecSoeur}
}