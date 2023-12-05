using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryTesting : MonoBehaviour
{
    public HistoryState state = new HistoryState();

    private void Update() {
        if(Input.GetKeyDown(KeyCode.C))
            state = HistoryState.Capture();
    
        if(Input.GetKeyUp(KeyCode.R))
            state.Load();
    }
}
