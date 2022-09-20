using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TutorialSystem
{
    [System.Serializable]
    public class TutorialSequence
    {
        [field: SerializeField] public string stepName { get; private set; } = "Tutorial";
        [field: SerializeField] public TutorialPanel panel { get; private set; } = null;
        [field: SerializeField] public Transform sender { get; set; } = null;
        // [field: SerializeField] public float activationDelay { get; private set; } = 0;
        // [field: SerializeField] public float minDisplayTime { get; private set; } = 1;
    }
}