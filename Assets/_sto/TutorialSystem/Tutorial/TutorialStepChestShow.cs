using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TutorialSystem
{
    public class TutorialStepChestShow : TutorialStep
    {
        protected override void OnEnabled(){
            //TODO: Activate chest
            //TODO: Set chest state in save
            Debug.Log("A chest pops up!");
        }
        protected override void OnDisabled(){
        }
    }
}