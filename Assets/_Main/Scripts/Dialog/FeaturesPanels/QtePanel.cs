using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QtePanel : MonoBehaviour
{
    public static QtePanel instance;

    public CanvasGroup canvasGroup;
    public TextMeshProUGUI timer;
    public TextMeshProUGUI sequenceText;
    public Animator[] buttonVfxs;

    public bool isWaitingOnUserInput = false;
    public bool hasWon;

    public List<int> sequence;

    public bool canPlay = false;

    public List<int> playerSequence;

    private float timeLimit;
    private float currentTimer;

    private List<string> icons = new List<string>(){ 
        "<sprite name=fire>",
        "<sprite name=smoke>",
        "<sprite name=synergy>",
        "<sprite name=wave>"
    };

    private CanvasGroupManager cg => new CanvasGroupManager(canvasGroup, this);

    private void Awake() {
        if(instance == null) instance = this;
    }

    private void Start() {
        canvasGroup.alpha = 0;
        SetInteractibleState(false);
    }

    public void Show(int time) {
    //public void Show(){
        sequenceText.text = GenerateSequence();
        timeLimit = 12;
        currentTimer = 0;
        cg.Show();
        Invoke("SetInteractible", 0.5f);
        isWaitingOnUserInput = true;
    }

    private void SetInteractible() {
        SetInteractibleState(true);
    }

    public void Hide() {
        cg.Hide();
        SetInteractibleState(false);
        isWaitingOnUserInput = false;
    }

    private string GenerateSequence() {
        List<int> newSequence = new List<int>();

        for (int i = 0; i < 8; i++) {
            int randIndex = Random.Range(0, icons.Count);
            newSequence.Add(randIndex);
        }

        sequence = newSequence;

        string text = string.Concat(sequence.Select(i => icons[i]));

        canPlay = true;

        return text;
    }

    private void Update() {
        if(canPlay) {
            currentTimer += Time.deltaTime;

            timer.text = (timeLimit - currentTimer).ToString("00.00");

            if(currentTimer >= timeLimit){ 
                Lose();
            }

            TestAnswer();
        }
    }

    private void TestAnswer() {
        for (int i = 0; i < playerSequence.Count; i++){
            if(playerSequence[i] != sequence[i]){
                Lose();
                return;
            }
        }

        //Debug.Log($"{playerSequence.Count} == {sequence.Count}");
        if(playerSequence.Count == sequence.Count) {
            hasWon = true;
            FinishQTE();
        }
    }

    private void Lose() {
        hasWon = false;
        FinishQTE();
    }

    public void FinishQTE() {
        canPlay = false;
        sequence.Clear();
        playerSequence.Clear();
        Hide();
    }

    public void ClickButton(int index) {
        if(!canPlay) return;

        playerSequence.Add(index);
        buttonVfxs[index].SetTrigger("click");
    }

    private void SetInteractibleState(bool active){
        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
    }
}
