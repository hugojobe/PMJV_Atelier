using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovableConnector : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform => GetComponent<RectTransform>();
    private Vector2 offset;

    private Vector2 startPos;

    public bool canDrag;

    private Vector2 lastFramePosition;

    public int captureInterval;
    private int currentInterval;
    private float[] capturedSpeed;

    private PointerEventData currentPointerEventData;

    private void Start() {
        capturedSpeed = new float[captureInterval];

        startPos = rectTransform.localPosition;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out offset);
    }

    public void OnDrag(PointerEventData eventData) {
        currentPointerEventData = eventData;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPointerPos);
        Vector2 clampedPosition = new Vector2((localPointerPos - offset).x, rectTransform.localPosition.y);

        lastFramePosition = rectTransform.localPosition;

        rectTransform.localPosition = clampedPosition;

        CalculateMovingSpeed();
    }

    public void OnEndDrag(PointerEventData eventData) {

    }

    public void RestartDrag() {
        ExecuteEvents.Execute(gameObject, currentPointerEventData, ExecuteEvents.endDragHandler);
        rectTransform.localPosition = startPos;
        ExecuteEvents.Execute(gameObject, currentPointerEventData, ExecuteEvents.beginDragHandler);
    }

    public float CalculateMovingSpeed() {
        float delta = Vector2.Distance(lastFramePosition, rectTransform.localPosition);

        capturedSpeed[currentInterval] = delta;

        currentInterval = (currentInterval + 1) % captureInterval;

        return delta;
    }

    public float SpeedAverage() {
        float totalSpeed = 0;

        foreach(float speed in capturedSpeed) {
            totalSpeed += speed;
        }

        return totalSpeed / captureInterval;
    }
}
