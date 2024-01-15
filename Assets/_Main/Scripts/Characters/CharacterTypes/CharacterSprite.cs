using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CharacterSprite : Character
{
    private const string SPRITE_RENDERER_PARENT_NAME = "Renderers";
    private CanvasGroup rootCG => root.GetComponent<CanvasGroup>();
    public Image spriteImage;
    public CanvasGroup rendererCG;
    private string artAssetsDirectory = "";
    private Coroutine transitionning = null, levelingAlpha = null;
    bool isTransitioningLayer => transitionning != null;
    bool isLeveling => levelingAlpha != null;
    private CanvasGroup oldRenderer = null;
    private const float defaultTransitionspeed = 3f;
    private float transitionSpeedMultiplier = 1f;
    public Vector2 position {get {return targetPosition;} set {targetPosition = value;} }

    public override bool isVisible { 
        get{return isRevealing || rootCG.alpha == 1;}
        set{rootCG.alpha = value ? 1 : 0;}
    }

    public CharacterSprite(string name, CharacterConfigData config, GameObject prefab, string rootAssetsFolder) : base(name, config, prefab){
        rootCG.alpha = 0f;
        artAssetsDirectory = rootAssetsFolder + "/Images";

        spriteImage = GetLayer();
    }

    public Image GetLayer(){
        Transform rendererRoot = animator.transform.Find(SPRITE_RENDERER_PARENT_NAME);

        if(rendererRoot == null)
            return null;

        Transform child = rendererRoot.transform.GetChild(0);
        Image rendererImage = child.GetComponent<Image>();
        rendererCG = rendererRoot.GetComponent<CanvasGroup>();

        if(rendererImage != null)
            return rendererImage;

        return null;
    }

    public void SetSprite(Sprite sprite){
        if(spriteImage == null){
            Debug.LogError($"SpriteImage not found for character '{name}'");
            return;
        }

        spriteImage.sprite = sprite;
    }

    public Sprite GetSprite(string spriteName){
        //Debug.Log($"Loading config '{config.name}'");
        if(config.sprites.Count > 0){
            /*StringBuilder sb = new StringBuilder();
            foreach(KeyValuePair<string, Sprite> pair in config.sprites) {
                sb.AppendLine($"Key : '{pair.Key}' -> Type : {pair.Key.GetType()}");
                sb.AppendLine($"Value : '{pair.Value.name}' -> Type : {pair.Value.GetType()}");
                sb.AppendLine("---");
            }
            Debug.Log(sb.ToString());*/

            if(config.sprites.TryGetValue(spriteName, out Sprite sprite))
                return sprite;
        }

        Debug.Log("Sprite '" + spriteName + "' not found in dictionary '" + config.name + "'");
        //return null;

        return Resources.Load<Sprite>($"{artAssetsDirectory}/{spriteName}");  
    }

    public Coroutine TransitionSprite(Sprite sprite, float speed = 1){
        Image image = GetLayer();
        if(sprite == image.sprite){
            Debug.Log("Sprite is the same");
            return null;
        }

        if(isTransitioningLayer){
            Debug.Log("<color=red>Stop transitionning</color>");
            manager.StopCoroutine(transitionning);
        }

        transitionning = manager.StartCoroutine(TransitionningSprite(sprite, speed));

        return transitionning;
    }

    private IEnumerator TransitionningSprite(Sprite sprite, float speed){
        transitionSpeedMultiplier = speed;
        CanvasGroup newRenderer = CreateNewRenderer(spriteImage.transform.parent);
        Debug.Log("Created a new renderer");
        Image newImage = newRenderer.transform.GetChild(0).GetComponent<Image>();
        newImage.sprite = sprite;

        yield return TryStartLevelingAlphas();

        transitionning = null;
    }

    private Coroutine TryStartLevelingAlphas(){
        //Debug.Log("Entered TrySartLevelingAlphasCoroutine");
        if(isLeveling){
            //Debug.Log("<color=red>Was leveling. Aborted</color>");
            return levelingAlpha; 
        }

        levelingAlpha = manager.StartCoroutine(RunAlphaLeveling());

        return levelingAlpha;
    }

    private IEnumerator RunAlphaLeveling(){
        //Debug.Log("Started leveling alphas");
        while(rendererCG.alpha < 1 || oldRenderer.alpha > 0){
            float speed = defaultTransitionspeed * transitionSpeedMultiplier * Time.deltaTime;

            rendererCG.alpha = Mathf.MoveTowards(rendererCG.alpha, 1, speed);
            //Debug.Log($"RendererCG alpha is now equal to {rendererCG.alpha}");

            oldRenderer.alpha = Mathf.MoveTowards(oldRenderer.alpha, 0, speed);
            //Debug.Log($"OldRenderer alpha is no equal to {oldRenderer.alpha}");

            if(oldRenderer.alpha <= 0){
                //Debug.Log("<color=red>Old renderer is less or equal 0</color>");
                //Debug.Log($"Should destroy '{oldRenderer.name}'");
                Object.Destroy(oldRenderer.gameObject);
                break;
            }

            yield return null;
        }

        //Debug.Log("End of transition");
        levelingAlpha = null;
        transitionning = null;
    }

    private CanvasGroup CreateNewRenderer(Transform parent){
        CanvasGroup newRenderer = Object.Instantiate(spriteImage.transform.parent.transform, parent.parent).GetComponent<CanvasGroup>();
        Image newImage = newRenderer.GetComponentInChildren<Image>();
        oldRenderer = rendererCG;
        rendererCG = newRenderer;

        newImage.name = spriteImage.name;
        newRenderer.name = SPRITE_RENDERER_PARENT_NAME;
        spriteImage = newImage;
        spriteImage.gameObject.SetActive(true);
        rendererCG.alpha = 0;

        return newRenderer;
    }

    public override IEnumerator ShowingOrHiding(bool show, float speedMultiplier = 1f)
    {
        float targetAlpha = show ? 1f : 0f;
        CanvasGroup self = rootCG;

        while(self.alpha != targetAlpha){
            self.alpha = Mathf.MoveTowards(self.alpha, targetAlpha, 3f * Time.deltaTime * speedMultiplier);
            yield return null;
        }

        revealingCoroutine = null;
        hidingCorouting = null;
    }

    public override void OnRecieveCastingExpression(string expression)
    {

        Sprite sprite = GetSprite(expression);
        if(sprite == null){
            Debug.LogError($"Sprite '{expression}' can't be found for character '{name}'");
            return;
        }

        TransitionSprite(sprite);
    }
}
