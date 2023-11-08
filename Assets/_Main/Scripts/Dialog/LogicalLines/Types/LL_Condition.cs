using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LogicalLinesUtilities.Encapsulation;
using static LogicalLinesUtilities.Conditions;

public class LL_Condition : ILogicalLine {
    public string keyword => "if";
    public const string ELSE = "else";
    public readonly string[] CONTAINERS = new string[]{"(", ")"};

    public IEnumerator Execute(DialogLine line) {
        string rawCondition = ExtractCondition(line.rawData.Trim());
        bool conditionResult = EvaluateCondition(rawCondition);

        Conversation currentConversation = DialogueSystem.instance.conversationManager.conversation;
        int currentProgress = DialogueSystem.instance.conversationManager.conversationProgress;

        EncapsulatedData ifData = RipEncapsulationData(currentConversation, currentProgress, false);
        EncapsulatedData elseData = new EncapsulatedData();
        
        if(ifData.endingIndex + 1 < currentConversation.Count){
            string nextLine = currentConversation.GetLines()[ifData.endingIndex + 1].Trim();
            if(nextLine == ELSE){
                elseData = RipEncapsulationData(currentConversation, ifData.endingIndex + 1, false);
                ifData.endingIndex = elseData.endingIndex;
            }
        }
        
        currentConversation.SetProgress(ifData.endingIndex);

        EncapsulatedData selData = conditionResult? ifData : elseData;
        if(!selData.isNull && selData.lines.Count > 0) {
            Conversation newConversation = new Conversation(selData.lines);
            DialogueSystem.instance.conversationManager.EnqueuePriority(newConversation);
        }

        yield return null;
    }

    public bool Matches(DialogLine line) {
        return line.rawData.Trim().StartsWith(keyword);
    }

    private string ExtractCondition(string line) {
        int startIndex = line.IndexOf(CONTAINERS[0]) + 1;
        int endIndex = line.IndexOf(CONTAINERS[1]);

        return line.Substring(startIndex, endIndex - startIndex).Trim();
    }
}
