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
  [SerializeField] CamCtlr _camCtrl = null;

  UIEarth _uiEarth;
  UIFade  _uiFade;

	void Awake()
  {
    TouchInputData.onTap += OnInputTapped;
    TouchInputData.onInputStarted += OnInputBeg;
    TouchInputData.onInputUpdated += OnInputMov;
    TouchInputData.onInputEnded += OnInputEnd;

    Earth.onLevelStart += ShowLevel;

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
  public void ShowEarth()
  {
    StartCoroutine(coShowEarth());
  }
  private IEnumerator coShowEarth()
  {
    yield return new WaitForSeconds(0.5f);
    _level.Hide();
    _uiFade.BlendTo(new Color(0,0,0,1));
    yield return new WaitForSeconds(0.5f);
    _camCtrl.SetTo(2);
    DestroyLevel();
    _earth.Show(GameState.Progress.levelIdx);
    yield return new WaitForSeconds(0.5f);
    _uiFade.BlendTo(new Color(0, 0, 0, 0));
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
    _uiFade.BlendTo(new Color(0, 0, 0, 1));
    yield return new WaitForSeconds(0.5f);
    _earth.Hide();
    CreateLevel(levelIdx);
    _uiFade.BlendTo(new Color(0, 0, 0, 0));
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
