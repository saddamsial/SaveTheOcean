using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib.Haptics;

public class HapticManager : MonoBehaviour
{ 
  void OnEnable()
  {
    Level.onStart += VibMed;
    Level.onFinished += VibMed;
    Item.onMerged += VibLo;
  }
  void OnDisable()
  {
    Level.onStart -= VibMed;
    Level.onFinished -= VibMed;
    Item.onMerged -= VibLo;
  }

  void VibLo0() => VibLo(null);
  void VibMed0() => VibMed(null);
  void VibHi0() => VibHi(null);
  void VibLo2(object obj0, object obj1) => VibLo(null);
  void VibMed2(object obj0, object obj1) => VibMed(null);
  void VibHi2(object obj0, object obj1) => VibHi(null);


  void VibLo(object sender)
  {
    HapticsSystem.Vibrate(HapticsSystem.HapticFeedback.Tick);
  }
  void VibMed(object sender)
  {
    HapticsSystem.Vibrate(HapticsSystem.HapticFeedback.Medium);
  }
  void VibHi(object sender)
  {
    HapticsSystem.Vibrate(HapticsSystem.HapticFeedback.High);
  }
}
