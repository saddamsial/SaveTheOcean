using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TutorialSystem;

public class TutorialCleanupMode : Tutorial
{
    protected override bool IsTutorialCompleted() => GameState.Cleanup.visits > 0;
    private void OnEnable() 
    {
      //subscribe to event Clearing unlocked / focused on map
      Earth.onLocationFocused += OnLocationFocused;
    }
    private void OnDisable()
    {
      Earth.onLocationFocused -= OnLocationFocused;
    }
    private void OnLocationFocused(int loc_idx)
    {
      if(loc_idx == Location.ClearLocation && GameState.Cleanup.visits == 0)
        StartTutorial();
    }    
}
