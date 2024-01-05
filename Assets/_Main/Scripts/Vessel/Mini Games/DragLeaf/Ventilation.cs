using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ventilation : MonoBehaviour
{
    public bool hasLeavesOnVentilation;

    private void OnTriggerStay(Collider other) {
        if(other.CompareTag("Leaf"))
            Debug.Log(other.gameObject.name);
            hasLeavesOnVentilation = true;
    }

    private void OnTriggerExit(Collider other) {
        hasLeavesOnVentilation = false;
    }
}
