using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TutorialSystem;

public class TutorialFeedingMode : Tutorial
{
    protected override bool IsTutorialCompleted() => GameState.Feeding.visits > 0; //GameState.Progress.Locations.GetFinishedCnt() > GameData.Levels.GetFeedingAvailLoc();
    private void OnEnable() 
    {
      //subscribe to event Feeding unlocked / focused on map
      Earth.onLocationFocused += OnLocationFocused;
    }
    private void OnDisable()
    {
      Earth.onLocationFocused -= OnLocationFocused;
    }
    private void OnLocationFocused(int loc_idx)
    {
      if(loc_idx == Location.FeedLocation && GameState.Feeding.visits == 0)
        StartTutorial();
    }
}
