using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    private RectTransform rectTransform => GetComponent<RectTransform>();

    public Vector2 targetPos;
    public float speed;

    private void Update() {
        rectTransform.localPosition = Vector2.MoveTowards(rectTransform.localPosition, targetPos, speed * Time.deltaTime);
    }
}
