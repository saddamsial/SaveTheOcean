using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib;
using GameLib.UI;

namespace TutorialSystem
{
    public class TutorialPanel : UIPanel
    {
        [SerializeField] RectTransform tutorialContainer = null;
        public void PlaceTutorialOverObject(Transform transform){
            tutorialContainer.SetAnchors(UIManager.GetViewportPosition(transform.position));
        }
        public void RemoveTutorial(){
            DeactivatePanel( () => Destroy(this.gameObject));
        }
    }
}