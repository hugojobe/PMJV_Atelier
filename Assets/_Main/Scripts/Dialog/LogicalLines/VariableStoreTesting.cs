using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableStoreTesting : MonoBehaviour
{
    void Start() {
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.F5))
            VariableStore.PrintAllVariables();
    }
}
