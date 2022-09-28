using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib;
using GameLib.UI;
using GameLib.InputSystem;

namespace TutorialSystem
{
    public class TutorialStep : MonoBehaviour
    {
        [SerializeField] bool autoActivateTutorialPanel = true;
        [SerializeField] bool completeOnInput = true;
        [SerializeField] TutorialPanel tutorialPanelPrefab = null;

        [Header("Position")]
        [SerializeField] protected Transform tutorialSender = null;
        public void SetSender(Transform transform) => tutorialSender = transform;
        TutorialPanel tutorialPanelInstance = null;

        protected void ActivateTutorialPanel(object sender) => ActivateTutorialPanel();
        protected void ActivateTutorialPanel(Transform sender = null) {
            tutorialPanelInstance?.ShowTutorial(sender??tutorialSender);
            if (completeOnInput)
                TouchInputManager.onAnyInputStarted += MoveToNextStep;              
        }
        
        [Header("Input Overlay")]
        [SerializeField] public List<Transform> InputOverlayTargets = new List<Transform>();

        protected void UpdateInputOverlay(object sender) => ShowInputOverlay();
        protected void ShowInputOverlay(List<Transform> transforms = null) => TutorialManger.ShowInputOverlay(transforms??InputOverlayTargets);


        protected TutorialStep NextTutorial { get; set; } = null;

        protected Level GetActiveLevel() => GetComponentInParent<Level>();

        private void OnEnable() {
            OnEnabled();
            
            ShowInputOverlay(InputOverlayTargets);    
        }
        private void OnDisable() {
            TutorialManger.HideInputOverlay();

            TouchInputManager.onAnyInputStarted -= MoveToNextStep;
            
            Level.onFinished -= MoveToNextStep; //TODO: should be removed!?
            
            OnDisabled();

            tutorialPanelInstance?.DeactivatePanel(() => Destroy(tutorialPanelInstance.gameObject));
        }

        protected virtual void OnEnabled(){}
        protected virtual void OnDisabled(){}
        protected virtual void OnTutorialCompleted(){}

        public void InitializeTutorial(Transform sender = null){

            var hierarchyIndex = transform.GetSiblingIndex();
            NextTutorial = (hierarchyIndex < transform.parent.childCount - 1) ? transform.parent.GetChild(hierarchyIndex + 1)?.GetComponent<TutorialStep>() : null;

            if (NextTutorial == null) Level.onFinished += MoveToNextStep;

            Debug.Log("Tutorial | " + this.name + " => " + NextTutorial?.name??"-");

            if (tutorialPanelPrefab != null)
                tutorialPanelInstance = Instantiate(tutorialPanelPrefab, TutorialManger.Instance.transform);
            
            this.enabled = true;

            if (autoActivateTutorialPanel)
                ActivateTutorialPanel(sender);
        }
        private void OnDestroy() {
            if (tutorialPanelInstance != null)
                Destroy(tutorialPanelInstance.gameObject);
        }
        protected void MoveToNextStep(object sender) => MoveToNextStep();
        public void MoveToNextStep(){
            this.enabled = false;
            NextTutorial?.InitializeTutorial();
            if (NextTutorial == null) OnTutorialCompleted();
        }
    }
}