using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TutorialSystem;

public class TutorialFeeding : Tutorial
{
  protected override bool IsTutorialCompleted() => GameState.Feeding.visits > 0;
}
