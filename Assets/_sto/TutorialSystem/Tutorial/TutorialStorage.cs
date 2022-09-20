using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TutorialSystem;
using GameLib.InputSystem;

public class TutorialStorage : TutorialLogic
{
    private void OnEnable() 
    {
      StorageBox.onShow += OnShow;
    }
    private void OnDisable() 
    {
      StorageBox.onShow -= OnShow;
    }

    void OnShow(StorageBox strorage)
    { 
      if(!GameState.Tutorial.storageDone)
      {
        ActivateTutorial();
        GameState.Tutorial.storageDone = true;
      }
    }
}
