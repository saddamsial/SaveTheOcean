using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib;
using GameLib.UI;

namespace TutorialSystem
{
    [DefaultExecutionOrder(-10)]
    public class TutorialManger : MonoBehaviour
    {
        public static System.Action onTutorialStepCompleted;
        public static TutorialManger Instance = null;

        TutorialSequence[] _activeTutorial = null;
        public static bool IsTutorialActive => Instance._activeTutorial != null;
        int _activeTutorialSegment = 0;
        TutorialPanel _activeTutorialPanelInstance = null;

        public bool ShowTutorials = true;
        public float initialTutorialDelay = 1;
        public float tutorialStepDelay = 1f;

        private void Awake() {
            Instance = this;
            if (!ShowTutorials){ Destroy(this.gameObject); }
            //subscribe to event that deactivates game board
        }
        private void OnDestroy() {
            //unsubscribe from event that deactivates game board
        }

        public void RequestTutorial(TutorialSequence[] tutorial){
            if (_activeTutorial != null) { Debug.Log("Tutorial | In progress ... requested tutorial will not be displayed!"); return; }

            Debug.Log("Tutorial | Tutorial Requested");

            _activeTutorial = tutorial;
            _activeTutorialSegment = 0;
            
            this.InvokeWithDelay(() => ProgressTutorial(), initialTutorialDelay);
        }
        public void ProgressTutorial(){
            if (_activeTutorial == null) return;

            _activeTutorialPanelInstance?.RemoveTutorial();
            onTutorialStepCompleted?.Invoke();

            if(_activeTutorialSegment >= _activeTutorial.Length) {
                _activeTutorial = null;
                _activeTutorialPanelInstance = null;
                Debug.Log("Tutorial | Tutorial Ended!");
                return;
            }

            var currentTutorialSegment = _activeTutorial[_activeTutorialSegment];

            _activeTutorialPanelInstance = Instantiate(currentTutorialSegment.panel, this.transform);
            _activeTutorialPanelInstance.PlaceTutorialOverObject(currentTutorialSegment.sender);
            _activeTutorialPanelInstance.ActivatePanel(tutorialStepDelay);

            Debug.Log("Tutorial | Step " + (_activeTutorialSegment + 1) + "/" + _activeTutorial.Length + " | " + currentTutorialSegment.stepName);

            _activeTutorialSegment++;   
        }
        public void ClearTutorial(){
            foreach (var tutorialPanel in GetComponentsInChildren<TutorialPanel>())
                Destroy(tutorialPanel.gameObject);

        }
    }
}