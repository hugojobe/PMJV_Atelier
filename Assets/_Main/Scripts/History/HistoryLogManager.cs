using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HistoryLogManager : MonoBehaviour
{
    private const float LOG_STARTING_HEIGTH = 2f;
    private const float LOG_HEIGHT_PER_LINE = 2f;
    private const float DEFAULT_HEIGHT = 1f;
    private const float TEXT_DEFAULT_SCALE = 1f;

    private const string NAME_TEXT_NAME = "Speaker Name";
    private const string DIALOG_TEXT_NAME = "Dialog Text";

    private float logScaling = 1f;

    [SerializeField] private Animator anim;
    [SerializeField] private GameObject logPrefab;

    HistoryManager manager => HistoryManager.instance;
    private List<HistoryLog> logs = new List<HistoryLog>();

    public bool isOpen {get; private set; } = false;

    [SerializeField] private Slider logScaleSlider;

    private float textScaling => logScaling * 3f;

    public void Open() {
        if(isOpen) return;

        anim.Play("Open");

        isOpen = true;
    }

    public void Close() {
        if(!isOpen) return;

        anim.Play("Close");

        isOpen = false;
    }

    public void AddLog(HistoryState state) {
        if(logs.Count >= HistoryManager.HISTORY_CACHE_LIMIT) {
            DestroyImmediate(logs[0].container);
            logs.RemoveAt(0);
        }

        CreateLog(state);
    }

    private void CreateLog(HistoryState state) {
        HistoryLog log = new HistoryLog();

        log.container = Instantiate(logPrefab, logPrefab.transform.parent);
        log.container.SetActive(true);

        log.nameText = log.container.transform.Find(NAME_TEXT_NAME).GetComponent<TextMeshProUGUI>();
        log.dialogText = log.container.transform.Find(DIALOG_TEXT_NAME).GetComponent<TextMeshProUGUI>();

        if(state.dialogue.currentSpeaker == string.Empty){
            log.nameText.text = string.Empty;
        } else {
            log.nameText.text = state.dialogue.currentSpeaker;
            log.nameText.font = HistoryCache.LoadFont(state.dialogue.speakerFont);
            log.nameText.color = state.dialogue.speakerColor;
            log.nameFontSize = TEXT_DEFAULT_SCALE * state.dialogue.speakerScale;
            log.nameText.fontSize = log.nameFontSize + textScaling;
        }

        log.dialogText.text = state.dialogue.currentDialog;
        log.dialogText.font = HistoryCache.LoadFont(state.dialogue.dialogFont);
        log.dialogText.color = state.dialogue.dialogColor;
        log.dialogFontSize = TEXT_DEFAULT_SCALE * state.dialogue.dialogScale;
        log.dialogText.fontSize = log.dialogFontSize + textScaling;
        
        FitLogToText(log);

        logs.Add(log);
    }

    private void FitLogToText(HistoryLog log) {
        RectTransform rect = log.dialogText.GetComponent<RectTransform>();
        ContentSizeFitter textCSF = log.dialogText.GetComponent<ContentSizeFitter>();

        textCSF.SetLayoutVertical();

        LayoutElement logLayout = log.container.GetComponent<LayoutElement>();
        float height = rect.rect.height;

        float perc = height / DEFAULT_HEIGHT;
        float extraScale = (LOG_HEIGHT_PER_LINE * perc) - LOG_HEIGHT_PER_LINE;
        float scale = LOG_STARTING_HEIGTH + extraScale;
        
        logLayout.preferredHeight = scale + textScaling;

        logLayout.preferredHeight += 2f * logScaling;
    }

    public void SetLogScaling() {
        logScaling = logScaleSlider.value;

        foreach(var log in logs) {
            log.nameText.fontSize = log.nameFontSize + textScaling;
            log.dialogText.fontSize = log.dialogFontSize + textScaling;

            FitLogToText(log);
        }
    }

    public void Clear() {
        for(int i = 0; i < logs.Count; i++) 
            DestroyImmediate(logs[i].container); 

        logs.Clear();
    }

    public void Rebuild() {
        foreach(var state in manager.history) {
            CreateLog(state);
        }       
    }
}