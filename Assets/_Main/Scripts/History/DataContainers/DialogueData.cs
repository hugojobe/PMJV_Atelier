using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public string currentDialog = "";
    public string currentSpeaker = "";

    public string dialogFont;
    public Color dialogColor;
    public float dialogScale;

    public string speakerFont;
    public Color speakerColor;
    public float speakerScale;

    public static DialogueData Capture() {
        DialogueData data = new DialogueData();
        DialogueSystem ds = DialogueSystem.instance;

        var dialogueText = ds.dialogContainer.dialogText;
        var nameText = ds.dialogContainer.nameContainer.nameText;
        
        data.currentDialog = dialogueText.text;
        data.dialogFont = FilePaths.resourcesFont + dialogueText.font.name;
        data.dialogColor = dialogueText.color;
        data.dialogScale = dialogueText.fontSize;

        data.currentSpeaker = nameText.text;
        data.speakerFont = FilePaths.resourcesFont + nameText.font.name;
        data.speakerColor = nameText.color;
        data.speakerScale = nameText.fontSize;

        return data;
    }

    public static void Apply(DialogueData data) {
        DialogueSystem ds = DialogueSystem.instance;

        var dialogueText = ds.dialogContainer.dialogText;
        var nameText = ds.dialogContainer.nameContainer.nameText;
    
        dialogueText.text = data.currentDialog;
        dialogueText.maxVisibleCharacters = data.currentDialog.Length;

        dialogueText.color = data.dialogColor;
        dialogueText.fontSize = data.dialogScale;

        nameText.text = data.currentSpeaker;
        if(nameText.text != string.Empty)
            ds.dialogContainer.nameContainer.Show();
        else
            ds.dialogContainer.nameContainer.Hide();
        
        nameText.color = data.speakerColor;
        nameText.fontSize = data.speakerScale;

        if(data.dialogFont != dialogueText.font.name) {
            TMP_FontAsset fontAsset = HistoryCache.LoadFont(data.dialogFont);
            if(fontAsset != null)
                dialogueText.font = fontAsset;
        }

        if(data.dialogFont != nameText.font.name) {
            TMP_FontAsset fontAsset = HistoryCache.LoadFont(data.speakerFont);
            if(fontAsset != null)
                nameText.font = fontAsset;
        }
    }
}
