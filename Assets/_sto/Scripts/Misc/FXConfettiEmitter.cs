using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXConfettiEmitter : MonoBehaviour
{
    ParticleSystem ps = null;

    private void Awake() {
        ps = GetComponent<ParticleSystem>();
    }
    private void OnEnable() {
        EffectsManager.onPlayConfetti += EmitConfetti;
    }
    private void OnDisable() {
        EffectsManager.onPlayConfetti -= EmitConfetti;
    }
    void EmitConfetti() => ps.Play();
}
