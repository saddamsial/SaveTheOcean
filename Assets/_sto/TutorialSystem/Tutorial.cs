using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameLib;

namespace TutorialSystem
{
    [DefaultExecutionOrder(-10)]
    public class Tutorial : MonoBehaviour
    {
        [SerializeField] float initialDelay = 2f;
        TutorialStep[] tutorialSequence = new TutorialStep[]{};
        protected virtual bool IsTutorialCompleted() => false;
        private void Awake(){
            if(IsTutorialCompleted())
            {
              Destroy(this.gameObject);
            }
            else
            {
              tutorialSequence = GetComponentsInChildren<TutorialStep>();
              System.Array.ForEach(tutorialSequence, x => x.enabled = false);
            }
        }
        private void OnEnable(){
          this.InvokeWithDelay(() => StartTutorial(), initialDelay);
        }
        public void StartTutorial(){
          tutorialSequence.FirstOrDefault()?.InitializeTutorial();
          Debug.Log("Tutorial Started");
        }
    }
}