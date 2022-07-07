using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib.AudioSystem;
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioPlayer ballSpawnAudio = null;
    [SerializeField] AudioPlayer ballHitAudio = null;
    [SerializeField] AudioPlayer ballDestroyAudio = null;
    [SerializeField] AudioPlayer victoryAudio = null;
    [SerializeField] AudioPlayer ballsMatchAudio = null;

    private void OnEnable() {
        Level.onDone += PlayConfettiAudio;
        Item.onMerged += PlayBallsMatchAudio;
    }
    private void OnDisable() {
        Level.onDone -= PlayConfettiAudio;
        Item.onMerged -= PlayBallsMatchAudio;
    }

    void PlayBallSpawnSFX(object sender) => ballSpawnAudio.Play();
    void PlayBallHitSFX(object sender) => ballHitAudio.Play();
    void PlayBallDestroyAudio(object sender) => ballDestroyAudio.Play();
    void PlayConfettiAudio(object sender) => victoryAudio.Play();
    void PlayBallsMatchAudio(object sender) => ballsMatchAudio?.Play();

}
