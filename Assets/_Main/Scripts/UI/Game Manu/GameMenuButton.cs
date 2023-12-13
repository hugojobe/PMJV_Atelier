using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    Image image => GetComponent<Image>();

    public Color baseColor;
    public Color hoverColor;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
        image.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData) {
        image.color = baseColor;
    }
}
