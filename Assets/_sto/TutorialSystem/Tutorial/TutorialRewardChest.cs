using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TutorialSystem;

public class TutorialRewardChest : TutorialLogic
{
    void Awake() {
        // this.enabled = chest.level == 0;
    }
    private void OnEnable() {
      RewardChest.onReward += ActivateTutorial;
      RewardChest.onPoped += ProgressTutorial;
    }
    private void OnDisable() {
      RewardChest.onReward -= ActivateTutorial;
      RewardChest.onPoped -= ProgressTutorial;
    }

    void OnRewardChest(RewardChest chest)
    { 
      if(chest.level == 0)
        this.enabled = true;
    }
}
