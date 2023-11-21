using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryTesting : MonoBehaviour
{
    public DialogueData data;
    public List<AudioData> audioData;
    public List<GraphicData> graphicData;
    public List<CharacterData> characterData;

    private void Update() {
        data = DialogueData.Capture();
        audioData = AudioData.Capture();
        graphicData = GraphicData.Capture();
        characterData = CharacterData.Capture();
    }
}
