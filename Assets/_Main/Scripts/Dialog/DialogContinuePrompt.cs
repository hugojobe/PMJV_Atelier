using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogContinuePrompt : MonoBehaviour
{
    private RectTransform root;

    public Animator anim;
    public TextMeshProUGUI tmpro;

    public bool isVisible => anim.gameObject.activeSelf;

    private void Start() {
        root = GetComponent<RectTransform>();
    }

    public void Show(){
        if(tmpro.text == string.Empty){
            if(isVisible)
                Hide();

            return;
        }

        tmpro.ForceMeshUpdate();

        anim.gameObject.SetActive(true);
        root.transform.SetParent(tmpro.transform);

        TMP_CharacterInfo finalCharacter = tmpro.textInfo.characterInfo[tmpro.textInfo.characterCount-1];
        Vector3 targetPos = finalCharacter.bottomRight;
        float characterWidth = finalCharacter.pointSize * 0.5f;
        targetPos = new Vector3(targetPos.x + characterWidth, targetPos.y, 0);

        root.localPosition = targetPos;
    }

    public void Hide(){
        anim.gameObject.SetActive(false);
    }
}
