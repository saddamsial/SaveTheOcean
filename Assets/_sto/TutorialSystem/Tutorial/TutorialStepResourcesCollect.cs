using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TutorialSystem
{
    public class TutorialStepResourcesCollect : TutorialStep
    {
        protected override void OnEnabled(){
            Level.onItemCollected += MoveToNextStep;
            Item.onDropped += UpdateInputOverlay;
        }
        protected override void OnDisabled(){
            Level.onItemCollected -= MoveToNextStep;  
            Item.onDropped -= UpdateInputOverlay;           
        }
    }
}
