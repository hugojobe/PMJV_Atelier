using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class TagManagers : MonoBehaviour
{
    public static Dictionary<string, Func<string>> tags = new Dictionary<string, Func<string>>(){
        {"<soeur>",         () => VNGameSave.activeFile.soeurName },
        {"<frere>",         () => VNGameSave.activeFile.frereName },
        {"<playerOx>",      () => VNGameSave.activeFile.playerOx.ToString() },
        {"<playerMoney>",   () => VNGameSave.activeFile.playerMoney.ToString() }, 
        {"<helpedAutostop>", () => VNGameSave.activeFile.helpedAutostop.ToString() },
        {"<money>",         () => "<color=#F4A015><sprite tint=1 name=money></color>"},  
        {"<risk>",          () => "<color=#FF3030><sprite tint=1 name=risk></color>"},  
        {"<ox>",            () => "<color=#2BA7EE>ox</color>"} 
    };
    private static readonly Regex tagRegex = new Regex("<\\w+>");
    private static readonly Regex endPhrase = new Regex("[.!?]+$");

    public static bool AddTag(string tagName, string tagValue) {
        tags.Add(tagName, () => tagValue); return true;
    }

    public static string Inject(string text, bool injectTags = true, bool injectVariables = true){
        if(injectTags)
            text = InjectTags(text);

        if(injectVariables)
            text = InjectVariables(text);

        return text;
    }

    public static string InjectTags(string value){
        if(tagRegex.IsMatch(value)){
            foreach(Match match in tagRegex.Matches(value)){
                if(tags.TryGetValue(match.Value, out var tagValueRequest)){
                    value = value.Replace(match.Value, tagValueRequest());
                }
            }
        }

        return value;
    }

    public static string InjectVariables(string value)
    {
        MatchCollection matches = Regex.Matches(value, VariableStore.REGEX_VARIABLE_IDS);
        List<Match> matchesList = matches.Cast<Match>().ToList();

        for(int i = matches.Count - 1; i >= 0; i--){
            Match match = matchesList[i];
            string variableName = match.Value.TrimStart(VariableStore.VARIABLE_ID, '!');
            bool negate = variableName.StartsWith("!");

            string endPhraseBackup = string.Empty;
            if(endPhrase.IsMatch(variableName)){
                endPhraseBackup = endPhrase.Match(variableName).Value;
            }

            variableName = variableName.TrimEnd('.','!', '?');
            if(!VariableStore.TryGetValue(variableName, out object variableValue)){
                Debug.LogError($"Variable '{variableName}' not found in string assignement !");
                continue;
            }

            if(negate && variableValue is bool) {
                variableValue = !(bool)variableValue;
            }

            int lenghtToBeRemoved = (match.Index + match.Length > value.Length)? value.Length - match.Index : match.Length;

            value = value.Remove(match.Index, lenghtToBeRemoved);
            value = value.Insert(match.Index, variableValue.ToString());
            value = value.Insert(match.Index + variableValue.ToString().Length, endPhraseBackup);
        }

        return value;
    }
}
