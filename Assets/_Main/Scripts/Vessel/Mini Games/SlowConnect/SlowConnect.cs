using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlowConnect : MiniGame
{
    private MovableConnector connector => GetComponentInChildren<MovableConnector>();

    public float maxMovingSpeed;

    public Slider speedSlider;

    private Coroutine noteCoroutine = null;
    public GameObject overspeedNoteObject;

    private void Start() {
        base.Start();

        overspeedNoteObject.SetActive(false);
    }

    private void Update() {
        UpdateSpeedSlider();
        CheckOverspeed();
    }

    private void UpdateSpeedSlider() {
        float currentSpeed = connector.SpeedAverage();
        float speedPercent = currentSpeed / maxMovingSpeed;
        
        //Debug.Log(speedPercent * 100 + "%");

        float lerpedValue = Mathf.MoveTowards(speedSlider.value, speedPercent, 0.05f * Time.deltaTime);

        speedSlider.value = speedPercent;
    }

    private void CheckOverspeed() {
        if(speedSlider.value >= 1) {

            if(noteCoroutine != null) 
                StopCoroutine(noteCoroutine);

            noteCoroutine = StartCoroutine(ShowOverspeedNote());
            speedSlider.value = 0;
            connector.RestartDrag();
        }
    }

    public IEnumerator ShowOverspeedNote() {
        overspeedNoteObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1.25f);
        overspeedNoteObject.SetActive(false);
    }
}
