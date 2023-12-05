using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class VesselIdle : VesselNode {
    private float waitTime, currentWaitTime;

    public override void OnStateEnter() {
        currentWaitTime = 0;
        waitTime = Random.Range(3f, 6f);
    }

    public override void OnStateExit() {

    }

    public override void OnStateUpdate() {
        if(currentWaitTime < waitTime) {
            currentWaitTime += Time.deltaTime;
        } else {
            SetProblem();
        }
    }

    public void SetProblem() {
        VesselManager.instance.ChangeState(VesselState.PROBLEM);
    }
}
