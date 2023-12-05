using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProblemSpawner : MonoBehaviour
{
    public string problemNameDebug;

    public void SpawnProblem() {
        Debug.Log($"Problem {problemNameDebug} Spawned");
    }

    public void OnProblemSolved() {
        VesselNode currentNode = VesselManager.instance.currentState;
        if (currentNode is VesselProblem) {
            (currentNode as VesselProblem).OnProblemSolved();
        }
    }
}
