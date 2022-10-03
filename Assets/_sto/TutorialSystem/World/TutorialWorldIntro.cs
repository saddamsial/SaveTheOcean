using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TutorialSystem;
public class TutorialWorldIntro : Tutorial
{
    protected override bool IsTutorialCompleted() => GameState.Progress.Locations.GetFinishedCnt() > 0;
}
