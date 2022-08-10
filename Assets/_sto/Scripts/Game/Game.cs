using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib;
using GameLib.InputSystem;

public class Game : MonoBehaviour
{
  [SerializeField] Transform levelsContainer;

  public static System.Action<Level> onLevelRestart;

  Level _level = null;
  [SerializeField] Earth _earth = null;
  [SerializeField] ActivatableObject _actObj = null;

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
    _earth.Hide();
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
    StartCoroutine(coDestroyLevel());
  }
  IEnumerator coDestroyLevel()
  {
    _level.Hide();
    yield return null;
    while(_level.InTransiton)
      yield return null;
    if(_level)
      Destroy(_level.gameObject);
    _level = null;
  }
  public void HideLevelShowEarth()
  {
    //StartCoroutine(coHideLevelShowEarth());
    this.Invoke(() => DestroyLevel(), 0.35f);
    this.Invoke(() => 
    {
      NextLevel(false); 
      _earth.Show(GameState.Progress.levelIdx);
    },0.6f);
  }
  IEnumerator coHideLevelShowEarth()
  {
    yield return new WaitForSeconds(0.35f);
    yield return coDestroyLevel();
    _earth.Show(GameState.Progress.levelIdx);
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
  }
#endif
}
