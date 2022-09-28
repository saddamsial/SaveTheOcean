using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TutorialSystem
{
    [DefaultExecutionOrder(-10)]
    public class Tutorial : MonoBehaviour
    {
        [SerializeField] float initialDelay = 2f;
        TutorialStep[] tutorialSequence = new TutorialStep[]{};
        protected virtual bool IsTutorialCompleted()
        {
          var state = GameState.Progress.Locations.GetLocationState(GameState.Progress.locationIdx);
          return  state > Level.State.Unlocked;
        }
        private void Awake() {

            if(IsTutorialCompleted())
              Destroy(this.gameObject);
            else  
            {
              tutorialSequence = GetComponentsInChildren<TutorialStep>();
              System.Array.ForEach(tutorialSequence, x => x.enabled = false);            
            }
        }
        IEnumerator Start() {
            yield return new WaitForSeconds(initialDelay);
            Debug.Log("Tutorial Started");
            tutorialSequence.FirstOrDefault()?.InitializeTutorial();            
        }
    }
}