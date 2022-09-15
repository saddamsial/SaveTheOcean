using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib.InputSystem;

namespace TutorialSystem
{
    public class TutorialIntro : TutorialObject
    {
      void Awake()
      {
        this.enabled = GameState.Progress.locationIdx == 0 && GameState.Progress.Locations.GetLocationState(0) <= Level.State.Unlocked;
      }
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