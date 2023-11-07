using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class TagManagers : MonoBehaviour
{
    public static readonly Dictionary<string, Func<string>> tags = new Dictionary<string, Func<string>>(){
        {"<mainChar>",      () => "MainCharacter"},
        {"<playerOx>",      () => "15"}
    };
    private static readonly Regex tagRegex = new Regex("<\\w+>");
    private static readonly Regex endPhrase = new Regex("[.!?]+$");


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

            string variableName = match.Value.TrimStart(VariableStore.VARIABLE_ID);

            string endPhraseBackup = string.Empty;
            if(endPhrase.IsMatch(variableName)){
                endPhraseBackup = endPhrase.Match(variableName).Value;
            }

            variableName = variableName.TrimEnd('.','!', '?');
            if(!VariableStore.TryGetValue(variableName, out object variableValue)){
                Debug.LogError($"Variable '{variableName}' not found in string assignement !");
                continue;
            }

            int lenghtToBeRemoved = (match.Index + match.Length > value.Length)? value.Length - match.Index : match.Length;

            value = value.Remove(match.Index, lenghtToBeRemoved);
            value = value.Insert(match.Index, variableValue.ToString());
            value = value.Insert(match.Index + variableValue.ToString().Length, endPhraseBackup);
        }

        return value;
    }
}
