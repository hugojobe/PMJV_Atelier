using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNManager : MonoBehaviour
{
    public static VNManager instance;

    private void Awake() {
        instance = this;
    }

    public void LoadFile(string filePath) {
        List<string> lines = new List<string>();
        TextAsset file = Resources.Load<TextAsset>(filePath);

        try {
            lines = FileManager.ReadTextAsset(file);
        } catch { 
            Debug.LogError($"Dialogue file at path 'Rsources/{filePath}' does not exist");   
        }

        DialogueSystem.instance.Say(lines, filePath);
    }
}
