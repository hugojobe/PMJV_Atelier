using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Morse : MiniGame
{

    public TextMeshProUGUI displayText;
    public TextMeshProUGUI playerInputText;

    public Color loseColor;

    private const string DOT_SYMBOL = ".";
    private const string LINE_SYMBOL = "_";

    private enum Symbols {dot = 0, line};
    private List<Symbols> referenceSequence;
    private List<Symbols> playerSequence = new List<Symbols>();

    private bool playerCanPlay = false;
    private bool gameRunning;

    private void Start(){
        GenerateSequence();
        Display(playerInputText, "<size=50>_> READ");
        StartCoroutine(DisplaySequence());
    }

    private void Update(){
        if(playerCanPlay){
            Display(playerInputText, 
                string.Concat(string.Join("", playerSequence.Select(symbol => (symbol is Symbols.dot) ? "." : "_"))) +
                new string('?', referenceSequence.Count - playerSequence.Count));
        }

        if(playerSequence.Count > 0 && playerCanPlay){
            if(playerSequence.Last() != referenceSequence[playerSequence.Count - 1])
                StartCoroutine(LoseCoroutine());
        }

        string playerString = string.Join("", playerSequence.Select(symbol => (symbol is Symbols.dot) ? "." : "_"));
        string refString = string.Join("", referenceSequence.Select(symbol => (symbol is Symbols.dot) ? "." : "_"));

        if(playerString == refString){
            StartCoroutine(CorrectSequence());
        }

        VesselNode node = VesselManager.instance.currentState;
        if(node is VesselProblem)
            (node as VesselProblem).currentProblemTimer = 0;
    }

    private IEnumerator CorrectSequence() {
        playerCanPlay = false;
        Display(displayText, "<size=60>CORRECT");
        yield return new WaitForSecondsRealtime(1.5f);
        SolveProblem();
    }
    
    public void OnDotButtonPressed() {
        if(!playerCanPlay) return;
        
        playerSequence.Add(Symbols.dot);
    }

    public void OnLineButtonPressed() {
        if(!playerCanPlay) return;

        playerSequence.Add(Symbols.line);
    }

    private IEnumerator LoseCoroutine() {
        playerCanPlay = false;
        playerInputText.color = loseColor;

        Display(displayText, "<size=60>ERROR");

        yield return new WaitForSecondsRealtime(1.5f);

        VesselProblem.ProblemNotSolved();
    }

    private IEnumerator DisplaySequence() {
        Display(displayText, " ");
        yield return new WaitForSeconds(1.5f);

        foreach(Symbols symbol in referenceSequence){
            Display(displayText, (symbol is Symbols.dot)? "." : "_");

            if (symbol == Symbols.dot) {
                AudioManager.instance.PlaySoundEffect("Court");
                yield return new WaitForSecondsRealtime(0.8f);
            } else if (symbol == Symbols.line) {
                AudioManager.instance.PlaySoundEffect("Long");   
                yield return new WaitForSecondsRealtime(0.8f);
            }

            Display(displayText, " ");
            yield return new WaitForSecondsRealtime(0.1f);
        }

        playerCanPlay = true;
    }

    private void GenerateSequence(){
        int sequenceLength = Random.Range(3, 7);
        referenceSequence = new List<Symbols>();

        for(int i = 0; i < sequenceLength; i++){
            Symbols newSymbol = (Symbols)Random.Range(0, 2);
            referenceSequence.Add (newSymbol);
        }

        string referenceString = string.Concat(referenceSequence.Select(symbol => (symbol is Symbols.dot) ? "." : "_"));
        Debug.Log(referenceString);
    }

    private void Display(TextMeshProUGUI label, string toDisplay, bool reset = true) {
        if(reset)
            label.text = "";

        label.text += toDisplay;
    }
}