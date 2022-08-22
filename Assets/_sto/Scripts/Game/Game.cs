using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib;
using GameLib.InputSystem;

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
    TouchInputData.onInputUpdated -= OnInputMov;
    TouchInputData.onInputEnded -= OnInputEnd;

    Earth.onLevelStart -= ShowLevel;
  }
  IEnumerator Start()
  {
    yield return new WaitForSeconds(0.0125f);
    _earth.Setup();
  }

  void OnInputTapped(TouchInputData tid)
  {
    //_level?.OnInputTapped(tid);
  }
  void OnInputBeg(TouchInputData tid)
  {
    _level?.OnInputBeg(tid);
    _earth?.OnInputBeg(tid);
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
  public void ShowEarth(bool show_next)
  {
    StartCoroutine(coShowEarth(show_next));
  }
  private IEnumerator coShowEarth(bool show_next)
  {
    yield return new WaitForSeconds(0.5f);
    _level.Hide();
    _uiFade.FadeIn();
    yield return new WaitForSeconds(0.5f);
    _camCtrl.SetTo(2);
    DestroyLevel();
    _earth.Show(GameState.Progress.levelIdx, show_next);
    yield return new WaitForSeconds(0.5f);
    _uiFade.FadeOut();
    _camCtrl.SwitchTo(1);
  }
  public void ShowLevel(int levelIdx)
  {
    GameState.Progress.levelIdx = levelIdx;
    StartCoroutine(coShowLevel(levelIdx));
  }
  IEnumerator coShowLevel(int levelIdx)
  {
    _camCtrl.SwitchTo(2);
    _uiFade.FadeIn();
    yield return new WaitForSeconds(0.5f);
    _earth.Hide();
    CreateLevel(levelIdx);
    _uiFade.FadeOut();
    _camCtrl.SwitchTo(0);    
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
