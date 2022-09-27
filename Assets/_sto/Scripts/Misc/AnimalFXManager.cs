using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib;
public class AnimalFXManager : MonoBehaviour
{
        [SerializeField] ParticleSystem fxWaterSplash;
        [SerializeField] ParticleSystem fxHit;
        [SerializeField] Transform fxContainer = null;

        public void ActivateSplash(){
            fxWaterSplash.Play();
        }
        public void ActivateHit(){
            fxHit.PlayAtPosition(fxContainer.position, fxContainer.up);
        }
}
