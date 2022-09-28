using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TutorialSystem
{
    public class TutorialStepResources : TutorialStep
    {
        protected override void OnEnabled(){
            //TODO: not working when it's on initial grid? 
            Level.onPremiumItem += OnShowPremiumItem;
        }
        protected override void OnDisabled(){
            Level.onPremiumItem -= OnShowPremiumItem;
        }
        void OnShowPremiumItem(Item sender){
            Debug.Log("Premium Item visible!");
            this.enabled = true;
            ActivateTutorialPanel();
            NextTutorial.InputOverlayTargets = new List<Transform>(){sender.transform};
        }
    }
}
