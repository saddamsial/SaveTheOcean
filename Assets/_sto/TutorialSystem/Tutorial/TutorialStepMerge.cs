using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TutorialSystem
{
    public class TutorialStepMerge : TutorialStep
    {
        protected override void OnEnabled() {
            InputOverlayTargets = GetActiveLevel().listItems.Take(2).Select(x => x.transform).ToList();

            Item.onMerged += MoveToNextStep;
            Item.onDropped += UpdateInputOverlay;
        }
        protected override void OnDisabled() {
            Item.onMerged -= MoveToNextStep;
            Item.onDropped -= UpdateInputOverlay;
        }
    }
}