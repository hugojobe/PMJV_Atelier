using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TestDialogFiles : MonoBehaviour
{
    public TextAsset fileToRead;

    private void Start() {
        StartConversation();
    }

    public void StartConversation(){
        /*string fullPath = AssetDatabase.GetAssetPath(fileToRead);

        int resourcesIndex = fullPath.IndexOf("Resources/");
        string relativePath = fullPath.Substring(resourcesIndex + 10);

        string filePath = Path.ChangeExtension(relativePath, null);

        LoadFile(filePath);

        List<string> lines = FileManager.ReadTextAsset(fileToRead);

        foreach(string line in lines){

            if(string.IsNullOrWhiteSpace(line)){
                continue;
            }

            DialogLine dl = DialogParser.Parse(line);

            if(dl.commandData == null)
                continue;

            for(int i = 0; i < dl.commandData.commands.Count; i++){
                CommandData.Command command = dl.commandData.commands[i];
                Debug.Log($"Command [{i}] '{command.name}' has arguments [{string.Join(", ", command.arguments)}]");
            }
        }

        DialogueSystem.instance.Say(lines);*/
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
