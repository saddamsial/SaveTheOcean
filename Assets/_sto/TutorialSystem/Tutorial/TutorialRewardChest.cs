using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TutorialSystem;
using GameLib.InputSystem;

public class TutorialRewardChest : TutorialLogic
{
    private void OnEnable() {
      RewardChest.onReward += OnRewardChest;
      RewardChest.onPoped += ProgressTutorial;
    }
    private void OnDisable() {
      RewardChest.onReward -= OnRewardChest;
      RewardChest.onPoped -= ProgressTutorial;
    }

    void OnRewardChest(RewardChest chest)
    { 
      if(!GameState.Tutorial.chestDone)
      {
        ActivateTutorial();
        GameState.Tutorial.chestDone = true;
      }
    }
}
