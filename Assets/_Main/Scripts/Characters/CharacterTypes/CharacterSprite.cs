using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
        if(config.sprites.Count > 0){
            if(config.sprites.TryGetValue(spriteName, out Sprite sprite))
                return sprite;
        }

        return Resources.Load<Sprite>($"{artAssetsDirectory}/{spriteName}");  
    }

    public Coroutine TransitionSprite(Sprite sprite, float speed = 1){
        Image image = GetLayer();
        if(sprite == image.sprite){
            Debug.Log("Sprite is the same");
            return null;
        }

        if(isTransitioningLayer)
            manager.StopCoroutine(transitionning);

        transitionning = manager.StartCoroutine(TransitionningSprite(sprite, speed));

        return transitionning;
    }

    private IEnumerator TransitionningSprite(Sprite sprite, float speed){
        transitionSpeedMultiplier = speed;
        CanvasGroup newRenderer = CreateNewRenderer(spriteImage.transform.parent);
        Image newImage = newRenderer.transform.GetChild(0).GetComponent<Image>();
        newImage.sprite = sprite;

        yield return TryStartLevelingAlphas();

        transitionning = null;
    }

    private Coroutine TryStartLevelingAlphas(){
        if(isLeveling)
            return levelingAlpha;

        levelingAlpha = manager.StartCoroutine(RunAlphaLeveling());

        return levelingAlpha;
    }

    private IEnumerator RunAlphaLeveling(){
        while(rendererCG.alpha < 1 || oldRenderer.alpha > 0){
            float speed = defaultTransitionspeed * transitionSpeedMultiplier * Time.deltaTime;

            rendererCG.alpha = Mathf.MoveTowards(rendererCG.alpha, 1, speed);

            oldRenderer.alpha = Mathf.MoveTowards(oldRenderer.alpha, 0, speed);

            if(oldRenderer.alpha <= 0){
                Object.Destroy(oldRenderer.gameObject);
                break;
            }

            yield return null;
        }

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
