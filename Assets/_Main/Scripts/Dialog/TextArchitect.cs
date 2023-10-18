using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using TMPro;
using UnityEditor.Build;
using UnityEngine;

public class TextArchitect
{
    public TextMeshProUGUI tmpro;

    public string currentText => tmpro.text;
    public string targetText;
    public string preText;
    public int proTextLength;

    public string fullTargetText => preText + targetText;

    public enum BuildMethod{
        instant,
        typewriter
    }

    public BuildMethod buildMethod = BuildMethod.typewriter;

    public Color textColor;

    public float speed = 1;

    public int charactersPerCycle => (speed <= 2f)? 1 : (speed <= 2.5f)? 3 : 4; //vitesse d'apparition des caractères

    public bool fastDisplay = false;

    public TextArchitect(TextMeshProUGUI tmpro){
        this.tmpro = tmpro;
    }

    public Coroutine Build(string text){
        preText = "";
        targetText = text;

        Stop();

        buildProcess = tmpro.StartCoroutine(BuildLogic());
        return buildProcess;
    }

    public Coroutine Append(string text){
        preText = tmpro.text;
        targetText = text;

        Stop();

        buildProcess = tmpro.StartCoroutine(BuildLogic());
        return buildProcess;
    }

    public Coroutine buildProcess = null; //la ligne en train d'être construite
    public bool isBuilding => buildProcess != null; //est-ce qu'une ligne est en train d'être construite

    public void Stop(){ //arrêter de construire une ligne
        if(!isBuilding)
            return;

        tmpro.StopCoroutine(buildProcess);
        buildProcess = null;
    }

    public IEnumerator BuildLogic(){
        Prepare();

        switch(buildMethod){
            case BuildMethod.instant:
                yield return BuildTypewriter();
                break;
            case BuildMethod.typewriter:
                yield return BuildTypewriter();
                break;
        }
    }

    public void OnComplete(){ //quand la ligne est finie d'être construite
        buildProcess = null;
        fastDisplay = false;
    }

    public void ForceComplete(){ //forcer la complétion de la ligne
        switch(buildMethod){
            case BuildMethod.typewriter:
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
                break;
        }

        Stop();
        OnComplete();
    }

    public void Prepare(){
        switch (buildMethod){
            case BuildMethod.instant:
                PrepareInstant();
                break;
            case BuildMethod.typewriter:
                PrepareTypewriter();
                break;
        }
    }

    public void PrepareInstant(){ //réinitialiser le texte et affiche la ligne à afficher
        tmpro.color = tmpro.color;
        tmpro.text = fullTargetText;
        tmpro.ForceMeshUpdate();
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
    }

    public void PrepareTypewriter(){ //réinitialise le texte et afficher la ligne à afficher, mais la cache pour l'effet typewriter
        tmpro.color = tmpro.color;
        tmpro.maxVisibleCharacters = 0;
        tmpro.text = preText;

        if(preText != ""){
            tmpro.ForceMeshUpdate();
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
        }

        tmpro.text += targetText;
        tmpro.ForceMeshUpdate(); 
    }

    public IEnumerator BuildTypewriter(){
        while(tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount){
            tmpro.maxVisibleCharacters += fastDisplay? charactersPerCycle * 5 : charactersPerCycle;
            yield return new WaitForSecondsRealtime(0.015f / speed);
        }
    }
}
