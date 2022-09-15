using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib.InputSystem;

namespace TutorialSystem
{
    public class TutorialIntro : TutorialObject
    {
        private void OnEnable() {
            TouchInputManager.onAnyInputStarted += ProgressTutorial;
            // Level.onTutorialStart += ActivateTutorial;
        }
        private void OnDisable() {
            TouchInputManager.onAnyInputStarted -= ProgressTutorial;            
            // Level.onTutorialStart -= ActivateTutorial;
        }

        void ActivateTutorial(object sender){
            ProgressTutorial();
        }
    }
}