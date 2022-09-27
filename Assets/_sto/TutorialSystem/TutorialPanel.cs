using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib;
using GameLib.UI;
using GameLib.InputSystem;

namespace TutorialSystem
{
    public class TutorialPanel : UIPanel
    {
        [SerializeField] bool completeOnInput = true;
        private void OnEnable() {
            if (completeOnInput)
                TouchInputManager.onAnyInputStarted += TutorialManger.Instance.ProgressTutorial;
        }
        private void OnDisable() {
            TouchInputManager.onAnyInputStarted -= TutorialManger.Instance.ProgressTutorial;            
        }

        [SerializeField] RectTransform tutorialContainer = null;
        public void PlaceTutorialOverObject(Transform transform){
            if (transform == null) return;
            tutorialContainer.SetAnchors(UIManager.GetViewportPosition(transform.position));
        }
        public void RemoveTutorial(){
            DeactivatePanel( () => Destroy(this.gameObject));
        }
    }
}