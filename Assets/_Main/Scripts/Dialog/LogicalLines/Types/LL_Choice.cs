using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class LL_Choice : ILogicalLine
{
    public string keyword => "choice";
    private const char ENCAPSULATION_START = '{';
    private const char ENCAPSULATION_END = '}';
    private const char CHOICE_IDENTIFIER = '-';

    public IEnumerator Execute(DialogLine line){
        RawChoiceData data = RipChoiceData();

        List<Choice> choices = GetChoicesFromData(data);

        string title = line.dialogData.rawData;
        ChoicePanel panel = ChoicePanel.instance;
        string[] choiceTitle = choices.Select(c => c.title).ToArray();

        panel.Show(title, choiceTitle);

        while(panel.isWaitingOnUserChoice)
            yield return null;

        Choice selecedChoice = choices[panel.lastDecision.answerIndex];

        Conversation newConversation = new Conversation(selecedChoice.resultLines);
        DialogueSystem.instance.conversationManager.conversation.SetProgress(data.endingIndex);
        DialogueSystem.instance.conversationManager.EnqueuePriority(newConversation);
    }

    public bool Matches(DialogLine line){
        return(line.hasSpeaker && line.speakerData.name.ToLower() == keyword);
    }

    private RawChoiceData RipChoiceData(){
        Conversation currentConversation  = DialogueSystem.instance.conversationManager.conversation;
        int currentProgress = DialogueSystem.instance.conversationManager.conversationProgress;
        int encapsulationDepth = 0;
        RawChoiceData data = new RawChoiceData{ lines = new List<string>(), endingIndex = 0};

        for(int i = currentProgress; i <currentConversation.Count; i++){
            string line = currentConversation.GetLines()[i];
            data.lines.Add(line);

            if(IsEncapsulationStart(line)){
                encapsulationDepth++;
                continue;
            }

            if(IsEncapsulationEnd(line)){
                encapsulationDepth--;
                if(encapsulationDepth == 0){
                    data.endingIndex = i;
                    break;
                }
            }
        }

        return data;
    }

    private List<Choice> GetChoicesFromData(RawChoiceData data)
    {
        List<Choice> choices = new List<Choice>();
        int encapsulationDepth = 0;
        bool isFirstChoice = true;

        Choice choice = new Choice{ title = string.Empty, resultLines = new List<string>()};

        foreach(string line in data.lines.Skip(1)){
            if(IsChoiceStart(line) && encapsulationDepth == 1){
                if(!isFirstChoice){
                    choices.Add(choice);
                    choice = new Choice{
                        title = string.Empty,
                        resultLines = new List<string>()
                    };
                }

                choice.title = line.Trim().Substring(1);
                isFirstChoice = false;
                continue;
            }

            AddLinesToResults(line, ref choice, ref encapsulationDepth);
        }

        if(!choices.Contains(choice))
            choices.Add(choice);

        return choices;
    }

    private void AddLinesToResults(string line, ref Choice choice, ref int encapsulationDepth){
        line.Trim();

        if(IsEncapsulationStart(line)){
            if(encapsulationDepth > 0)
                choice.resultLines.Add(line);
            encapsulationDepth++;
            return;
        }

        if(IsEncapsulationEnd(line)){
            encapsulationDepth--;

            if(encapsulationDepth > 0)
                choice.resultLines.Add(line);

            return;
        }

        choice.resultLines.Add(line);
    }

    private bool IsEncapsulationStart(string line) => line.Trim().StartsWith(ENCAPSULATION_START);
    private bool IsEncapsulationEnd(string line) => line.Trim().StartsWith(ENCAPSULATION_END);
    private bool IsChoiceStart (string line) => line.Trim().StartsWith(CHOICE_IDENTIFIER);

    private struct RawChoiceData{
        public List<string> lines;
        public int endingIndex;
    }

    private struct Choice{
        public string title;
        public List<string> resultLines;
    }
}
