using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogStarter : MonoBehaviour, IPointerDownHandler
{
    public TextAsset fileToStart;

    public void OnClick() {
        FindObjectOfType<TestDialogFiles>().fileToRead = fileToStart;
        FindObjectOfType<TestDialogFiles>().StartConversation();
    }

    public void OnPointerDown(PointerEventData eventData) {
        OnClick();
    }
}
