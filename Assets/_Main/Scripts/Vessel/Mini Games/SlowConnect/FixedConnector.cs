using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FixedConnector : MonoBehaviour
{
    private SlowConnect SlowConnect => GetComponentInParent<SlowConnect>();

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("MovableConnector")) {
            SlowConnect.SolveProblem();
        }
    }
}
