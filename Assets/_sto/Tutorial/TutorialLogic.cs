using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib;
using GameLib.UI;
using GameLib.InputSystem;

namespace TutorialSystem
{
    public abstract class TutorialLogic : MonoBehaviour
    {
        public System.Action onTutorialCompleted;
        [SerializeField] protected TutorialSequence[] tutorialSequence = new TutorialSequence[]{}; 

        public void ActivateTutorial(object sender = null){
            this.enabled = true;
            TutorialContainer.Instance?.RequestTutorial(tutorialSequence);
        }
        public void ProgressTutorial() => ProgressTutorial(null);
        public void ProgressTutorial(object sender){
            TutorialContainer.Instance?.ProgressTutorial();
            if (!TutorialContainer.IsTutorialActive){
                onTutorialCompleted?.Invoke();
                this.enabled = false;
            }   
        }
    }
}