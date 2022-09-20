using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TutorialSystem;
using GameLib.InputSystem;

public class TutorialStorage : TutorialLogic
{
    bool _activated = false;
    void Awake() 
    {

    }
    private void OnEnable() 
    {
      StorageBox.onShow += OnShow;
      //TouchInputManager.onAnyInputStarted += OnClicked;
      RewardChest.onPoped += ProgressTutorial;
    }
    private void OnDisable() 
    {
      StorageBox.onShow -= OnShow;
      //TouchInputManager.onAnyInputStarted -= OnClicked;
      RewardChest.onPoped -= ProgressTutorial;
      _activated = false;       
    }

    // void OnClicked()
    // {
    //   if(_activated)
    //     ProgressTutorial();
    // }
    void OnShow(StorageBox strorage)
    { 
      if(GameState.StorageBox.IsFirstShow())
      {
        ActivateTutorial();
        _activated = true;
      }
    }
}
