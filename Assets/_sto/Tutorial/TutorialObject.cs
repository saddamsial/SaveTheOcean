using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib;
using GameLib.UI;
using GameLib.InputSystem;

namespace TutorialSystem
{
    public class TutorialObject : MonoBehaviour
    {
        [SerializeField] bool showTutorial = true;      //TODO: add static ref to game save / tutorials completed
        [SerializeField] bool autoStart = true;
        [SerializeField] protected float startDelay = 1;

        [System.Serializable]
        public class TutorialSegment{
            [field: SerializeField] public TutorialPanel panel { get; private set; } = null;
            [field: SerializeField] public Transform sender { get; private set; } = null;
            // [field: SerializeField] public float activationDelay { get; private set; } = 0;
            // [field: SerializeField] public float minDisplayTime { get; private set; } = 1;
        }

        [SerializeField] TutorialSegment[] tutorialSequence = new TutorialSegment[]{};
        int tutorialProgress = 0;
        TutorialPanel tutorialInstance = null;

        private void Awake() {
            if (!showTutorial){
                Destroy(this);
                return;
            }
            if (!autoStart)
                this.enabled = false;
        }
        IEnumerator Start() {
            yield return new WaitForSeconds(startDelay);
            ProgressTutorial();
        }

        public void ProgressTutorial(object sender) => ProgressTutorial();
        public void ProgressTutorial() {
            tutorialInstance?.HideTutorial();

            if(tutorialProgress == tutorialSequence.Length) {
                this.enabled = false;
                Debug.Log("Tutorial Ended!");
                return;
            }

            var currentTutorialSegment = tutorialSequence[tutorialProgress];

            tutorialInstance = currentTutorialSegment.panel.ShowTutorial(currentTutorialSegment.sender);

            tutorialProgress++;
        }
    }
}