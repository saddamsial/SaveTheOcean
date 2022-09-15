using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TutorialSystem;

public class TutorialRewardChest : TutorialLogic
{
    void Awake() {
      this.enabled = false;
      RewardChest.onReward += OnRewardChest;
    }
    void Destroy() {
      RewardChest.onReward -= OnRewardChest;
    }
    private void OnEnable() {
      RewardChest.onPoped += ProgressTutorial;
    }
    private void OnDisable() {
      RewardChest.onPoped -= ProgressTutorial;
    }

    void OnRewardChest(RewardChest chest)
    { 
      if(chest.level == 0)
        this.enabled = true;
    }
}
