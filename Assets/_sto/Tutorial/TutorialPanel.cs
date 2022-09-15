using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib;
using GameLib.UI;

namespace TutorialSystem
{
    public class TutorialPanel : UIPanel
    {
        [SerializeField] RectTransform positionContainer = null;
        public TutorialPanel ShowTutorial(Transform sender, float activationDelay  = 0f){
            if (TutorialContainer.Instance == null) { Debug.Log("Tutorial | No Tutorial Container present on the scene | Please ad a container"); return null; }

            var instance = Instantiate(this, TutorialContainer.Instance.transform);
            instance.positionContainer.SetAnchors(UIManager.GetViewportPosition(sender.position));
            instance.ActivatePanel(activationDelay);
            return instance;
        }

        public void HideTutorial(){
            DeactivatePanel( () => Destroy(this.gameObject));
        }
    }
}