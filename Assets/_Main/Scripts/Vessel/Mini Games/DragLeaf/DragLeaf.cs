using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragLeaf : MiniGame
{
    private Ventilation ventilation => GetComponentInChildren<Ventilation>();

    public float timeToWaitAfterNothing;
    public float currentTimer;

    void Start() {
        base.Start();
    }

    void Update() {
        CheckTimer();
    }

    public void CheckTimer() {
        if(ventilation.hasLeavesOnVentilation)
            currentTimer = 0;
        else
            currentTimer += Time.deltaTime;

        if(currentTimer >= timeToWaitAfterNothing)
            SolveProblem();
    }
}
