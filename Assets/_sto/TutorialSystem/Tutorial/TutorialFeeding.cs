using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TutorialSystem
{
  public class TutorialFeeding : TutorialLogic
  {
    void Awake()
    {
    }
    private void OnEnable()
    {
      Level.onStart += OnLevelStarted;
    }
    private void OnDisable()
    {
      Level.onStart -= OnLevelStarted;
    }

    void OnLevelStarted(Level lvl)
    {
      if(lvl.isFeedingMode)
      {
        if(!GameState.Events.Tutorials.feedDone)
        {
          ActivateTutorial();
          GameState.Events.Tutorials.feedDone = true;
        }
      }
    }
  }
}
