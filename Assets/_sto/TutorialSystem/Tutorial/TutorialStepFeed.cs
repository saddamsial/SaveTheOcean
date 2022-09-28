using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TutorialSystem
{
    public class TutorialStepFeed : TutorialStep
    {
        protected override void OnEnabled() {
            InputOverlayTargets.Add(FindObjectsOfType<Item>().FirstOrDefault()?.transform);
            InputOverlayTargets.Add(GetComponentInParent<Level>().GetPrimaryAnimalContainer());
            Item.onPut += MoveToNextStep;
            Item.onDropped += UpdateInputOverlay;
        }
        protected override void OnDisabled() {
            Item.onPut -= MoveToNextStep;
            Item.onDropped += UpdateInputOverlay;
        }
    }            
}