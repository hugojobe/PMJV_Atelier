using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class KeyTurnRing : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private KeyTurn keyTurn => GetComponentInParent<KeyTurn>();
    private RectTransform rectTtransform => GetComponent<RectTransform>();

    public float rotationAngle;
    public bool isRotating = false;

    // Set the angle range within which the button can rotate
    public float minRotationAngle = -45f;
    public float maxRotationAngle = 45f;
    public bool canPlay;

    private Vector2 mousePos;

    private float startOffset = 0f;

    private float lastFrameAngleValue;

    private void Start() {
    }

    private void Update() {
        CheckTouched();
    
        if (isRotating){
            /*Vector2 direction = mousePos - (Vector2)rectTtransform.anchoredPosition;
            float angle = Vector2.SignedAngle(Vector2.up, direction);
            Debug.Log(angle - 180);

            Debug.DrawLine((Vector2)rectTtransform.anchoredPosition, (Vector2)rectTtransform.anchoredPosition + Vector2.up * 3f);
            Debug.DrawLine((Vector2)rectTtransform.anchoredPosition, (Vector2)rectTtransform.anchoredPosition + direction);

            rotationAngle = Mathf.Clamp(angle - 180f, minRotationAngle, maxRotationAngle);

            rectTtransform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);*/
        }

        CheckLastFrameTouched();
    }

    private void CheckTouched() {
        lastFrameAngleValue = rotationAngle;
    }

    private void CheckLastFrameTouched() {
        keyTurn.notTouched = (lastFrameAngleValue == rotationAngle);
    }

    public void OnPointerDown(PointerEventData eventData) {
        isRotating = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        isRotating = false;
    }

    public void OnDrag(PointerEventData eventData) {
        
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Camera.main.ScreenToWorldPoint(Input.mousePosition), 5f);

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 localPos = rectTtransform.InverseTransformPoint(worldPos + (transform.parent.transform as RectTransform).anchoredPosition + (Vector2)GetComponentInParent<Canvas>().transform.position);

        Debug.Log(localPos);
    }
}
