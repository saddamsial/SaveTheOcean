using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TutorialSystem;

public class TutorialRewardChest : TutorialObject
{
    private void OnEnable() {
        RewardChest.onPoped += ProgressTutorial;
    }
    private void OnDisable() {
        RewardChest.onPoped -= ProgressTutorial;        
    }
}
