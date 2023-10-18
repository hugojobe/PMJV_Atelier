using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogParser : MonoBehaviour
{
    public const string commandRegexPattern = @"[\w\[\]]*[^\s]\(";

    public static DialogLine Parse(string rawLine){
        //Debug.Log($"Parsing line - '{rawLine}'");

        (string speaker, string dialog, string commands) = RipContent(rawLine);

        return new DialogLine(rawLine, speaker, dialog, commands);
    }

    public static (string, string, string) RipContent(string rawLine){
        string speaker = "", dialogue = "", commands = "";

        //Reconnaitre e dialogue
        int dialogStart = -1;
        int dialogEnd = -1;
        bool isQuoted = false;

        for(int i = 0; i < rawLine.Length; i++){
            char current = rawLine[i];
            if(current == '\\')
                isQuoted = !isQuoted;
            else if(current == '"' && !isQuoted){
                if(dialogStart == -1)
                    dialogStart = i;
                else if(dialogEnd == -1)
                    dialogEnd = i;
            }
            else
                isQuoted = false;
        }

        //Reconnaitre les commandes
        Regex commandRegex = new Regex(commandRegexPattern);
        MatchCollection matches = commandRegex.Matches(rawLine);
        int commandStart = -1;
        foreach(Match match in matches){
            if(match.Index < dialogStart || match.Index > dialogEnd){
                commandStart = match.Index;
                break;
            }
        }

        if(commandStart != -1 && dialogStart == -1 && dialogEnd == -1){
            return("", "", rawLine.TrimEnd());
        }

        if(dialogStart != -1 && dialogEnd != -1  && (commandStart == -1 || commandStart > dialogEnd))
        {
            speaker = rawLine.Substring(0, dialogStart).Trim();
            dialogue = rawLine.Substring(dialogStart + 1, dialogEnd - dialogStart - 1).Replace("\\\"", "\"");
            if(commandStart != -1)
                commands = rawLine.Substring(commandStart).Trim();
        }
        else if(commandStart != 1 && dialogStart > commandStart)
            commands = rawLine;
        else 
            dialogue = rawLine;
        
        
        if(!rawLine.Contains('"'))
            dialogue = "";

        return (speaker, dialogue, commands);
    }
}
