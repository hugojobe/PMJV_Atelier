using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)){
            PromptAdvance();
        }
    }

    public void PromptAdvance(){
        DialogueSystem.instance.OnUserPromptNext();
    }
}
