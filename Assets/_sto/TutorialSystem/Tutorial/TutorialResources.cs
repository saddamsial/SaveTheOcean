using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TutorialSystem;
using GameLib.InputSystem;

public class TutorialResources : TutorialLogic
{
  bool _active = false;
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
    if(_active)
    {
      ProgressTutorial();
      _active = false;
    }
  }
  void OnFirstPremium(Item item)
  { 
    if(!GameState.Tutorial.premiumDone)
    {
      //tutorialSequence[1].sender = item.transform;
      ActivateTutorial();
      GameState.Tutorial.premiumDone = true;
      _active = true;
    }
  }
}
