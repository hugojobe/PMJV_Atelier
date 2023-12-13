using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonGlitch : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    TextMeshProUGUI buttonText => GetComponentInChildren<TextMeshProUGUI>();
    public GameObject buttonSelector;
    public AudioClip buttonGlitchSound;

    public void OnPointerEnter(PointerEventData eventData) {
        StartCoroutine(GlitchEffectCoroutine());
        buttonSelector.SetActive(true);
        AudioManager.instance.PlaySoundEffect(buttonGlitchSound, 0.1f);
    }

    public void OnPointerExit(PointerEventData eventData) {
        buttonSelector.SetActive(false);
    }

    private IEnumerator GlitchEffectCoroutine() {
        string baseText = buttonText.text;
        buttonText.text = "<shake a=1.5 d=0.3><slide a=5 f=10>" + baseText;

        yield return new WaitForSecondsRealtime(0.1f);

        buttonText.text = baseText +" "; 
    }
}
