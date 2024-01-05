using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Leaf : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	private RectTransform rectTransform => GetComponent<RectTransform>();

	private Vector2 targetPosition;
    public float smoothSpeed = 5f;

    private void Start() {
        targetPosition = rectTransform.anchoredPosition;
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {

	}

	public void OnDrag(PointerEventData eventData) {
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out targetPosition);
	}

	public void OnEndDrag(PointerEventData eventData) {

	}

    private void Update() {
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, smoothSpeed * Time.deltaTime);
    }

}
