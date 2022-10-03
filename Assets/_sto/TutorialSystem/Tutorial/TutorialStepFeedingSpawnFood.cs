using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TutorialSystem
{
    public class TutorialStepFeedingSpawnFood : TutorialStep
    {
        protected override void OnEnabled() {
            FeedingMachine.onPoped += MoveToNextStep;
        }
        protected override void OnDisabled() {
            FeedingMachine.onPoped -= MoveToNextStep;
        }
    }
}