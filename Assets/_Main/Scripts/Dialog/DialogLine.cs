using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogLine
{
    public string rawData {get; private set;} = string.Empty;
    public SpeakerData speakerData;
    public DialogData dialogData;
    public CommandData commandData;

    public bool hasSpeaker => speakerData != null;
    public bool hasDialog => dialogData != null;
    public bool hasCommands => commandData != null;

    public DialogLine(string rawLine, string speaker, string dialog, string commands){
        rawData = rawLine;
        this.speakerData = (string.IsNullOrWhiteSpace(speaker) ? null : new SpeakerData(speaker));
        this.dialogData = (string.IsNullOrWhiteSpace(dialog) ? null : new DialogData(dialog));
        this.commandData = (string.IsNullOrWhiteSpace(commands) ? null : new CommandData(commands));
    }
}
