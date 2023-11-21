using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Object = UnityEngine.Object;

public class ExtensionGraphicPanels : CommandDatabaseExtension
{
    private static string[] PARAM_IMMEDIATE => new string[]{"-i", "-immediate"};
    private static string[] PARAM_SPEED => new string[]{"-spd", "-speed"};
    private static string[] PARAM_IMAGE => new string[]{"-img", "-image"};
    private static string[] PARAM_BLENDTEX => new string[]{"-bt", "-blendtex"};
    private static string[] PARAM_PANEL => new string[]{"-p", "-panel"};


    new public static void Extend(CommandDatabase database){
        database.AddCommand("setbackground", new Func<string[], IEnumerator>(SetBackground));
        database.AddCommand("clearbackground", new Func<string[], IEnumerator>(ClearLayer));
    }

    private static IEnumerator SetBackground(string[] data){
        string imageName = data[0];
        float transitionSpeed;
        bool immediate = false;
        string panelName = "";

        string blendTexName = "";
        string pathToGraphic = "";
        Object graphic = null;
        Texture blendTex = null;
        GraphicLayer layer = null;

        CommandParameters parameters = ConvertDataToParameters(data);

        parameters.TryGetValue(PARAM_SPEED, out transitionSpeed, defaultValue: 1f);
        parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue : false);
        parameters.TryGetValue(PARAM_IMAGE, out imageName);
        parameters.TryGetValue(PARAM_BLENDTEX, out blendTexName);
        parameters.TryGetValue(PARAM_PANEL, out panelName, defaultValue: "Background");

        GraphicPanel panel = GraphicPanelManager.instance.GetPanel(panelName);
        if(panel == null) yield break;
        
        pathToGraphic = FilePaths.backgroundImages + imageName;

        graphic = Resources.Load<Texture>(pathToGraphic);

        if(graphic == null)
            graphic = Resources.Load<VideoClip>(pathToGraphic);

        if(graphic == null){
            Debug.LogError($"Graphic '{imageName}' not found at path '{pathToGraphic}'");
            yield break;
        }

        if(!immediate && blendTexName != string.Empty){
            blendTex = Resources.Load<Texture>(FilePaths.blendTexs + blendTexName);
        }

        layer = panel.GetLayer(0, true);

        if (blendTex != null){
            yield return graphic is Texture
                ? layer.SetTexture(graphic as Texture, transitionSpeed, blendTex, pathToGraphic, immediate)
                : layer.SetVideo(graphic as VideoClip, transitionSpeed, false, blendTex, pathToGraphic, immediate);
        } else {
            yield return graphic is Texture
                ? layer.SetTexture(graphic as Texture, transitionSpeed, filePath: pathToGraphic, immediate: immediate)
                : layer.SetVideo(graphic as VideoClip, transitionSpeed, false, filePath: pathToGraphic, immediate: immediate);
        }
    }

    private static IEnumerator ClearLayer(string[] data){
        string panelName = "";
        float transitionSpeed;
        bool immediate = false;
        string blendTexName = "";

        Texture blendTex = null;
        GraphicLayer layer = null;

        CommandParameters parameters = ConvertDataToParameters(data);
        parameters.TryGetValue(PARAM_PANEL, out panelName, defaultValue: "Background");
        parameters.TryGetValue(PARAM_SPEED, out transitionSpeed, defaultValue: 1f);
        parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue : false);
        parameters.TryGetValue(PARAM_BLENDTEX, out blendTexName);

        GraphicPanel panel = GraphicPanelManager.instance.GetPanel(panelName);
        if(panel == null) yield break;
        
        layer = panel.GetLayer(0, false);
        if(layer == null) yield break;

        if(!immediate && blendTexName != string.Empty){
            blendTex = Resources.Load<Texture>(FilePaths.blendTexs + blendTexName);
        }

        if(blendTex != null)
            panel.Clear(transitionSpeed, blendTex, immediate);
        else
            //Debug.Log($"Clearing panel '{panelName}' with speed of '{transitionSpeed}'");
            panel.Clear(transitionSpeed);
    }
}
