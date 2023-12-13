using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Throttle : MiniGame
{
    public Image instructionImage;
    public Sprite instructionUp, instructionDown;
    public Color upColor, downColor;
    public Slider throttleslider;
    public float targetValue;
    public float lastSliderValue;
    public float timeSinceLastTouched;
    public float timeRequiredWithoutValueChange;


    private void Start() {
        targetValue = Random.Range(0, 51);
        do {
            throttleslider.value = Random.Range(0, 51);
        }
        while(throttleslider.value == targetValue);
    }

    private void Update() {
        if(throttleslider.value < targetValue) {
            instructionImage.sprite = instructionUp;
            instructionImage.color = upColor;
        }

        if(throttleslider.value > targetValue) {
            instructionImage.sprite = instructionDown;
            instructionImage.color = downColor;
        }

        if(throttleslider.value == targetValue) {
            instructionImage.sprite = null;
            instructionImage.color = new Color(0, 0, 0 ,0);
        }

        if(throttleslider.value != lastSliderValue) {
            lastSliderValue = throttleslider.value;
            timeSinceLastTouched = 0f;
        } else {
            timeSinceLastTouched += Time.deltaTime;

            if(timeSinceLastTouched > timeRequiredWithoutValueChange && throttleslider.interactable) {
                timeSinceLastTouched = 0f;

                if(throttleslider.value == targetValue){
                    throttleslider.interactable = false;
                    SolveProblem();
                }
            }
        }
    }
}
