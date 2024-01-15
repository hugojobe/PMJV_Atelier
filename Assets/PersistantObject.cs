using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistantObject : MonoBehaviour
{
    private void Start() {
        DontDestroyOnLoad(gameObject);
    }
}
