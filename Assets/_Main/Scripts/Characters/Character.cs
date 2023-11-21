using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;

public abstract class Character
{
    public string name = "";
    public string displayName = "";
    public RectTransform root = null;
    public CharacterConfigData config;
    public Animator animator;

    protected CharacterManager manager => CharacterManager.instance;
    public DialogueSystem dialog => DialogueSystem.instance;

    protected Coroutine revealingCoroutine, hidingCorouting;
    protected Coroutine moving;
    public bool isRevealing => revealingCoroutine != null;
    public bool isHiding => hidingCorouting != null;
    public bool isMoving => moving != null;

    public virtual bool isVisible {get; set;}

    public Vector2 targetPosition {get; private set;}

    public Character(string name, CharacterConfigData config, GameObject prefab){
        this.name = name;
        displayName = name;
        this.config = config;

        if(prefab != null){
            GameObject obj = Object.Instantiate(prefab, manager.characterPanel);
            obj.name = manager.FormatCharacterPath(manager.characterPrefabNameFormat, name);
            obj.SetActive(true);
            root = obj.GetComponent<RectTransform>();
            animator = root.GetComponentInChildren<Animator>();
        }
    }

    public Coroutine Say(string dialogLine) => Say(new List<string>{dialogLine});

    public Coroutine Say(List<string> dialogLines){
        dialog.ShowSpeakerName(displayName);
        UpdateTextCustomizationsOnScreen();
        return dialog.Say(dialogLines);
    }

    public void SetNameColor(Color color) => config.nameColor = color;
    public void SetDialogColor(Color color) => config.dialogColor = color;
    public void SetNameFont(TMP_FontAsset font) => config.nameFont = font;
    public void SetDialogFont(TMP_FontAsset font) => config.dialogFont = font;

    public void ResetConfigurationsData() => config = CharacterManager.instance.GetCharacterConfig(name);

    public void UpdateTextCustomizationsOnScreen() => dialog.ApplySpeakerDataToDialogContainer(config);

    public virtual Coroutine Show(float speedMultiplier = 1f){
        if(isRevealing)
            return revealingCoroutine;

        if(isHiding)
            manager.StopCoroutine(hidingCorouting);

        revealingCoroutine = manager.StartCoroutine(ShowingOrHiding(true, speedMultiplier));

        return revealingCoroutine;
    }

    public virtual Coroutine Hide(float speedMultiplier = 1f){
        if(isHiding)
            return hidingCorouting;

        if(isRevealing)
            manager.StopCoroutine(revealingCoroutine);

        hidingCorouting = manager.StartCoroutine(ShowingOrHiding(false, speedMultiplier));
        
        return hidingCorouting;
    }


    public virtual IEnumerator ShowingOrHiding(bool show, float speedMultiplier = 1f){
        yield return null;
    }

    public virtual void SetPosition(Vector2 position){
        if(root == null)
            return;

        (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUITargetPositionRelativeCharacterAnchorTarget(position);

        root.anchorMin = minAnchorTarget;
        root.anchorMax = maxAnchorTarget;

        targetPosition = position;
    }

    public virtual Coroutine MoveToPosition(Vector2 position, float speed = 2, bool smooth = false){
        if(root == null)
            return null;

        if(isMoving)
            manager.StopCoroutine(moving);

        moving = manager.StartCoroutine(MovingToPosition(position, speed, smooth));

        targetPosition = position;
        
        return moving;
    }

    private IEnumerator MovingToPosition(Vector2 position, float speed, bool smooth){
        (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUITargetPositionRelativeCharacterAnchorTarget(position);
        Vector2 padding = root.anchorMax - root.anchorMin;

        while(root.anchorMin != minAnchorTarget || root.anchorMax != maxAnchorTarget){
            root.anchorMin = (smooth) ? 
                Vector2.Lerp(root.anchorMin, minAnchorTarget, speed * Time.deltaTime) :
                Vector2.MoveTowards(root.anchorMin, minAnchorTarget, speed * Time.deltaTime * 0.35f);

            root.anchorMax = root.anchorMin + padding;

            if(smooth && Vector2.Distance(root.anchorMin, minAnchorTarget) < 0.001f){
                root.anchorMin = minAnchorTarget;
                root.anchorMax = maxAnchorTarget;
                break;
            }

            yield return null;
        }

        moving = null;
    }

    public virtual void OnRecieveCastingExpression(string expression){
        Debug.Log("Received casting expression null");
        return;
    }

    protected(Vector2, Vector2) ConvertUITargetPositionRelativeCharacterAnchorTarget(Vector2 position){
        Vector2 padding = root.anchorMax - root.anchorMin;

        float maxX = 1f - padding.x;
        float maxY = 1f - padding.y;

        Vector2 minAnchorTarget = new Vector2(maxX * position.x, maxY * position.y);
        Vector2 maxAnchorTarget = minAnchorTarget + padding;

        return (minAnchorTarget, maxAnchorTarget);
    }

    public enum CharacterTypes{
        Text,
        Sprite
    }
}
