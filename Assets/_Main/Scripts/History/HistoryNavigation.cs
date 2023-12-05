using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HistoryNavigation : MonoBehaviour
{
    public int progress = 0;

    [SerializeField] private TextMeshProUGUI statusText;

    HistoryManager manager => HistoryManager.instance;
    List<HistoryState> history => manager.history;
    HistoryState cachedState = null;
    public bool isOnCachedState = false;

    public bool isViewingHistory = false;

    public bool canNavigate => !DialogueSystem.instance.conversationManager.isOnLogicalLine;

    public void GoForward() {
        if(!isViewingHistory || !canNavigate)
            return;

        HistoryState state = null;

        if(progress < history.Count - 1) {
            progress++;
            state = history[progress];
        } else {
            isOnCachedState = true;
            state = cachedState;
        }

        state.Load();

        if(isOnCachedState) {
            isViewingHistory = false;
            DialogueSystem.instance.onUserPromptNext -= GoForward;
            statusText.text = "";
            DialogueSystem.instance.OnStopViewingHistry();
        } else
            UpdateStatusText();

    }

    public void GoBack() {
        if(history.Count == 0 || (progress == 0 && isViewingHistory) || !canNavigate)
            return;

        progress = isViewingHistory? progress - 1 : history.Count - 1;

        if(!isViewingHistory) {
            isViewingHistory = true;
            isOnCachedState = false;
            cachedState = HistoryState.Capture();

            DialogueSystem.instance.onUserPromptNext += GoForward;
            DialogueSystem.instance.OnStartViewingHistory();
        }

        HistoryState state = history[progress];
        state.Load();
        UpdateStatusText();
    }

    private void UpdateStatusText() {
        statusText.text = $"Viewing history : {history.Count - progress}/{history.Count}";
    }
}
