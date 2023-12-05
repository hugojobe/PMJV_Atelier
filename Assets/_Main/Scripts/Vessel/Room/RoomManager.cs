using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    VesselManager vesselManager => VesselManager.instance;

    public List<ProblemSpawner> problems;
    public ProblemSpawner lastProblem;

    public ProblemSpawner RequestProblem() {
        var availableProblems = problems.Where(p => p != lastProblem).ToList();

        var newProblem = problems[Random.Range(0, availableProblems.Count)];
        lastProblem = newProblem;

        return newProblem;
    }
}
