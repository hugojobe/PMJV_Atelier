using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChoicePanel : MonoBehaviour
{
    public static ChoicePanel instance;

    private const float BUTTON_MIN_WIDTH = 50;
    private const float BUTTON_MAX_SIZE = 750;
    private const float BUTTON_WIDTH_PADDING = 25;
    private const float BUTTON_HEIGHT_PER_LINE = 60f;

    public CanvasGroup cg;
    private List<Choicebutton> buttons = new List<Choicebutton>();
    public TextMeshProUGUI titleText;
    public GameObject choiceButtonPrefab;
    public VerticalLayoutGroup buttonLayoutGroup;

    public ChoicePanelDecision lastDecision;

    private Coroutine showingCoroutine = null;
    private Coroutine hidingCoroutine = null;
    private bool isHiding => hidingCoroutine != null;
    private bool isShowing => showingCoroutine != null;
    private bool isFading => isShowing || isHiding;

    private bool isVisible => showingCoroutine != null || cg.alpha > 0;
    private float alpha {get {return cg.alpha;} set {cg.alpha = value;}}

    public bool isWaitingOnUserChoice = false;

    private void Awake() {
        if(instance == null) instance = this;
    }

    private void Start() {
        alpha = 0;
        SetInteractibleState(false);
    }

    private void SetInteractibleState(bool active){
        cg.interactable = active;
        cg.blocksRaycasts = active;
    }

    public void Show(string question, string[] choices){
        lastDecision = new ChoicePanelDecision(question, choices);

        isWaitingOnUserChoice = true;

        ShowPanel();
        SetInteractibleState(true);

        titleText.text = question;
        StartCoroutine(GenerateChoices(choices));
    }

    private IEnumerator GenerateChoices(string[] choices){
        for(int i = 0; i < choices.Length; i++){
            Choicebutton choicebutton;
            if(i < buttons.Count){
                choicebutton = buttons[i];
            } else {
                GameObject newButtonObject = Instantiate(choiceButtonPrefab, buttonLayoutGroup.transform);
                newButtonObject.SetActive(true);

                Button newButton = newButtonObject.GetComponent<Button>();
                TextMeshProUGUI newTitle = newButton.GetComponentInChildren<TextMeshProUGUI>();
                LayoutElement newLayout = newButton.GetComponent<LayoutElement>();

                choicebutton = new Choicebutton{button = newButton, layout = newLayout, title = newTitle};

                buttons.Add(choicebutton);
            }

            choicebutton.button.onClick.RemoveAllListeners();
            int buttonIndex = i;
            choicebutton.button.onClick.AddListener(() => AcceptAnswer(buttonIndex));
            choicebutton.title.text = choices[i];

            float buttonWidth = Mathf.Clamp(BUTTON_WIDTH_PADDING + choicebutton.title.preferredWidth, BUTTON_MIN_WIDTH, BUTTON_MAX_SIZE);
            choicebutton.layout.preferredWidth = buttonWidth;
        }

        for (int i = 0; i < buttons.Count; i++){
            bool show = i < choices.Length;  
            buttons[i].button.gameObject.SetActive(show);
        }

        yield return new WaitForEndOfFrame();

        foreach(var button in buttons){
            int lines = button.title.textInfo.lineCount;
            button.layout.preferredHeight = BUTTON_HEIGHT_PER_LINE + (lines*25);
        }
    }

    public void Hide(){
        HidePanel();
        SetInteractibleState(false);
    }

    private void AcceptAnswer(int index){
        if(index < 0 || index > lastDecision.choices.Length - 1) return;

        lastDecision.answerIndex = index;
        isWaitingOnUserChoice = false;
        Hide();
    }

    public class ChoicePanelDecision{
        public string question = string.Empty;
        public int answerIndex = -1;
        public string[] choices = new string[0];

        public ChoicePanelDecision(string question, string[] choices){
            this.question = question;
            this.choices = choices;
            answerIndex = -1;
        }
    }

    private struct Choicebutton{
        public Button button;
        public TextMeshProUGUI title;
        public LayoutElement layout;
    }

    public Coroutine ShowPanel(){
        if(isShowing) return showingCoroutine;

        if(isHiding){
            DialogueSystem.instance.StopCoroutine(hidingCoroutine);
            hidingCoroutine = null;
        }

        showingCoroutine = DialogueSystem.instance.StartCoroutine(FadingPanel(1f));

        return showingCoroutine;
    }

    public Coroutine HidePanel(){
        if(isHiding) return hidingCoroutine;

        if(isShowing){
            DialogueSystem.instance.StopCoroutine(showingCoroutine);
            showingCoroutine = null;
        }

        hidingCoroutine = DialogueSystem.instance.StartCoroutine(FadingPanel(0f));

        return hidingCoroutine;
    }

    private IEnumerator FadingPanel(float alpha){

        while(cg.alpha != alpha){
            cg.alpha = Mathf.MoveTowards(cg.alpha, alpha, Time.deltaTime * 4f);
            yield return null;
        }

        showingCoroutine = null;
        hidingCoroutine = null;
    }
}
