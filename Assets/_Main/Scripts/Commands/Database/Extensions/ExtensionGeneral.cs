using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using UnityEngine;

public class ExtensionGeneral : CommandDatabaseExtension
{
    private static readonly string[] PARAM_FILEPATH = new string[]{"-f", "-file", "-filepath"};
    private static readonly string[] PARAM_ENQUEUE = new string[]{"-e", "-enqueue"};

    new public static void Extend(CommandDatabase database){
        database.AddCommand("wait", new Func<string, IEnumerator>(Wait));

        database.AddCommand("showdialog", new Func<IEnumerator>(ShowDialog));
        database.AddCommand("hidedialog", new Func<IEnumerator>(HideDialog));

        database.AddCommand("load", new Action<string[]>(LoadNewDialogueFile));
    }

    public static void LoadNewDialogueFile(string[] data){
        string filePath = string.Empty;
        bool enqueue = false;

        CommandParameters parameters = ConvertDataToParameters(data);
        parameters.TryGetValue(PARAM_FILEPATH, out filePath);
        parameters.TryGetValue(PARAM_ENQUEUE, out enqueue, defaultValue: false);

        TextAsset file = Resources.Load<TextAsset>(FilePaths.GetPathToResource(FilePaths.resourcesDialogFiles, filePath));

        if(file == null) {
            Debug.LogError($"File '{filePath}' could not be loaded !");
            return;
        }

        List<string> lines = FileManager.ReadTextAsset(file, true);
        Conversation newConversation = new Conversation(lines);

        if(enqueue)
            DialogueSystem.instance.conversationManager.Enqueue(newConversation);
        else
            DialogueSystem.instance.conversationManager.StartConversation(newConversation);
    }

    private static IEnumerator Wait(string data){
        //Debug.Log("Waiting for time : " + data);
        if(float.TryParse(data, NumberStyles.Any, CultureInfo.InvariantCulture, out float waitTime))
            yield return new WaitForSecondsRealtime(waitTime);
    }

    private static IEnumerator ShowDialog(){
        yield return DialogueSystem.instance.dialogContainer.Show();
    }

    private static IEnumerator HideDialog(){
        yield return DialogueSystem.instance.dialogContainer.Hide();
    }
}
