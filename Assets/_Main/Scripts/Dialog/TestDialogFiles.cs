using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class TestDialogFiles : MonoBehaviour
{
    public TextAsset fileToRead;

    private void Start() {
        StartConversation();
    }

    public void StartConversation(){
        List<string> lines = FileManager.ReadTextAsset(fileToRead);

        /*foreach(string line in lines){

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
        }*/

        DialogueSystem.instance.Say(lines);
    }
}
