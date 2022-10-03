using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameLib;
using GameLib.UI;

namespace TutorialSystem
{
    [DefaultExecutionOrder(-50)]
    public class TutorialManger : MonoBehaviour
    {
        public static System.Action onTutorialStepCompleted;
        public static TutorialManger Instance = null;

        InputOverlayTutorial inputOverlayTutorial = null;

        public bool ShowTutorials = true;

        private void Awake() {
            Instance = this;

            inputOverlayTutorial = GetComponent<InputOverlayTutorial>();

            if (!ShowTutorials){ Destroy(this.gameObject); }
        }

        public static void ShowInputOverlay(Vector3 tapPosition) => Instance.inputOverlayTutorial.Activate(tapPosition);
        public static void ShowInputOverlay(List<Transform> tapPositions){
            if (tapPositions.Any(x => x == null)) { Debug.Log("Tutorial Manager | Missing Input Overlay Target | Unable to show tutorial"); return; }
            
            if (tapPositions.Count == 0) return;
            
            if (tapPositions.Count > 1)
                Instance.inputOverlayTutorial.Activate(tapPositions.Select(x => x.position).ToArray());
            else
                Instance.inputOverlayTutorial.Activate(tapPositions[0].position);
        }
        public static void ShowInputOverlay(Vector3[] pointList) => Instance.inputOverlayTutorial.Activate(pointList);
        public static void HideInputOverlay() => Instance.inputOverlayTutorial.Deactivate();
    }
}