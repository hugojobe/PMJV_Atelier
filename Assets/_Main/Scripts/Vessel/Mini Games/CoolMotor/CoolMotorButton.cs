using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CoolMotorButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    CoolMotor motor => GetComponentInParent<CoolMotor>();

    public void OnPointerDown(PointerEventData eventData) {
        motor.buttonPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        motor.buttonPressed = false;
    }
}
