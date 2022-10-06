using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TutorialSystem;

public class TutorialLevel : Tutorial
{
  //protected override bool IsTutorialCompleted() => GameState.Progress.Locations.GetLocationState(GameState.Progress.locationIdx) > Level.State.Unlocked;
  protected override bool IsTutorialCompleted() => GameState.Progress.Locations.GetLocationVisits(GameState.Progress.locationIdx) > 0;
}

