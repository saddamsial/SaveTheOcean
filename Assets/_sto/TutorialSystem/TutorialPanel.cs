using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameLib;
using GameLib.UI;
using GameLib.InputSystem;

namespace TutorialSystem
{
    public class TutorialPanel : UIPanel
    {
        [SerializeField] RectTransform tutorialContainer = null;
        Transform tutorialSender = null;

        public void ShowTutorial(Transform sender = null){
            transform.SetParent(TutorialManger.Instance.transform);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            PlaceOverObject(sender);
            ActivatePanel();            
        }
        void PlaceOverObject(Transform sender){
            if (sender == null) return;
            tutorialContainer.SetAnchors(UIManager.GetViewportPosition(sender.position));
        }
    }
}