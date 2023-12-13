using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public AudioClip menuMusic;
    public CanvasGroup mainPanel;
    private CanvasGroupManager mainCG;

    public static MainMenu instance;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        mainCG = new CanvasGroupManager(mainPanel, this);
        AudioManager.instance.PlayTrack(menuMusic, channel:0, startingVolume:1);
    }

    public void StartNewGame() {
        VNGameSave.activeFile = new VNGameSave();


        StartCoroutine(StartingGame());
    }

    public void LoadGame(VNGameSave file) {
        VNGameSave.activeFile = file;
        StartCoroutine(StartingGame());
    }


    private IEnumerator StartingGame() {
        VNConfiguration.activeConfig.Save();

        mainCG.Show();
        AudioManager.instance.StopTrack(0);

        while(!mainCG.isVisible)
            yield return null;
        
        SceneManager.LoadScene("VisualNovel");
    }
}
