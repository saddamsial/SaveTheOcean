using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib.InputSystem;

public class Earth : MonoBehaviour
{
  [Header("Refs")]
  [SerializeField] GameObject   _earthPrefab;
  [SerializeField] Transform    _levelsContainer;
  [SerializeField] Transform    _locationsContainer;
  [SerializeField] Location[]   _locations;
  [SerializeField] Transform    _fx;

  [Header("Earth fx")]
  [SerializeField] EarthFx      _earthFx;

  [Header("Vessel")]
  [SerializeField] Vessel _vessel;

  [Header("Rotate params")]
  [SerializeField] float _rotateDragDegrees = 180.0f;
  [SerializeField] float _rotateMax = 720;
  [SerializeField] float _rotateToLocationSpeed = 5.0f;
  [SerializeField] float _rotateDamping = 0;


  public static System.Action<int> onShow;
  public static System.Action onHide;
  public static System.Action<int> onLevelStart, onLevelSelected;

  int         _selectedLocation = 0;
  float       _rotateSpeed = 0;
  Vector2?    _vdragBeg = null;
  Vector2     _vdragPrev;
  bool        _move2location = false;

  void Awake()
  {
    InitLocations();
    UIEarth.onBtnPlay += OnBtnPlay;
  }
  void OnDestroy()
  {
    UIEarth.onBtnPlay -= OnBtnPlay;
  }

  private void InitLocations()
  {
    _locations = new Location[_levelsContainer.childCount];
    for(int q = 0; q < _levelsContainer.childCount; ++q)
    {
      var levelTransf = _levelsContainer.GetChild(q);
      _locations[q] = GameData.Prefabs.CreateLocation(_locationsContainer);
      _locations[q].Init(q, levelTransf, GameState.Progress.Levels.GetLevelState(q));
    }
  }

  public void Setup()
  {
    _selectedLocation = GameState.Progress.levelIdx;
    SelectLocation(_selectedLocation);
    _vessel.transform.position = _locations[_selectedLocation].transform.position;
    _earthPrefab.SetActive(true);
    _fx.transform.localRotation = _locations[_selectedLocation].localDstRoto;
    UpdateLevelsStates();
    onShow?.Invoke(_selectedLocation);
  }
  public void Show(int indexLocation, bool show_next)
  {
    SelectLocation(indexLocation);
    _earthPrefab.SetActive(true);

    UpdateLevelsStates();
    int location_idx = (show_next)? GetNextLocation(indexLocation) : indexLocation;      
    this.Invoke(()=> 
    {
      SelectLocation(location_idx); 
      StartRotateToLocation(_locations[location_idx]);
      if(show_next)
        MoveVesselToLocation(location_idx);

      onShow?.Invoke(location_idx);
    }, 1.0f);
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
      var location = tid.HoveredCollider.GetComponentInParent<Location>();
      if(location)
      {
        SelectLocation(location);
        StartRotateToLocation(location);
        MoveVesselToLocation(location.idx);
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
    GameState.Progress.levelIdx = location;
    onLevelSelected?.Invoke(location);
  }
  void MoveVesselToLocation(int location)
  {
    _vessel.FlyTo(_locations[location].transform.localPosition);
  }
  int GetNextLocation(int location)
  {
    return Mathf.Clamp(location + 1, 0, _locations.Length - 1);
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
    _earthFx?.RotoSpeed(_rotateSpeed);
  }
}
