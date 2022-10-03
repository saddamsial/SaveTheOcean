using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TutorialSystem;
public class TutorialFeedingMode : Tutorial
{
    protected override bool IsTutorialCompleted() => GameState.Progress.Locations.GetFinishedCnt() > 2;
    private void OnEnable() {
        //subscribe to event Feeding unlocked / focused on map
        // += StartTutorial;
    }
}
