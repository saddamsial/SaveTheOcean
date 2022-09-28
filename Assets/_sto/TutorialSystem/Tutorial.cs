using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TutorialSystem
{
    [DefaultExecutionOrder(-10)]
    public class Tutorial : MonoBehaviour
    {
        [SerializeField] float initialDelay = 2f;
        TutorialStep[] tutorialSequence = new TutorialStep[]{};

        private void Awake() {
            tutorialSequence = GetComponentsInChildren<TutorialStep>();
            System.Array.ForEach(tutorialSequence, x => x.enabled = false);            
        }
        IEnumerator Start() {
            yield return new WaitForSeconds(initialDelay);
            Debug.Log("Tutorial Started");
            tutorialSequence.FirstOrDefault()?.InitializeTutorial();            
        }
    }
}