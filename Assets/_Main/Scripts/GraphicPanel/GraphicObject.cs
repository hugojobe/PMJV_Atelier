using System.Collections;
using System.Runtime.InteropServices;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GraphicObject
{
    private const string NAME_FORMAT = "Graphic - [{0}]";
    private const string MATERIAL_PATH = "Materials/layerTransitionMaterial";
    private const string MATERIAL_COLOR =    "_Color";
    private const string MATERIAL_MAINTEX =  "_MainTex";
    private const string MATERIAL_BLENDTEX = "_BlendTex";
    private const string MATERIAL_BLEND =    "_Blend";
    private const string MATERIAL_ALPHA =    "_Alpha";
    public RawImage renderer;
    public string graphicPath = "";
    public string graphicName;

    private GraphicLayer layer;

    public bool isVideo => video != null;
    public bool useAudio => (audio != null ? !audio.mute : false);
    public VideoPlayer video = null;
    public AudioSource audio = null;


    private Coroutine fadingInCoroutine = null;
    private Coroutine fadingOutCoroutine = null;

    public GraphicObject(GraphicLayer layer, string graphicPath, Texture tex, bool immediate){
        this.graphicPath = graphicPath;
        this.layer = layer;

        GameObject obj = new GameObject();
        obj.transform.SetParent(layer.panel);
        renderer = obj.AddComponent<RawImage>();

        graphicName = tex.name;

        InitGraphic(immediate);

        renderer.name = string.Format(NAME_FORMAT, graphicName);
        renderer.material.SetTexture(MATERIAL_MAINTEX, tex);
    }

    public GraphicObject(GraphicLayer layer, string graphicPath, VideoClip clip, bool useAudio, bool immediate){
        this.graphicPath = graphicPath;
        this.layer = layer;

        GameObject obj = new GameObject();
        obj.transform.SetParent(layer.panel);
        renderer = obj.AddComponent<RawImage>();

        graphicName = clip.name;
        renderer.name = string.Format(NAME_FORMAT, clip.name);

        InitGraphic(immediate);

        RenderTexture tex = new RenderTexture(Mathf.RoundToInt(clip.width), Mathf.RoundToInt(clip.height), 0);
        renderer.material.SetTexture (MATERIAL_MAINTEX, tex);

        video = renderer.gameObject.AddComponent<VideoPlayer>();
        video.playOnAwake = true;
        video.source = VideoSource.VideoClip;
        video.clip = clip;
        video.renderMode = VideoRenderMode.RenderTexture;
        video.targetTexture = tex;
        video.isLooping = true;

        video.audioOutputMode = VideoAudioOutputMode.AudioSource;
        audio = video.gameObject.AddComponent<AudioSource>();

        audio.volume = immediate ? 1 : 0;
        if(!useAudio)
            audio.mute = true;

        video.SetTargetAudioSource(0, audio);

        video.frame = 0;
        video.Prepare();
        video.Play();

        video.enabled = false;
        video.enabled = true;
    }

    private void InitGraphic(bool immediate){
        renderer.transform.localPosition = Vector3.zero;
        renderer.transform.localScale = Vector3.one;

        RectTransform rect = renderer.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.one;

        renderer.material = GetTransitionMaterial();

        float startingOpacity = immediate ? 1f : 0f;
        renderer.material.SetFloat(MATERIAL_BLEND, startingOpacity);
        renderer.material.SetFloat(MATERIAL_ALPHA, startingOpacity);
    }

    private Material GetTransitionMaterial(){
        Material mat = Resources.Load<Material>(MATERIAL_PATH);
        if(mat != null)
            return new Material(mat);
        else
            return null;
    }

    GraphicPanelManager panelManager => GraphicPanelManager.instance;
    public Coroutine FadeIn(float speed = 1f, Texture blend = null){
        if(fadingOutCoroutine != null){
            panelManager.StopCoroutine(fadingOutCoroutine);
        }

        if(fadingInCoroutine != null){
            return fadingInCoroutine;
        }

        fadingInCoroutine = panelManager.StartCoroutine(Fading(1, speed, blend));

        return fadingInCoroutine;
    }

    public Coroutine FadeOut(float speed = 1f, Texture blend = null){
        if(fadingInCoroutine != null){
            panelManager.StopCoroutine(fadingOutCoroutine);
        }

        if(fadingOutCoroutine != null){
            return fadingInCoroutine;
        }

        fadingOutCoroutine = panelManager.StartCoroutine(Fading(0, speed, blend));

        return fadingInCoroutine;
    }

    public IEnumerator Fading(float target, float speed, Texture blend = null){
        bool isBlending = blend != null;
        bool fadingIn = target > 0;

        renderer.material.SetTexture(MATERIAL_BLENDTEX, blend);
        renderer.material.SetFloat(MATERIAL_ALPHA, isBlending ? 1 : (fadingIn ? 0 : 1));
        renderer.material.SetFloat(MATERIAL_BLEND, isBlending ? (fadingIn ? 0 : 1) : 1);

        string opacityParam = isBlending ? MATERIAL_BLEND : MATERIAL_ALPHA;

        while(renderer.material.GetFloat(opacityParam) != target){
            float opacity = Mathf.MoveTowards(renderer.material.GetFloat(opacityParam), target, speed * Time.deltaTime);
            renderer.material.SetFloat(opacityParam, opacity);

            if(isVideo)
                audio.volume = opacity;

            yield return null;
        }

        fadingInCoroutine = null;
        fadingOutCoroutine = null;

        if(target == 0){
            Destroy();
        } else {
            DestroyBackgroundGraphicsOnlayer();
        }
    }

    public void Destroy(){
        if(layer.currentGraphic == null && layer.currentGraphic.renderer == renderer)
            layer.currentGraphic = null;

        Object.Destroy(renderer.gameObject);
    }

    private void DestroyBackgroundGraphicsOnlayer(){
        layer.DestroyOldGraphics();
    }
}
