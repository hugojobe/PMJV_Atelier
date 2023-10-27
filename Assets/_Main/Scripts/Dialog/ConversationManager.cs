using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ConversationManager
{
    private DialogueSystem dialogSystem => DialogueSystem.instance;

    public Coroutine process = null;
    public bool isRunning => process != null;

    public TextArchitect architect = null;
    public bool userPrompt = false;

    private LogicalLineManager logicalLineManager;

    public Conversation conversation => conversationQueue.IsEmpty() ? null : conversationQueue.top;
    public int conversationProgress => (conversationQueue.IsEmpty())? -1 : conversationQueue.top.GetProgress();
    private ConversationQueue conversationQueue;

    public ConversationManager(TextArchitect architect){
        this.architect = architect;
        dialogSystem.onUserPromptNext += OnUserPromptNext;

        logicalLineManager = new LogicalLineManager();

        conversationQueue = new ConversationQueue();
    }

    public void Enqueue(Conversation conversation) => conversationQueue.Enqueue(conversation);
    public void EnqueuePriority(Conversation conversation) => conversationQueue.EnqueuePriority(conversation);

    public void OnUserPromptNext(){
        userPrompt = true;
    }

    public Coroutine StartConversation(Conversation conversation){
        StopConversation();

        Enqueue(conversation);

        process = dialogSystem.StartCoroutine(RunningConversation());

        return process;
    }

    public void StopConversation(){
        if(!isRunning)
            return;
        
        dialogSystem.StopCoroutine(process);
        process = null;
    }

    public IEnumerator RunningConversation(){
        while(!conversationQueue.IsEmpty()){
            Conversation currentConversation = conversation;

            if(currentConversation.HasReachedEnd()){
                conversationQueue.Dequeue();
                continue;
            }
            
            string rawLine = currentConversation.CurrentLine();

            //Debug.Log($"Parsing line {rawLine}");

            if(string.IsNullOrWhiteSpace(rawLine)){ //Passer les lignes vides
                TryAdvanceConversation(currentConversation);
                continue;
            }

            DialogLine line = DialogParser.Parse(rawLine);

            if(logicalLineManager.TryGetLogic(line, out Coroutine logic)){
                yield return logic;
            } else {
                if(line.hasDialog){ //Afficher le dialogue
                    //Debug.Log("Dialog detected in line");
                    yield return LineRunDialog(line);
                }
                
                //Debug.Log("Line.hasCommands : " + line.hasCommands + ((line.hasCommands)? " : " + line.commandData.commands[0].name : ""));

                if(line.hasCommands)
                    yield return LineRunCommands(line);

                if(line.hasDialog){
                    yield return WaitForUserInput();
                    CommandManager.instance.StopAllProcesses();
                }
            }

            TryAdvanceConversation(currentConversation);
        }

        process = null;
    }

    private void TryAdvanceConversation(Conversation conversation){
        conversation.IncrementProgress();

        if(conversation != conversationQueue.top)
            return;

        if(conversation.HasReachedEnd()){
            conversationQueue.Dequeue();
        }
    }

    IEnumerator LineRunDialog(DialogLine line){
        if(line.hasSpeaker){ 
            HandleSpeakerLogic(line.speakerData);
        } else {
            dialogSystem.HideSpeakerName();
        }

        yield return BuildLineSegment(line.dialogData);
    }

    private void HandleSpeakerLogic(SpeakerData speakerData){
        bool characterMustBeCreated = (speakerData.makeCharacterEnter)? speakerData.isCastingPosition : speakerData.isCastingExpressions;

        Character character = CharacterManager.instance.GetCharacter(speakerData.name, true); ///////////////////////////////////////////////////// was true

        if(speakerData.makeCharacterEnter && (!character.isVisible && !character.isRevealing))
            character.Show();

        dialogSystem.ShowSpeakerName(speakerData.displayName); //line.speakerData.finalName est le nom affiché à l'écran

        DialogueSystem.instance.ApplySpeakerDataToDialogContainer(speakerData.displayName);

        if(speakerData.isCastingPosition)
            character.MoveToPosition(speakerData.displayPosition);       
        
        if(speakerData.isCastingExpressions){
            
            character.OnRecieveCastingExpression(speakerData.castExpression);

        }
    }

    IEnumerator LineRunCommands(DialogLine line){
        List<CommandData.Command> commands = line.commandData.commands;
        foreach(CommandData.Command command in commands){
            //Debug.Log($"Processing command '{command.name}' with {command.arguments.Length} arguments");
            
            if(command.waitForCompletion || command.name.ToLower() == "wait"){
                //Debug.Log("Waiting");
                CoroutineWrapper cw = CommandManager.instance.Execute(command.name, command.arguments);
                while(!cw.isDone){
                    if(userPrompt){
                        CommandManager.instance.StopCurrentProcess();
                        userPrompt = false;
                    }
                    yield return null;
                }
            }
            else
                CommandManager.instance.Execute(command.name, command.arguments);
        }

        yield return null;
    }

    IEnumerator BuildLineSegment(DialogData line){
        for(int i = 0; i < line.segments.Count; i++){
            DialogData.DialogSegment segment = line.segments[i];
        
            yield return WaitForDialogSegmentSignalToBeTriggered(segment);

            yield return BuildDialog(segment.dialog, segment.appendText);
        }
    }

    IEnumerator WaitForDialogSegmentSignalToBeTriggered(DialogData.DialogSegment segment){
        switch(segment.startSignal){
            case DialogData.DialogSegment.StartSignal.C:
            case DialogData.DialogSegment.StartSignal.A:
                yield return WaitForUserInput();
                break;
            case DialogData.DialogSegment.StartSignal.WC:
            case DialogData.DialogSegment.StartSignal.WA:
                yield return new WaitForSecondsRealtime(segment.signalDelay);
                break;
            default:
                yield return null;
                break;
        }
    }

    IEnumerator BuildDialog(string dialog, bool append = false){
        if(!append){
            architect.Build(dialog);
        } else {
            architect.Append(dialog);
        }

        yield return null;
    }

    IEnumerator WaitForUserInput(){
        dialogSystem.continuePrompt.Show();

        while(!userPrompt)
            yield return null;
        
        dialogSystem.continuePrompt.Hide();

        userPrompt = false;
    }
}
