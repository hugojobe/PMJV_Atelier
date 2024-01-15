using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TextAsset = UnityEngine.TextAsset;

public class ExtensionGeneral : CommandDatabaseExtension
{
    private static readonly string[] PARAM_FILEPATH = new string[]{"-f", "-file", "-filepath"};
    private static readonly string[] PARAM_ENQUEUE = new string[]{"-e", "-enqueue"};
    
    private static string[] PARAM_IMMEDIATE => new string[]{"-i", "-immediate"};
    private static string[] PARAM_BLENDTEX => new string[]{"-bt", "-blendtex"};
    private static string[] PARAM_SPEED => new string[]{"-spd", "-speed"};
    private static string[] PARAM_TIME => new string[]{"-d", "duration"};

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
        database.AddCommand("switchtovesselmode", new Func<string[], IEnumerator>(SwitchToVesselMode));
        database.AddCommand("hidevessel", new Action(HideVessel));
        database.AddCommand("load", new Action<string[]>(LoadNewDialogueFile));
        database.AddCommand("debug", new Action<string>(DebugToConsole));
        database.AddCommand("kill", new Action(Kill));
        database.AddCommand("end", new Action<string>(Win));
        database.AddCommand("blockinteraction", new Action(BlockInteraction));
        database.AddCommand("resumeinteraction", new Action(ResumeInteraction));
    }

    public static void LoadNewDialogueFile(string[] data){
        string filePath = string.Empty;
        bool enqueue = false;

        CommandParameters parameters = ConvertDataToParameters(data);
        parameters.TryGetValue(PARAM_FILEPATH, out filePath);
        parameters.TryGetValue(PARAM_ENQUEUE, out enqueue, defaultValue: false);

        Debug.Log("FilePath : " + filePath);

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

    public static IEnumerator HideDialog(){
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

    public static IEnumerator HideBG(string[] data){
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

    private static IEnumerator SwitchToVesselMode(string[] data) {
        VesselManager.instance.ChangeState(VesselState.IDLE);

        string exitFile = string.Empty;
        float phaseDuration;

        CommandParameters parameters = ConvertDataToParameters(data);
        parameters.TryGetValue(PARAM_FILEPATH, out exitFile);
        parameters.TryGetValue(PARAM_TIME, out phaseDuration, 60f);

        AudioManager.instance.StopTrack(0);
        AudioManager.instance.StopTrack(1);
        AudioManager.instance.PlayTrack(FilePaths.resourcesMusic + "Vessel", channel: 0);

        yield return VesselManager.instance.EnterVesselMode(exitFile, phaseDuration);
    }

    private static void HideVessel() {
        VesselManager.instance?.animator.SetTrigger("Reset");
        VesselManager.instance?.Reset();
    }

    private static void DebugToConsole(string data) {
        Debug.Log(data);
    }

    private static void Kill() {
        VNManager.Kill();
    }

    private static void Win(string data) {
        int endNumber = int.Parse(data);

        switch (endNumber) {
            case 1:
                VNManager.Win("Votre frère sera fier de vous", false);
                break;
            case 2:
                VNManager.Win("Votre soeur s'est sacrifiée pour vous sauver", false);
                break;
            case 3:
                VNManager.Win("Votre soeur n'a pas pu vous sauver", true);
                break;
            case 4:
                VNManager.Win("Vous serez séparés à jamais", true);
                break;
        }
    }

    private static void BlockInteraction() {
        PlayerInputManager.instance.canUserPromptNext = false;
    }

    private static void ResumeInteraction() {
        PlayerInputManager.instance.canUserPromptNext = true;
    }
}
