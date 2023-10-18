using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class SpeakerData
{
    public string name, castingName;
    public string displayName => castingName != string.Empty ? castingName : name;
    public Vector2 displayPosition;
    public string castExpression;

    public bool isCastingName => castingName != string.Empty;
    public bool isCastingPosition = false;
    public bool isCastingExpressions => !string.IsNullOrEmpty(castExpression);

    public bool makeCharacterEnter = false;

    public const string DISPLAY_NAME_ID = " as ";
    public const string DISPLAY_POSITION_ID = " at ";
    public const string DISPLAY_EXPRESSION_ID = " [";
    public const char AXISSEPARATOR = ':';

    private const string ENTER_KEYWORD = "enter ";

    private string ProcessKeywords(string rawSpeaker){
        //Debug.Log($"Processing enter keyword for line \"{rawSpeaker}\"");
        if(rawSpeaker.StartsWith(ENTER_KEYWORD)){
            rawSpeaker = rawSpeaker.Substring(ENTER_KEYWORD.Length);
            makeCharacterEnter = true;
        }

        return rawSpeaker;
    }
    
    public SpeakerData(string rawSpeaker){
        rawSpeaker = ProcessKeywords(rawSpeaker);

        string pattern = @$"{DISPLAY_NAME_ID}|{DISPLAY_POSITION_ID}|{DISPLAY_EXPRESSION_ID.Insert(DISPLAY_EXPRESSION_ID.Length-1, @"\")}";
        MatchCollection matches = Regex.Matches(rawSpeaker, pattern);

        castingName = "";
        displayPosition = Vector2.zero;
        castExpression = "";
        
        if(matches.Count == 0){
            name = rawSpeaker;
            return;
        }

        int index = matches[0].Index;

        name = rawSpeaker.Substring(0, index);

        for(int i = 0; i < matches.Count; i++){
            Match match = matches[i];

            int startIndex = 0;
            int endIndex = 0;

            if(match.Value == DISPLAY_NAME_ID){
                startIndex = match.Index + DISPLAY_NAME_ID.Length;
                endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                castingName = rawSpeaker.Substring(startIndex, endIndex - startIndex);
            } 
            else if(match.Value == DISPLAY_POSITION_ID){
                isCastingPosition = true;
                startIndex = match.Index + DISPLAY_POSITION_ID.Length;
                endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                string displayPos = rawSpeaker.Substring(startIndex, endIndex - startIndex);

                string[] axis = displayPos.Split(AXISSEPARATOR, System.StringSplitOptions.RemoveEmptyEntries);

                float.TryParse(axis[0], out displayPosition.x);

                if(axis.Length > 1){
                    float.TryParse(axis[1], out displayPosition.y);
                }
            } 
            else if(match.Value == DISPLAY_EXPRESSION_ID){
                startIndex = match.Index + DISPLAY_EXPRESSION_ID.Length;
                endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                string displayExpression = rawSpeaker.Substring(startIndex, endIndex - (startIndex + 1));

                castExpression = displayExpression;
            }
        }
    }
}
