using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoolMotor : MiniGame
{
    RectTransform rect => transform.GetChild(0).GetComponent<RectTransform>();
    [SerializeField] private float requiredPressTime;
    private float currentPressTime;

    [HideInInspector] public bool buttonPressed;

    public float sliderValue;
    public Image fillImage;
    public AnimationCurve shakeCurveOverTime;
    public float speed;

    private Vector2 originalPos;
    
    private void Start() {
        originalPos = rect.localPosition;
    }

    private void Update() {
        Shake();
        sliderValue = currentPressTime / requiredPressTime;
        fillImage.fillAmount = sliderValue;

        if(buttonPressed){
            if(currentPressTime < requiredPressTime){
                currentPressTime += Time.deltaTime;
            }
        } else {
            if(currentPressTime > 0){
                currentPressTime -= Time.deltaTime;
            }
        }
        

        if(currentPressTime >= requiredPressTime)
            SolveProblem();
    }

    private void Shake() {
        float amount = shakeCurveOverTime.Evaluate(currentPressTime);
        Vector2 randomOffset = Random.insideUnitSphere * amount;
        rect.localPosition = originalPos + randomOffset;
    }
}
