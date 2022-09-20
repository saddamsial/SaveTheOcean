using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor {
    Level level = null;
    private void OnEnable() {
        level = (Level)target;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        GUILayout.Label("Dev Info");
        GUILayout.Label("Number of moves: " + level.GetNumberOfMovesToSolve());
    }
}
