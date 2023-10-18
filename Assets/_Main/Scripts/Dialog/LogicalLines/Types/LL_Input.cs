using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LL_Input : ILogicalLine
{
    public string keyword => "input";

    public IEnumerator Execute(DialogLine line){
        Debug.Log("Input triggered");
        throw new System.NotImplementedException();
    }

    public bool Matches(DialogLine line){
        return (line.hasSpeaker && line.speakerData.name.ToLower() == keyword);
    }
}
