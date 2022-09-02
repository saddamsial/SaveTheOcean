using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib;
using GameLib.InputSystem;
using Cinemachine;

public class Game : MonoBehaviour
{
  [SerializeField] Transform levelsContainer;
  [SerializeField] CamCtlr _camCtrl = null;

  public static System.Action<Level> onLevelRestart;

  Level   _level = null;
  Earth   _earth = null;
  UIFade  _uiFade = null;

	void Awake()
  {
    TouchInputData.onTap += OnInputTapped;
    TouchInputData.onInputStarted += OnInputBeg;
    TouchInputData.onInputMoveStarted += OnInputStart;
    TouchInputData.onInputUpdated += OnInputMov;
    TouchInputData.onInputEnded += OnInputEnd;

    Earth.onLevelStart += ShowLevel;

    _earth = FindObjectOfType<Earth>(true);
    _uiFade = FindObjectOfType<UIFade>(true);
  }
  void OnDestroy()
  {
    TouchInputData.onTap -= OnInputTapped;
    TouchInputData.onInputStarted -= OnInputBeg;
    TouchInputData.onInputMoveStarted -= OnInputStart;
    TouchInputData.onInputUpdated -= OnInputMov;
    TouchInputData.onInputEnded -= OnInputEnd;

    Earth.onLevelStart -= ShowLevel;
  }
  IEnumerator Start()
  {
    yield return new WaitForSeconds(0.0125f);
    _earth.Setup();
    FindObjectOfType<UIStatusBar>(true).Show();
  }

  void OnInputTapped(TouchInputData tid)
  {
    _level?.OnInputTapped(tid);
  }
  void OnInputBeg(TouchInputData tid)
  {
    _earth?.OnInputBeg(tid);
  }
  void OnInputStart(TouchInputData tid)
  {
    _level?.OnInputBeg(tid);
  }
  void OnInputMov(TouchInputData tid)
  {
    _level?.OnInputMov(tid);
    _earth?.OnInputMov(tid);
  }
  void OnInputEnd(TouchInputData tid)
  {
    _level?.OnInputEnd(tid);
    _earth?.OnInputEnd(tid);
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
    if(_level)
      _level.End();
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
    {
      _level.End();
      Destroy(_level.gameObject);
    }
    _level = null;
  }
  public void ShowEarth(bool show_next)
  {
    StartCoroutine(coShowEarth(show_next));
  }
  private IEnumerator coShowEarth(bool show_next)
  {
    _level?.Hide();
    _uiFade.FadeIn(_camCtrl.zoomSpeed * 2.0f);
    yield return new WaitForSeconds(0.5f);
    DestroyLevel();
    _earth.Show(GameState.Progress.levelIdx, show_next);
    _camCtrl.SwitchToGlobe();
    yield return new WaitForSeconds(1.0f);
    _uiFade.FadeOut(_camCtrl.zoomSpeed * 2f);
    _camCtrl.ZoomOut();
  }
  public void ShowLevel(int levelIdx)
  {
    GameState.Progress.levelIdx = levelIdx;
    StartCoroutine(coShowLevel(levelIdx));
  }
  IEnumerator coShowLevel(int levelIdx)
  {
    _camCtrl.ZoomIn();
    yield return new WaitForSeconds(0.5f);
    _uiFade.FadeIn(_camCtrl.zoomSpeed * 2.0f);
    yield return new WaitForSeconds(0.75f);
    _earth.Hide();
    CreateLevel(levelIdx);
    _camCtrl.SwitchToIngame();
    yield return new WaitForSeconds(1.0f);
    _uiFade.FadeOut(_camCtrl.zoomSpeed * 2);
  }

  void Update()
  {
    GameState.Process();

#if UNITY_EDITOR
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
#endif    
  }
}
