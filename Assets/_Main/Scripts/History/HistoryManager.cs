using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryManager : MonoBehaviour
{
    public const int HISTORY_CACHE_LIMIT = 100;

    public static HistoryManager instance;
    public List<HistoryState> history = new List<HistoryState>();

    private HistoryNavigation navigation;
    public HistoryLogManager logManager;

    private void Awake() {
        instance = this;
        navigation = GetComponent<HistoryNavigation>();
        logManager = GetComponent<HistoryLogManager>();
    }

    private void Start() {
        DialogueSystem.instance.onClear += LogCurrentState;
    }

    public void LogCurrentState() {
        HistoryState state = HistoryState.Capture();
        history.Add(state);
        logManager.AddLog(state);

        if(history.Count > HISTORY_CACHE_LIMIT)
            history.RemoveAt(0);
    }

    public void LoadState(HistoryState state) {
        state.Load();
    }

    public void GoForward() => navigation.GoForward();
    public void GoBack() => navigation.GoBack();
}
