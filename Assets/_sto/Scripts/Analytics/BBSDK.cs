#if BYTE_BREW_SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ByteBrewSDK;

public class BBSDK : MonoBehaviour
{
  void Awake()
  {
    ByteBrew.InitializeByteBrew();

    Level.onStart += OnLevelStart;
    Level.onFinished += OnLevelFinished;
  }
  void OnDestroy()
  {
    Level.onStart -= OnLevelStart;
    Level.onFinished -= OnLevelFinished;
  }

  string LevelStr(int levelIdx)
  {
    return string.Format("level_{0:D2}", levelIdx); //.ToString("D2"));
  }
  void OnLevelStart(Level level)
  {
  #if UNITY_EDITOR
    Debug.Log("BB OnStart:"  + LevelStr(level.locationIdx));
  #endif
    ByteBrew.NewProgressionEvent(ByteBrewProgressionTypes.Started, LevelStr(level.locationIdx), "", level.locationIdx);
  }
  void OnLevelFinished(Level level) => OnLevelSuccess(level);
  void OnLevelSuccess(Level level)
  {
  #if UNITY_EDITOR
    Debug.Log("BB OnSuccess:" + LevelStr(level.locationIdx));
  #endif
    ByteBrew.NewProgressionEvent(ByteBrewProgressionTypes.Completed, LevelStr(level.locationIdx),"", level.locationIdx);
  }
  void OnLevelFail(Level level)
  {
  #if UNITY_EDITOR
    Debug.Log("BB OnFail:" + LevelStr(level.locationIdx));
  #endif
    ByteBrew.NewProgressionEvent(ByteBrewProgressionTypes.Failed, LevelStr(level.locationIdx), "", level.locationIdx);
  }
}
#endif