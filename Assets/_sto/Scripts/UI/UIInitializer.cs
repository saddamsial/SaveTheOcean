using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class UIInitializer : MonoBehaviour
{
    private void Awake() {
        GetComponent<Canvas>().enabled = true;
    }
}
