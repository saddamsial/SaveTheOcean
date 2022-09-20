using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TutorialSystem;
using GameLib.InputSystem;

public class TutorialResources : TutorialLogic
{
    bool active = false;
    private void OnEnable(){
      Level.onPremiumItem += OnFirstPremium;
      Level.onItemCollected += OnItemCollected;
    }
    private void OnDisable(){

      Level.onPremiumItem -= OnFirstPremium;
      Level.onItemCollected -= OnItemCollected;
    }

    void OnItemCollected(Item item)
    {
      ProgressTutorial();
    }
    void OnFirstPremium(Item item)
    { 
      if(!GameState.Tutorial.premiumDone)
      {
        tutorialSequence[1].panel.PlaceTutorialOverObject(item.transform);
        //tutorialSequence[1].sender = item.transform;

        ActivateTutorial(item.transform);
        GameState.Tutorial.premiumDone = true;
      }
    }
}
