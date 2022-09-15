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
        [SerializeField] float initialDelay = 1f;
        [SerializeField] TutorialPanel[] tutorialPanel = new TutorialPanel[]{};
        [SerializeField] Queue<TutorialPanel> tutorialQueue = new Queue<TutorialPanel>();
        TutorialPanel tutorialInstance = null;

        private void OnEnable() {
            tutorialQueue = new Queue<TutorialPanel>(tutorialPanel);
            this.InvokeWithDelay(() => ProgressTutorial(), initialDelay);
            TouchInputManager.onAnyInputStarted += ProgressTutorial;
        }
        private void OnDisable() {
            TouchInputManager.onAnyInputStarted -= ProgressTutorial;            
        }
        void ProgressTutorial() {
            tutorialInstance?.HideTutorial();
            if(tutorialQueue.Count == 0) {
                this.enabled = false;
                Debug.Log("Tutorial Ended!");
                return;
            }            
            tutorialInstance = tutorialQueue.Dequeue().ShowTutorial(transform.transform);
        }
    }
}