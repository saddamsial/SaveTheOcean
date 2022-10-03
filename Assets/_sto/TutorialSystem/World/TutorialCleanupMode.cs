using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib;
using TutorialSystem;
public class TutorialCleanupMode : Tutorial
{
    protected override bool IsTutorialCompleted() => GameState.Progress.Locations.GetFinishedCnt() > 3;
    private void OnEnable() {
        //subscribe to event Feeding unlocked / focused on map
        // += StartTutorial;
    }
}
