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

  Quaternion _rotateBeg = Quaternion.identity;
  public float _rotateDragDegrees = 180.0f;
  public float _rotateMax = 900;

  float _rotateAngle = 0;
  float _rotateSpeed = 0;
  public float _rotateDamping = 0;
  Vector2? _vdragBeg = null;
  List<Vector2> _vdragPrev = new List<Vector2>();

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
    _rotateBeg = _fx.transform.localRotation;

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

    _vdragPrev.Clear();
    _vdragBeg = tid.InputPosition;
    _vdragPrev.Add(_vdragBeg.Value);
    _rotateBeg = _fx.transform.localRotation;
    _rotateSpeed = 0;
  }
  public void OnInputMov(TouchInputData tid)
  {
    if(!_earthPrefab.activeSelf)
      return;

    if(_vdragBeg != null)
    {
      _rotateAngle = (tid.InputPosition.x - _vdragBeg.Value.x) * _rotateDragDegrees;
      _vdragPrev.Add(tid.InputPosition);
    }
  }  
  public void OnInputEnd(TouchInputData tid)
  {
    if(!_earthPrefab.activeSelf)
      return;

    if(tid.HoveredCollider && Mathf.Abs(tid.InputPosition.x - _vdragBeg.Value.x) < 0.1f)
    {
      var levelEarth = tid.HoveredCollider.GetComponentInParent<LevelEarth>();
      if(levelEarth)
        SelectLevel(levelEarth);
    }

    _vdragBeg = null;
    int prev_idx = (_vdragPrev.Count > 2) ? -2 : -1;
    float moveDist = tid.InputPosition.x - _vdragPrev.last(prev_idx).x;    
    float rotateAcc = 500 * moveDist / Mathf.Clamp(tid.InputTime, Time.deltaTime, Time.deltaTime * 20);
    _rotateSpeed += Mathf.Clamp(rotateAcc, -_rotateMax, _rotateMax);
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

  void Update()
  {
    if(_vdragBeg != null)
    {
      _fx.transform.localRotation = _rotateBeg * Quaternion.AngleAxis(-_rotateAngle, Vector2.up);
      //_fx.transform.localRotation = Quaternion.RotateTowards(_fx.transform.localRotation, _rotateBeg * Quaternion.AngleAxis(-_rotateAngle, Vector2.up), Time.deltaTime * 120);
    }
    else
    {
      _rotateSpeed *= Mathf.Pow(_rotateDamping, Time.deltaTime / 0.016666f);
      _fx.transform.localRotation *= Quaternion.AngleAxis(-_rotateSpeed * Time.deltaTime, Vector3.up);
    }
  }
}
