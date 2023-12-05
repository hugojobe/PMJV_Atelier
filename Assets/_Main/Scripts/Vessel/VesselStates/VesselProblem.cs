using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesselProblem : VesselNode {
    public override void OnStateEnter() {
        ChooseRoom().RequestProblem().SpawnProblem();
    }

    public override void OnStateExit() {

    }

    public override void OnStateUpdate() {

    }

    public RoomManager ChooseRoom() {
        return VesselManager.instance.roomManagers[Random.Range(0, VesselManager.instance.roomManagers.Count)];
    }

    public void OnProblemSolved() {
        VesselManager.instance.ChangeState(VesselState.IDLE);
    }
}
