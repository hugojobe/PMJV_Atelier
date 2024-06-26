using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    private PlayerInput input;
    private List<(InputAction action, Action<InputAction.CallbackContext> command)> actions = new List<(InputAction action, Action<InputAction.CallbackContext> command)>();

    public bool canUserPromptNext = true;

    private void Awake() {
        if(instance == null) 
            instance = this;

        input = GetComponent<PlayerInput>();
        InitializeActions();
    }

    private void InitializeActions(){
        actions.Add((input.actions["Next"], OnNext));
        actions.Add((input.actions["HistoryForward"], OnHistoryForward));
        actions.Add((input.actions["HistoryBack"], OnHistoryBack));
        actions.Add((input.actions["HistoryLogs"], OnHistoryToggleLog));
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
        #if UNITY_EDITOR
        #else
            if(!canUserPromptNext) return;
        #endif
        DialogueSystem.instance.OnUserPromptNext();
    }

    public void OnNext(){
        #if UNITY_EDITOR
        #else
            if(!canUserPromptNext) return;
        #endif
        DialogueSystem.instance.OnUserPromptNext();
    }

    public void OnHistoryForward(InputAction.CallbackContext ctx){
        if(VesselManager.instance.currentStateDisplay == VesselState.VN)
            HistoryManager.instance.GoForward();
    }

    public void OnHistoryBack(InputAction.CallbackContext ctx){
        if(VesselManager.instance.currentStateDisplay == VesselState.VN)
            HistoryManager.instance.GoBack();
    }

    public void OnHistoryToggleLog(InputAction.CallbackContext ctx){
        if(VesselManager.instance.currentStateDisplay == VesselState.VN){
            var logs = HistoryManager.instance.logManager;
            if(!logs.isOpen)
                logs.Open();
            else
                logs.Close();
        }
    }
}
