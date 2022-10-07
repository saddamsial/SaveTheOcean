using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TutorialSystem
{
    public class TutorialStepResources : TutorialStep
    {
        bool _shown = false;
        void Awake()
        {
          //Level.onPremiumItem += OnShowPremiumItem;
        }
        protected override void OnEnabled(){
          Item.onShown += OnShowPremiumItem;
        }
        protected override void OnDisabled(){

          Item.onShown -= OnShowPremiumItem;
        }
        void OnShowPremiumItem(Item sender){
            if(sender.id.IsSpecial && !_shown) // NextTutorial == null)
            {
              GameState.StorageBox.shown = true;
              _shown = true;
              Debug.Log("Premium Item visible!");
              //this.enabled = true;
              ActivateTutorialPanel();
              NextTutorial.InputOverlayTargets = new List<Transform>(){sender.transform};
            }
        }
    }
}
