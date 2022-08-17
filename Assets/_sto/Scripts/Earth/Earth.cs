using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib.InputSystem;

public class Earth : MonoBehaviour
{
  [SerializeField] GameObject   _earthPrefab;
  [SerializeField] LevelEarth[] _levels;
  [SerializeField] Transform    _fx;


  public static System.Action<int> onShow;
  public static System.Action onHide;
  public static System.Action<int> onLevelStart, onLevelSelected;

  int _selectedLevel = 0;

  [SerializeField] float _rotateDragDegrees = 180.0f;
  [SerializeField] float _rotateMax = 900;
  [SerializeField] float _rotateDamping = 0;

  float       _rotateSpeed = 0;
  Vector2?    _vdragBeg = null;
  Vector2     _vdragPrev;
  bool        _move2Lvl = false;

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

    _vdragBeg = tid.InputPosition;
    _vdragPrev = _vdragBeg.Value;
    _rotateSpeed = 0;
  }
  public void OnInputMov(TouchInputData tid)
  {
    if(!_earthPrefab.activeSelf)
      return;

    if(_vdragBeg != null)
    {
      float moveDist = tid.InputPosition.x - _vdragPrev.x;
      _rotateSpeed = Mathf.Clamp(moveDist * _rotateDragDegrees, -_rotateMax, _rotateMax);
      _vdragPrev = tid.InputPosition;
    }
  }  
  public void OnInputEnd(TouchInputData tid)
  {
    if(!_earthPrefab.activeSelf)
      return;

    if(tid.HoveredCollider && Mathf.Abs(tid.InputPosition.x - _vdragBeg.Value.x) < 0.05f)
    {
      var levelEarth = tid.HoveredCollider.GetComponentInParent<LevelEarth>();
      if(levelEarth)
      {
        SelectLevel(levelEarth);
        StartRotateToLevel(levelEarth);
      }
    }
    _vdragBeg = null;
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

  void StartRotateToLevel(LevelEarth level)
  {
    //level.get
  }
  void RotateToLevel()
  {

  }

  void Update()
  {
    RotateToLevel();
    _rotateSpeed *= Mathf.Pow(_rotateDamping, Time.deltaTime / 0.016666f);
    _fx.transform.localRotation *= Quaternion.AngleAxis(-_rotateSpeed, Vector3.up);
  }
}
