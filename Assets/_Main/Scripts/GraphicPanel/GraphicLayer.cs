using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicLayer
{
    public int layerDepth = 0;
    public const string LAYER_OBJCT_NAME_FORMAT = "Layer : {0}";
    public Transform panel;
    public GraphicObject currentGraphic = null;
    private List<GraphicObject> oldGraphics = new List<GraphicObject>();

    public Coroutine SetTexture(string filePath, float transitionSpeed = 1, Texture blendingTexture = null, bool immediate = false){
        Texture tex = Resources.Load<Texture>(filePath);

        if(tex == null){
            Debug.LogError($"Can't find texture at path '{filePath}'");
            return null;
        }

        return SetTexture(tex, transitionSpeed, blendingTexture, filePath, immediate);
    }

    public Coroutine SetTexture(Texture tex, float transitionSpeed = 1, Texture blendingTexture = null, string filePath = "", bool immediate = false){
        return CreateGraphic(tex, transitionSpeed, filePath, blendingTexture: blendingTexture, immediate: immediate);
    }

    private Coroutine CreateGraphic(Texture graphicData, float transitionSpeed, string filePath, Texture blendingTexture = null, bool immediate = false){
        GraphicObject newGraphic = new GraphicObject(this, filePath, graphicData, immediate);

        if(currentGraphic != null && !oldGraphics.Contains(currentGraphic))
            oldGraphics.Add(currentGraphic);

        currentGraphic = newGraphic;

        if(!immediate)
            return currentGraphic.FadeIn(transitionSpeed, blendingTexture);

        DestroyOldGraphics();
        return null;
    }

    public void DestroyOldGraphics(){
        foreach(GraphicObject g in oldGraphics)
            Object.Destroy(g.renderer.gameObject);

        oldGraphics.Clear();
    }

    public void Clear(float transitionSpeed = 1, Texture blendTex = null, bool immediate = false){
        if(currentGraphic != null){
            if(!immediate)
                currentGraphic.FadeOut(transitionSpeed, blendTex);
            else
                currentGraphic.Destroy();
        }
            

        foreach(GraphicObject g in oldGraphics)
            if(!immediate)
                g.FadeOut(transitionSpeed, blendTex);
            else
                g.Destroy();
    }
}
