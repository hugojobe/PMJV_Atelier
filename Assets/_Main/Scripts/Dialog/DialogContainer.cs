using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[Serializable]
public class DialogContainer
{
    private const float DEFAULT_FADE_SPEED = 3f;
    public GameObject dialogContainer;
    public NameContainer nameContainer;
    public TextMeshProUGUI dialogText;

    private CanvasGroup rootCG => dialogContainer.GetComponent<CanvasGroup>();

    private Coroutine hidingCoroutine, showingCoroutine;

    private bool isShowing => showingCoroutine != null;
    private bool isHiding => hidingCoroutine != null;
    private bool isFading => isShowing || isFading;

    public void SetDialogColor(Color color) => dialogText.color = color;
    public void SetDialogFont(TMP_FontAsset font) => dialogText.font = font;
    public void SetDialogSize(float fontSize) => dialogText.fontSize = fontSize;

    public Coroutine Show(){
        rootCG.interactable = true;
        rootCG.blocksRaycasts = true;
        if(isShowing) return showingCoroutine;

        if(isHiding){
            DialogueSystem.instance.StopCoroutine(hidingCoroutine);
            hidingCoroutine = null;
        }

        showingCoroutine = DialogueSystem.instance.StartCoroutine(Fading(1f));

        return showingCoroutine;
    }

    public Coroutine Hide(){
        rootCG.interactable = false;
        rootCG.blocksRaycasts = false;
        if(isHiding) return hidingCoroutine;

        if(isShowing){
            DialogueSystem.instance.StopCoroutine(showingCoroutine);
            showingCoroutine = null;
        }

        hidingCoroutine = DialogueSystem.instance.StartCoroutine(Fading(0f));

        return hidingCoroutine;
    }

    private IEnumerator Fading(float alpha){
        CanvasGroup cg = rootCG;

        while(cg.alpha != alpha){
            cg.alpha = Mathf.MoveTowards(cg.alpha, alpha, Time.deltaTime * DEFAULT_FADE_SPEED);
            yield return null;
        }

        showingCoroutine = null;
        hidingCoroutine = null;
    }
}
