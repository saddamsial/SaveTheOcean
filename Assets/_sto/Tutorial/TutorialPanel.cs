using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib;
using GameLib.UI;

namespace TutorialSystem
{
    public class TutorialPanel : UIPanel
    {
        public TutorialPanel ShowTutorial(Transform sender){
            var instance = Instantiate(this, TutorialContainer.Instance.transform);
            instance.ContentContainer.SetAnchors(UIManager.GetViewportPosition(sender.position));
            instance.ActivatePanel();
            return instance;
        }

        public void HideTutorial(){
            DeactivatePanel( ()=> Destroy(this.gameObject));
        }

    }
}