using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib.InputSystem;

namespace TutorialSystem
{
    public class TutorialRewardChest : TutorialLogic
    {
        void Awake() {
            // this.enabled = GameState.Progress.locationIdx == 0 && GameState.Progress.Locations.GetLocationState(0) <= Level.State.Unlocked;
        }
        private void OnEnable() {
            ActivateTutorial();
            RewardChest.onReward += ProgressTutorial;
            RewardChest.onPoped += ProgressTutorial;
        }
        private void OnDisable() {
            RewardChest.onReward -= ProgressTutorial;
            RewardChest.onPoped -= ProgressTutorial;
        }
    }
}