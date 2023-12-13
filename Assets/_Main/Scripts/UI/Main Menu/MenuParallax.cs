using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    Vector3 startPosition;
    public float strength;
    public Vector2 parallaxClamp;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        Vector2 mouPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        // Correction pour la position de départ en bas à gauche
        mouPos -= new Vector2(0.5f, 0.5f);

        float posX = Mathf.Lerp(transform.localPosition.x, startPosition.x + (mouPos.x * -strength), 5f * Time.deltaTime);
        float posY = Mathf.Lerp(transform.localPosition.y, startPosition.y + (mouPos.y * -strength), 5f * Time.deltaTime);

        posX = Mathf.Clamp(posX, startPosition.x - parallaxClamp.x, startPosition.x + parallaxClamp.x);
        posY = Mathf.Clamp(posY, startPosition.y - parallaxClamp.y, startPosition.y + parallaxClamp.y);

        transform.localPosition = new Vector3(posX, posY, transform.localPosition.z);
    }
}