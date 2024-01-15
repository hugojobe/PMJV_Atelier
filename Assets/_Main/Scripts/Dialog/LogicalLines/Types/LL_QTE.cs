using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LogicalLinesUtilities.Encapsulation;

public class LL_QTE : ILogicalLine {
    public string keyword => "qte";
    private const char RESULT_IDENTIFIER = '-';

    public IEnumerator Execute(DialogLine line) {

        Debug.Log($"Executing LL line \"{line}\"");

        Conversation currentConversation = DialogueSystem.instance.conversationManager.conversation;
        int progress = DialogueSystem.instance.conversationManager.conversationProgress;
        EncapsulatedData data = RipEncapsulationData(currentConversation, progress, true, parentStartingIndex:currentConversation.fileStartIndex);
        List<QteResult> results = GetResultsFromData(data);

        QtePanel panel = QtePanel.instance;

        panel.Show(int.Parse(line.dialogData.rawData));

        //panel.Show();

        while(panel.isWaitingOnUserInput)
            yield return null;

        QteResult result = panel.hasWon? results[0] : results[1];

        Conversation newConversation = new Conversation(result.resultLines, file:currentConversation.file, fileStartIndex:result.startIndex, fileEndIndex:result.endIndex);
        DialogueSystem.instance.conversationManager.conversation.SetProgress(data.endingIndex - currentConversation.fileStartIndex);
        DialogueSystem.instance.conversationManager.EnqueuePriority(newConversation);
    }

    public bool Matches(DialogLine line) {
        return(line.hasSpeaker && line.speakerData.name.ToLower() == keyword);
    }

    private List<QteResult> GetResultsFromData(EncapsulatedData data)
    {
        List<QteResult> results = new List<QteResult>();
        int encapsulationDepth = 0;
        bool isFirstResult = true;

        QteResult result = new QteResult{resultLines = new List<string>()};

        int resultIndex = 0, i = 0;
        for(i = 1; i < data.lines.Count; i++) {
            var line = data.lines[i];
            if(IsResultStart(line) && encapsulationDepth == 1){
                if(!isFirstResult){
                    result.startIndex = data.startingIndex + (resultIndex + 1);
                    result.endIndex = data.startingIndex + (i - 1);
                    results.Add(result);
                    result = new QteResult{
                        resultLines = new List<string>()
                    };
                }

                resultIndex = i;
                isFirstResult = false;
                continue;
            }

            AddLinesToResults(line, ref result, ref encapsulationDepth);
        }

        if(!results.Contains(result)){
            result.startIndex = data.startingIndex + (resultIndex + 1);
            result.endIndex = data.startingIndex + (i - 2);
            results.Add(result);
        }

        return results;
    }

    private void AddLinesToResults(string line, ref QteResult qteResult, ref int encapsulationDepth){
        line.Trim();

        if(IsEncapsulationStart(line)){
            if(encapsulationDepth > 0)
                qteResult.resultLines.Add(line);
            encapsulationDepth++;
            return;
        }

        if(IsEncapsulationEnd(line)){
            encapsulationDepth--;

            if(encapsulationDepth > 0)
                qteResult.resultLines.Add(line);

            return;
        }

        qteResult.resultLines.Add(line);
    }

    private bool IsResultStart (string line) => line.Trim().StartsWith(RESULT_IDENTIFIER);

    private struct QteResult{
        public List<string> resultLines;
        public int startIndex;
        public int endIndex;
    }
}
