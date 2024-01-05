using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyTurn : MiniGame
{
    private KeyTurnRing keyTurn => GetComponentInChildren<KeyTurnRing>();

    private float currentTimeWithoutTouched;
    public float timeRequiredWithoutTouch;
    public bool notTouched;


    private void Start() {
        base.Start();

        keyTurn.canPlay = true;
    }

    private void Update() {
        CheckTouched();

        if(currentTimeWithoutTouched > timeRequiredWithoutTouch) {
            CheckVictory();
        }
    }

    private void CheckTouched() {
        if(notTouched) {
            currentTimeWithoutTouched += Time.deltaTime;
        }
    }

    public void CheckVictory() {
        if(keyTurn.rotationAngle <= -90f) {
            keyTurn.canPlay = false;
            StartCoroutine(VicoryCoroutine());
        }
    }

    private IEnumerator VicoryCoroutine() {
        yield return new WaitForSeconds(0.75f);
        SolveProblem();
    }
}
