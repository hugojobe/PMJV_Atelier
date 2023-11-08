using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableStoreTesting : MonoBehaviour
{
    void Start() {
        VariableStore.CreateVariable("playerName", "Joueur");
        VariableStore.CreateVariable("playerOx", 0);
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.F5))
            VariableStore.PrintAllVariables();
    }
}
