using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogueSystem : MonoBehaviour
{
    public DialogConfig config;
    public DialogContainer dialogContainer = new DialogContainer();
    public ConversationManager conversationManager;
    public TextArchitect architect;

    public CanvasGroup blackOccluderCanvasGroup;

    public bool initialized = false;

    public bool isRunningConversation => conversationManager.isRunning;

    public static DialogueSystem instance;

    public delegate void DialogSystemEvent();
    public DialogSystemEvent onUserPromptNext;
    public DialogSystemEvent onClear;

    public DialogContinuePrompt continuePrompt;

    private void Awake() {
        if(instance == null){
            instance = this;
            Initialize();
        }
        else 
            DestroyImmediate(gameObject);
    }

    public void Initialize(){
        if(initialized)
            return;
        
        architect = new TextArchitect(dialogContainer.dialogText);
        conversationManager = new ConversationManager(architect);
    }

    public void OnUserPromptNext(){
        onUserPromptNext?.Invoke();
    }

    public void OnSystemPromptClear() {
        onClear?.Invoke();
    }

    public void OnStartViewingHistory() {
        conversationManager.allowUserPrompt = false;
        continuePrompt.Hide();
    }

    public void OnStopViewingHistry() {
        conversationManager.allowUserPrompt = true;
        continuePrompt.Show();
    }

    public void ApplySpeakerDataToDialogContainer(string speakerName){
        Character character = CharacterManager.instance.GetCharacter(speakerName);
        CharacterConfigData config = character != null ? character.config : CharacterManager.instance.GetCharacterConfig(speakerName);

        ApplySpeakerDataToDialogContainer(config);
    }

    public void ApplySpeakerDataToDialogContainer(CharacterConfigData config){
        dialogContainer.SetDialogColor(config.dialogColor);
        dialogContainer.SetDialogFont(config.dialogFont);
        dialogContainer.SetDialogSize(config.fontSize);
        dialogContainer.nameContainer.SetNameColor(config.nameColor);
        dialogContainer.nameContainer.SetNameFont(config.nameFont);
    }

    public void ShowSpeakerName(string speakerName = ""){
        if(speakerName.ToLower() != "narrator"){
            dialogContainer.nameContainer.Show(speakerName);
        } else {
            HideSpeakerName();
            dialogContainer.nameContainer.nameText.text = "";
        }
    }
    public void HideSpeakerName() => dialogContainer.nameContainer.Hide();

    public Coroutine Say(string speaker, string dialog){
        List<string> conversation = new List<string>() {$"{speaker} \"{dialog}\""};
        return Say(conversation);
    }

    public Coroutine Say(List<string> lines, string filePath = ""){
        Conversation conversation = new Conversation(lines, file:filePath);
        //Debug.Log($"Successfully created conversation {conversation} with {conversation.Count} lines");
        return conversationManager.StartConversation(conversation);
    }

    public Coroutine Say(Conversation conversation){
        return conversationManager.StartConversation(conversation);
    }
}
