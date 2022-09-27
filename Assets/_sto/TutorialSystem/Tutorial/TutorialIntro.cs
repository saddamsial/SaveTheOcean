using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib.InputSystem;

namespace TutorialSystem
{
    public class TutorialIntro : TutorialLogic
    {
        void Awake() {
            this.enabled = GameState.Progress.locationIdx == 0 && GameState.Progress.Locations.GetLocationState(0) <= Level.State.Unlocked;
        }
        private void OnEnable() {
            ActivateTutorial();
            Item.onMerged += ProgressTutorial;
            Level.onFinished += ProgressTutorial;
        }
        private void OnDisable() {
            Item.onMerged -= ProgressTutorial;
            Level.onFinished -= ProgressTutorial;
        }
    }
}