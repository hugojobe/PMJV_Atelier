using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerInput input;
    private List<(InputAction action, Action<InputAction.CallbackContext> command)> actions = new List<(InputAction action, Action<InputAction.CallbackContext> command)>();

    private void Awake() {
        input = GetComponent<PlayerInput>();
        InitializeActions();
    }

    private void InitializeActions(){
        actions.Add((input.actions["Next"], OnNext));
    }

    private void OnEnable() {
        foreach(var inputAction in actions){
            inputAction.action.performed += inputAction.command;
        }
    }

    private void OnDisable() {
        foreach(var inputAction in actions){
            inputAction.action.performed -= inputAction.command;
        }
    }

    public void OnNext(InputAction.CallbackContext ctx){
        DialogueSystem.instance.OnUserPromptNext();
    }

    public void OnNext(){
        DialogueSystem.instance.OnUserPromptNext();
    }
}
