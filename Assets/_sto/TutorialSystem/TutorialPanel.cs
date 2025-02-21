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
        [SerializeField] RectTransform inputBlocker = null;
        [SerializeField] RectTransform tutorialContainer = null;
        [SerializeField] Image focusFade = null;
        Transform tutorialSender = null;

        [SerializeField] TextMeshProUGUI nameLabel = null;
        public void SetSenderName(string name) => nameLabel.text = name;
        [SerializeField] float screenMargin = 64f;

        Vector2 dialogRectSize = Vector2.zero;

        public void ShowTutorial(Transform sender = null){
            transform.SetParent(TutorialManger.Instance.transform);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.localScale = Vector3.one;

            var viewportPosition = UIManager.GetViewportPosition(sender.position);


            if (sender != null){
                PlaceInViewport(viewportPosition);

                if (focusFade != null) {
                    focusFade.material = Instantiate(focusFade.material);
                    focusFade?.material.SetVector("_ViewportPosition", viewportPosition);
                }
            }
            rectTransform.ContainWithin(GetComponentInParent<RectTransform>(), Camera.main, screenMargin);

            var senderAnimal = sender?.GetComponentInChildren<Animal>();
            if (sender != null && nameLabel != null)
                nameLabel.text = senderAnimal?.DisplayName??"-";

            ActivatePanel();            
        }
        void PlaceOverObject(Transform sender){
            if (sender == null) return;
            tutorialContainer.SetAnchors(UIManager.GetViewportPosition(sender.position));
        }
        void PlaceInViewport(Vector2 viewportPosition){
            tutorialContainer.SetAnchors(viewportPosition);
        }
        public void BlockInput(bool targetState){
            if (inputBlocker == null) return;
            inputBlocker.gameObject.SetActive(targetState);
        }

    }
}