using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CoolMotorButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    CoolMotor motor => GetComponentInParent<CoolMotor>();
    AudioSource sfx;

    public void OnPointerDown(PointerEventData eventData) {
        sfx = AudioManager.instance.PlaySoundEffect("Cooling", 0.7f);
        motor.buttonPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        sfx.volume = 0f;
        motor.buttonPressed = false;
    }
}
