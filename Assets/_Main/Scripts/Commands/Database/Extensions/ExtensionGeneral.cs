using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using TextAsset = UnityEngine.TextAsset;

public class ExtensionGeneral : CommandDatabaseExtension
{
    private static readonly string[] PARAM_FILEPATH = new string[]{"-f", "-file", "-filepath"};
    private static readonly string[] PARAM_ENQUEUE = new string[]{"-e", "-enqueue"};
    
    private static string[] PARAM_IMMEDIATE => new string[]{"-i", "-immediate"};
    private static string[] PARAM_BLENDTEX => new string[]{"-bt", "-blendtex"};
    private static string[] PARAM_SPEED => new string[]{"-spd", "-speed"};
    private const string MATERIAL_MAINTEX =  "_MainTex";
    private const string MATERIAL_BLENDTEX = "_BlendTex";
    private const string MATERIAL_BLEND =    "_Blend";
    private const string MATERIAL_ALPHA =    "_Alpha";

    new public static void Extend(CommandDatabase database){
        database.AddCommand("wait", new Func<string, IEnumerator>(Wait));

        database.AddCommand("showdialog", new Func<IEnumerator>(ShowDialog));
        database.AddCommand("hidedialog", new Func<IEnumerator>(HideDialog));
        database.AddCommand("hideall", new Func<string[], IEnumerator>(HideBG));
        database.AddCommand("showall", new Func<string[], IEnumerator>(ShowBG));

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

    private static IEnumerator ShowBG(string[] data) {
        bool immediate = false;
        float speed = 1f;
        float target = 1f;
        Texture blend = null;

        CommandParameters parameters = ConvertDataToParameters(data);

        parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
        parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
        parameters.TryGetValue(PARAM_BLENDTEX, out blend, defaultValue: null);

        DialogueSystem.instance.StartCoroutine(Fading(immediate, speed, target, blend));
        yield return null;
    }

    private static IEnumerator HideBG(string[] data){
        bool immediate = false;
        float speed = 1f;
        float target = 0f;
        Texture blend = null;

        CommandParameters parameters = ConvertDataToParameters(data);

        parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
        parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
        parameters.TryGetValue(PARAM_BLENDTEX, out blend, defaultValue: null);

        DialogueSystem.instance.StartCoroutine(Fading(immediate, speed, target, blend));
        yield return null;
    }

    private static IEnumerator Fading(bool immediate, float speed, float target, Texture blend) {
        CanvasGroup cg = DialogueSystem.instance.blackOccluderCanvasGroup;
        Image renderer = DialogueSystem.instance.blackOccluderCanvasGroup.GetComponent<Image>();

        bool isBlending = blend != null;
        bool fadingIn = target > 0;

        renderer.material.SetTexture(MATERIAL_BLENDTEX, blend);
        renderer.material.SetFloat(MATERIAL_ALPHA, isBlending ? 1 : (fadingIn ? 0 : 1));
        renderer.material.SetFloat(MATERIAL_BLEND, isBlending ? (fadingIn ? 0 : 1) : 1);

        string opacityParam = isBlending ? MATERIAL_BLEND : MATERIAL_ALPHA;

        if(immediate)
            cg.alpha = target;
        else{
            while(renderer.material.GetFloat(opacityParam) != target){
                float opacity = Mathf.MoveTowards(renderer.material.GetFloat(opacityParam), target, speed * Time.deltaTime);
                renderer.material.SetFloat(opacityParam, opacity);
                yield return null;
            }
        }
    }
}
