using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TutorialSystem
{
    public class TutorialStepChestShow : TutorialStep
    {
        [SerializeField] RewardChest _rewardChest = null;

        protected override void OnEnabled(){
            _rewardChest.Show();
            Debug.Log("A chest pops up!");
        }
        protected override void OnDisabled(){
        }
    }
}