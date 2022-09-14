using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib.UI;

namespace TutorialSystem
{
    public class TutorialContainer : MonoBehaviour
    {
        public static TutorialContainer Instance = null;

        private void Awake() {
            Instance = this;
        }
    }
}