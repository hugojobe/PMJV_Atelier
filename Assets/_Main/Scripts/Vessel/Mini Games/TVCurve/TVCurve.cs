using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TVCurve : MiniGame
{
    public RawImage playerCurveImage;
    public RawImage tvCurve;
    public float tvAngle;
    public Slider playerSlider;

    public float timeSinceLastTouched;
    public float timeRequiredWithoutValueChange;
    public float lastSliderValue;
    public bool playedSFX;

    private void Start() {
        base.Start();

        tvAngle = Random.Range(0, 126);
        tvCurve.uvRect = new Rect(tvAngle*2 / 360, 0f, 1f, 1f);

        playerSlider.value = Random.Range(0, 126);
    }

    private void Update() {
        playerCurveImage.uvRect = new Rect(playerSlider.value*2 / 360f, 0f, 1f, 1f);

        if(playerSlider.value != lastSliderValue) {
            lastSliderValue = playerSlider.value;
            timeSinceLastTouched = 0f;
        } else {
            timeSinceLastTouched += Time.deltaTime;

            if(timeSinceLastTouched > timeRequiredWithoutValueChange && playerSlider.interactable) {
                timeSinceLastTouched = 0f;
                CheckCorrespondance(playerSlider.value*2);
            }
        }
    }

    public void CheckCorrespondance(float value) {
        if(value == tvAngle*2){
            playerSlider.interactable = false;
            if(!playedSFX){
                AudioManager.instance.PlaySoundEffect("TV", 0.5f);
                playedSFX = true;
            }
            SolveProblem();
        }
    }
}
