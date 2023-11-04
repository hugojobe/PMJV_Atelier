using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableStoreTesting : MonoBehaviour
{
    void Start() {
        VariableStore.CreateDatabase("DB1");
        VariableStore.CreateDatabase("DB2");
        VariableStore.CreateDatabase("DB3");

        VariableStore.CreateVariable("DB1.num1", 1);
        VariableStore.CreateVariable("DB2.float1", 2.5f);
        VariableStore.CreateVariable("DB3.bool1", true);
        VariableStore.CreateVariable("string1", "ui");

        VariableStore.PrintAllDatabase();

        VariableStore.PrintAllVariables();
    }

    private void Update() {
        
    }
}
