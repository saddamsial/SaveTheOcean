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

        [SerializeField] float minOnScreenTime = 1f;
        float _tutorialMinViewedTime = 0f;

        TutorialLogic _activeTutorial = null;
        public static bool IsTutorialActive => Instance._activeTutorial != null;
        int _activeTutorialSegment = 0;
        TutorialPanel _activeTutorialPanelInstance = null;

        public bool ShowTutorials = true;
        public float initialTutorialDelay = 1;
        public float tutorialStepDelay = 1f;

        private void Awake() {
            Instance = this;
            if (!ShowTutorials){ Destroy(this.gameObject); }

            // Level.onCreate += ClearTutorial;
            // Level.onDestroy += ClearTutorial;
        }
        private void OnDestroy() {
            // Level.onCreate -= ClearTutorial;
            // Level.onDestroy -= ClearTutorial;
        }

        public void RequestTutorial(TutorialLogic tutorial){
            if (_activeTutorial != null) { Debug.Log("Tutorial | In progress ... requested tutorial will not be displayed!"); return; }

            Debug.Log("Tutorial | Tutorial Requested");

            _activeTutorial = tutorial;
            _activeTutorialSegment = 0;
            
            this.InvokeWithDelay(() => ProgressTutorial(), initialTutorialDelay);
        }
        public void ProgressTutorial(){
            if (_activeTutorial == null) return;

            if (Time.time < _tutorialMinViewedTime) return;   

            _activeTutorialPanelInstance?.RemoveTutorial(); 
            _activeTutorialPanelInstance = null;
                
            onTutorialStepCompleted?.Invoke();

            if(_activeTutorialSegment >= _activeTutorial.TutorialSequence.Length) {
                _activeTutorial.OnTutorialEnded();
                _activeTutorial = null;
                _activeTutorialPanelInstance = null;
                Debug.Log("Tutorial | Tutorial Ended!");
                return;
            }

            var currentTutorialSegment = _activeTutorial.TutorialSequence[_activeTutorialSegment];

            Debug.Log("Tutorial | Step " + (_activeTutorialSegment + 1) + "/" + _activeTutorial.TutorialSequence.Length + " | " + currentTutorialSegment.stepName);

            if (currentTutorialSegment.panel != null)
            {
                _activeTutorialPanelInstance = Instantiate(currentTutorialSegment.panel, this.transform);
                _activeTutorialPanelInstance.PlaceTutorialOverObject(currentTutorialSegment.sender);
                _activeTutorialPanelInstance.ActivatePanel(tutorialStepDelay);
                _tutorialMinViewedTime = Time.time + minOnScreenTime;
            }

            _activeTutorialSegment++;
        }
        public void ClearTutorial(object sender) => ClearTutorial();
        public void ClearTutorial(){
            foreach (var tutorialPanel in GetComponentsInChildren<TutorialPanel>())
                Destroy(tutorialPanel.gameObject);

        }
    }
}