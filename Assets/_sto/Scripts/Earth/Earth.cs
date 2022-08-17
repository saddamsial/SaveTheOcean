using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib.InputSystem;

public class Earth : MonoBehaviour
{
  [SerializeField] GameObject   _earthPrefab;
  [SerializeField] LevelEarth[] _levels;


  public static System.Action<int> onShow;
  public static System.Action onHide;
  public static System.Action<int> onLevelStart, onLevelSelected;

  int _selectedLevel = 0;

  void Awake()
  {
    for(int q = 0; q < _levels.Length; ++q)
      _levels[q].Init(q, GameState.Progress.Levels.GetLevelState(q));

    UIEarth.onBtnPlay += OnBtnPlay;
  }
  void OnDestroy()
  {
    UIEarth.onBtnPlay -= OnBtnPlay;
  }

  public void Show(int indexLevel)
  {
    SelectLevel(indexLevel);
    _earthPrefab.SetActive(true);

    UpdateLevelsStates();

    onShow?.Invoke(indexLevel);
  }
  public void Hide()
  {
    onHide?.Invoke();
    _earthPrefab.SetActive(false);
  }
  
  public void OnInputBeg(TouchInputData tid)
  {
    if(!_earthPrefab.activeSelf)
      return;

    if(tid.HoveredCollider)
    {
      var levelEarth = tid.HoveredCollider.GetComponentInParent<LevelEarth>();
      if(levelEarth)
        SelectLevel(levelEarth);
    }      
  }
  public void OnInputMov(TouchInputData tid)
  {
    if(!_earthPrefab.activeSelf)
      return;
    // if(tid.HoveredCollider)
    // {
    //   var levelEarth = tid.HoveredCollider.GetComponentInParent<LevelEarth>();
    //   if(levelEarth)
    //     levelEarth.Select(true);
    // }
  }  
  public void OnInputEnd(TouchInputData tid)
  {
    if(!_earthPrefab.activeSelf)
      return;
  }
  void OnBtnPlay()
  {
    onLevelStart?.Invoke(_selectedLevel);
  }

  void UpdateLevelsStates()
  {
    for(int q = 0; q < _levels.Length; ++q)
      _levels[q].state = GameState.Progress.Levels.GetLevelState(q);
  }

  void SelectLevel(LevelEarth lvlEarth) => SelectLevel(lvlEarth.idx);
  void SelectLevel(int levelIdx)
  {
    if(_selectedLevel >=0)
      _levels[_selectedLevel].Select(false);
    _levels[levelIdx].Select(true);
    _selectedLevel = levelIdx;

    onLevelSelected?.Invoke(levelIdx);
  }
}
