using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class NameContainer
{
    public GameObject dialogContainer;
    public TextMeshProUGUI nameText;

    public void Show(string nameToShow = ""){
        dialogContainer.SetActive(true);

        if(nameToShow != string.Empty){
            nameText.text = nameToShow;
        }
    }

    public void Hide(){
        dialogContainer.SetActive(false);
    }

    public void SetNameColor(Color color) => nameText.color = color;
    public void SetNameFont(TMP_FontAsset font) => nameText.font = font;
}
