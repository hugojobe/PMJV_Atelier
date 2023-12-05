using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VNConversationData
{
    public List<string> conversation = new List<string>();
    public int startIndex, endIndex;
    public int progress;
}
