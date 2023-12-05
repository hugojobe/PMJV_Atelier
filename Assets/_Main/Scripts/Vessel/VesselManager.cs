using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesselManager : MonoBehaviour
{
    [HideInInspector] public VesselNode currentState;

    [HideInInspector] public static VesselManager instance;

    public List<RoomManager> roomManagers;

    private void Awake() {
        if(instance == null) instance = this;
        ChangeState(VesselState.IDLE);
    }

    private void Update() {
        currentState.OnStateUpdate();
    }

    public void ChangeState(VesselState newState) {
        VesselNode newNode = null;
        switch(newState) {
            case VesselState.IDLE:
                newNode = new VesselIdle();
                break;
            case VesselState.PROBLEM:
                newNode = new VesselProblem();
                break;
        }

        SwitchState(newNode);
    }

    public void SwitchState(VesselNode newState) {
        currentState?.OnStateExit();
        currentState = newState;
        currentState.OnStateEnter();
    }
}

public enum VesselState {
    IDLE,
    PROBLEM,
}
