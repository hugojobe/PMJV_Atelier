using System.Collections;
using System.Collections.Generic;
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
}
