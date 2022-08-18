using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib.InputSystem;

public class Earth : MonoBehaviour
{
  [SerializeField] GameObject   _earthPrefab;
  [SerializeField] Location[]   _locations;
  [SerializeField] Transform    _fx;


  public static System.Action<int> onShow;
  public static System.Action onHide;
  public static System.Action<int> onLevelStart, onLevelSelected;

  int _selectedLocation = 0;

  [SerializeField] float _rotateDragDegrees = 180.0f;
  [SerializeField] float _rotateMax = 720;
  [SerializeField] float _rotateToLocationSpeed = 90.0f;
  [SerializeField] float _rotateDamping = 0;

  float       _rotateSpeed = 0;
  Vector2?    _vdragBeg = null;
  Vector2     _vdragPrev;
  bool        _move2location = false;

  void Awake()
  {
    for(int q = 0; q < _locations.Length; ++q)
      _locations[q].Init(q, GameState.Progress.Levels.GetLevelState(q));

    UIEarth.onBtnPlay += OnBtnPlay;
  }
  void OnDestroy()
  {
    UIEarth.onBtnPlay -= OnBtnPlay;
  }

  public void Show(int indexLevel)
  {
    SelectLocation(indexLevel);
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
    _move2location = false;
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
      var levelEarth = tid.HoveredCollider.GetComponentInParent<Location>();
      if(levelEarth)
      {
        SelectLocation(levelEarth);
        StartRotateToLocation(levelEarth);
      }
    }
    _vdragBeg = null;
  }
  void OnBtnPlay()
  {
    onLevelStart?.Invoke(_selectedLocation);
  }

  void UpdateLevelsStates()
  {
    for(int q = 0; q < _locations.Length; ++q)
      _locations[q].state = GameState.Progress.Levels.GetLevelState(q);
  }

  void SelectLocation(Location location) => SelectLocation(location.idx);
  void SelectLocation(int location)
  {
    if(_selectedLocation >=0)
      _locations[_selectedLocation].Select(false);
    _locations[location].Select(true);
    _selectedLocation = location;

    onLevelSelected?.Invoke(location);
  }

  void StartRotateToLocation(Location location)
  {
    _move2location = true;
  }
  void RotateToLocation()
  {
    if(_move2location)
    {
      _rotateSpeed = 0;
      var rotDest = _locations[_selectedLocation].localDstRoto;
      _fx.transform.localRotation = Quaternion.Lerp(_fx.transform.localRotation, rotDest, _rotateToLocationSpeed * Time.deltaTime);
      if(Mathf.Abs(Quaternion.Angle(_fx.transform.localRotation, rotDest)) < 0.1f)
        _move2location = false;
    }
  }

  void Update()
  {
    RotateToLocation();
    _rotateSpeed *= Mathf.Pow(_rotateDamping, Time.deltaTime / 0.016666f);
    _fx.transform.localRotation *= Quaternion.AngleAxis(-_rotateSpeed, Vector3.up);
  }
}
