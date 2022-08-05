using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib.InputSystem;

public class Game : MonoBehaviour
{
  [SerializeField] Transform levelsContainer;

  public static System.Action<Level> onLevelRestart;

  Level _level = null;
  [SerializeField] Earth _earth = null;

	void Awake()
  {
    TouchInputData.onTap += OnInputTapped;
    TouchInputData.onInputStarted += OnInputBeg;
    TouchInputData.onInputUpdated += OnInputMov;
    TouchInputData.onInputEnded += OnInputEnd;

    Earth.onLevelStart += CreateLevel;
  }
  void OnDestroy()
  {
    TouchInputData.onTap -= OnInputTapped;
    TouchInputData.onInputStarted -= OnInputBeg;
    TouchInputData.onInputUpdated -= OnInputMov;
    TouchInputData.onInputEnded -= OnInputEnd;

    Earth.onLevelStart -= CreateLevel;
  }
  IEnumerator Start()
  {
    yield return new WaitForSeconds(0.125f);
    _earth.Show(GameState.Progress.levelIdx);
  }

  void OnInputTapped(TouchInputData tid)
  {
    //_level?.OnInputTapped(tid);
  }
  void OnInputBeg(TouchInputData tid)
  {
    _level?.OnInputBeg(tid);
  }
  void OnInputMov(TouchInputData tid)
  {
    _level?.OnInputMov(tid);
  }
  void OnInputEnd(TouchInputData tid)
  {
    _level?.OnInputEnd(tid);
  }

  public void CreateLevel(int levelIdx)
  {
    if(_level)
      Destroy(_level.gameObject);
    _level = null;
    GameState.Progress.levelIdx = levelIdx;
    CreateLevel();
  }
  public void CreateLevel()
  {
    if(_level)
      Destroy(_level.gameObject);
    _level = null;  

    _level = GameData.Levels.CreateLevel(GameState.Progress.levelIdx, levelsContainer);
  }
  public void RestartLevel()
  {
    onLevelRestart?.Invoke(_level);
    CreateLevel();
  }
  public void PrevLevel(bool create = true)
  {
    GameState.Progress.levelIdx = GameData.Levels.PrevLevel(GameState.Progress.levelIdx);
    if(create)
      CreateLevel();
  }
  public void NextLevel(bool create = true)
  {
    GameState.Progress.levelIdx = GameData.Levels.NextLevel(GameState.Progress.levelIdx);
    if(create)
      CreateLevel();
  }
  public void DestroyLevel()
  {
    if(_level)
      Destroy(_level.gameObject);
    _level = null;  
  }
#if UNITY_EDITOR
  void Update()
  {
    if(Input.GetKeyDown(KeyCode.Z))
    {
      Level.onFinished?.Invoke(_level);
      PrevLevel();
    }
    else if(Input.GetKeyDown(KeyCode.X))
    {
      Level.onFinished?.Invoke(_level);
      NextLevel();
    }
    else if(Input.GetKeyDown(KeyCode.R))
    {
      Level.onFinished?.Invoke(_level);
      RestartLevel();
    }
    // else if(Input.GetKeyDown(KeyCode.E))
    //   level?.DbgFinishLevel();
  }
#endif
}
