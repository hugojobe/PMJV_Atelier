using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MiniGame : MonoBehaviour
{
    public UnityAction onMiniGameComplete;

    public void SolveProblem() {
        VesselManager.instance.OnProblemSolved.Invoke();
        Debug.Log("Problem Solved");
        Destroy(this.gameObject);
    }
}
