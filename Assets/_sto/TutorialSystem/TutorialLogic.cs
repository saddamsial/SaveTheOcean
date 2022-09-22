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
        [SerializeField] protected TutorialSequence[] tutorialSequence = new TutorialSequence[]{};

        void OnDestroy()
        {
          TutorialManger.Instance.ClearTutorial();
        }
        public void SetUndefinedSenders(Transform senderTransform){
            foreach (var tutorial in tutorialSequence)
                if (tutorial.sender == null) tutorial.sender = senderTransform;
        }

        public void ActivateTutorial(object sender = null){
            this.enabled = true;
            TutorialManger.Instance?.RequestTutorial(tutorialSequence);
        }
        public void ProgressTutorial() => ProgressTutorial(null);
        public void ProgressTutorial(object sender = null){
            TutorialManger.Instance?.ProgressTutorial();
            if (!TutorialManger.IsTutorialActive){
                this.enabled = false;
            }   
        }
    }
}