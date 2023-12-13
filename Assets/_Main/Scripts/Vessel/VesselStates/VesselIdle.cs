using UnityEngine;

public class VesselIdle : VesselNode {
    private float waitTime, currentWaitTime;

    public override void OnStateEnter() {
        foreach(var obj in VesselManager.instance.objectsToDisableInVesselMode) {
            obj.SetActive(false);
        }

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
        if(!VesselManager.instance.canGenerateProblem) return;

        VesselManager.instance.ChangeState(VesselState.PROBLEM);
    }
}
