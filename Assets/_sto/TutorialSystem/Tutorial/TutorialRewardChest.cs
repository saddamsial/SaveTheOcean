using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TutorialSystem;
using GameLib.InputSystem;

public class TutorialRewardChest : TutorialLogic
{
    bool _activated = false;
    void Awake() {
        // this.enabled = chest.level == 0;
    }
    private void OnEnable() {
      RewardChest.onReward += OnRewardChest;
      TouchInputManager.onAnyInputStarted += OnClicked;
      RewardChest.onPoped += ProgressTutorial;
    }
    private void OnDisable() {
      RewardChest.onReward -= OnRewardChest;
      TouchInputManager.onAnyInputStarted -= OnClicked;
      RewardChest.onPoped -= ProgressTutorial;
      _activated = false;       
    }

    void OnClicked()
    {
      if(_activated)
        ProgressTutorial();
    }
    void OnRewardChest(RewardChest chest)
    { 
      if(chest.level == 0)
      {
        ActivateTutorial();
        _activated = true;
      }
    }
}
