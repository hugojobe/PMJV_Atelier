using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovingConnectorUnspeed : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform rectTransform => GetComponent<RectTransform>();
    private DecoReco reco => GetComponentInParent<DecoReco>();

    public Vector2 hiddenPos;
    public Vector2 idlePos;
    public Vector2 connectedPosition;

    private Vector2 offset;

    public bool interactible;

    public bool connected;

    public enum ConnectorType {starting, replace}
    public ConnectorType type;

    private void Start() {
        if(type == ConnectorType.replace) {
            rectTransform.anchoredPosition = hiddenPos;
            interactible = false;
        }
    }

    public void Show() {
        StartCoroutine(ShowCoroutine());
    }

    public void Hide() {
        StartCoroutine(HideCoroutine());
    }

    public void OnBeginDrag(PointerEventData eventData) {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out offset);
    }
    
    public void OnDrag(PointerEventData eventData) {
        if(interactible){
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPointerPos);
            Vector2 clampedPosition = localPointerPos - offset;

            rectTransform.localPosition = clampedPosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        if(type == ConnectorType.starting) {
            if(!connected)
                //Debug.Log("Trigger switch");
                Hide();
                reco.toReplace.Show();
        }
    }

    private IEnumerator HideCoroutine() {
        //Debug.Log("Should Move to hide");
        while((Vector2)rectTransform.localPosition != hiddenPos) {
            //Debug.Log("Moving to hide. Position : " + rectTransform.localPosition.x);
            rectTransform.localPosition = Vector2.MoveTowards(rectTransform.localPosition, hiddenPos, 3500f * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator ShowCoroutine() {
        while((Vector2)rectTransform.localPosition != idlePos) {
            rectTransform.localPosition = Vector2.MoveTowards(rectTransform.localPosition, idlePos, 3500f * Time.deltaTime);
            yield return null;
        }

        if(type == ConnectorType.replace) {
            interactible = true;
        }
    }

    public void ForceEndDrag() {
        ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
    }
}
