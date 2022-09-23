using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib.InputSystem;

[DefaultExecutionOrder(-1)]
public class Earth : MonoBehaviour
{
  [Header("Refs")]
  [SerializeField] GameObject     _earthPrefab;
  [SerializeField] GameObject     _locationsPath;
  [SerializeField] Transform      _levelsContainer;
  [SerializeField] Transform      _locationsContainer;
  [SerializeField] Transform      _fx;
  [SerializeField] Transform      _extras;

  [Header("Earth fx")]
  [SerializeField] EarthFx      _earthFx;

  [Header("Vessel")]
  [SerializeField] Vessel _vessel;

  [Header("Rotate params")]
  [SerializeField] float _rotateDragDegrees = 180.0f;
  [SerializeField] float _rotateMax = 720;
  [SerializeField] float _rotateToLocationSpeed = 5.0f;
  [SerializeField] float _rotateDamping = 0;
  [SerializeField] float _rotateXRange = 45;

  public static System.Action<int> onShow;
  public static System.Action onHide;
  public static System.Action<int> onLevelStart, onLevelSelected;

  int            _selectedLocation = 0;
  //float          _rotateSpeed = 0;
  Vector2        _vrotateSpeed = Vector2.zero;
  Vector2?       _vdragBeg = null;
  Vector2        _vdragPrev;
  bool           _move2location = false;
  Location[]     _locations;

  public static int locationsCnt {get; private set;}

  void Awake()
  {
    InitLocations();
    GameState.Progress.Locations.onLocationPolluted += OnLocationPolluted;
    UIEarth.onBtnPlay += OnBtnPlay;
    UIEarth.onBtnFeed += OnBtnFeed;
  }
  void OnDestroy()
  {
    UIEarth.onBtnPlay -= OnBtnPlay;
    UIEarth.onBtnFeed -= OnBtnFeed;
    GameState.Progress.Locations.onLocationPolluted -= OnLocationPolluted;
  }

  private void InitLocations()
  {
    List<Location> listLocations = new List<Location>();
    for(int q = 0; q < _levelsContainer.childCount; ++q)
    {
      var levelTransf = _levelsContainer.GetChild(q);
      if(levelTransf.gameObject.activeSelf)
      {
        var loc = GameData.Prefabs.CreateLocation(_locationsContainer);
        loc.Init(listLocations.Count, levelTransf, _rotateXRange, GameState.Progress.Locations.GetLocationState(listLocations.Count));
        listLocations.Add(loc);
      }
    }
    _locations = listLocations.ToArray();
    locationsCnt = _locations.Length;
  }
  public void Setup()
  {
    _selectedLocation = GameState.Progress.locationIdx;
    //SelectLocation(_selectedLocation);
    if(_selectedLocation < 0)
    {
      SelectLocation(0);
      _vessel.Init(Vector3.up);
      _vessel.transform.localRotation = Quaternion.AngleAxis(0, _vessel.vpos - _earthPrefab.transform.position);
      _fx.transform.localRotation = _locations[_selectedLocation].localDstRoto;
      this.Invoke(()=>MoveVesselToLocation(_selectedLocation), 0.5f);
    }
    else
    {
      SelectLocation(_selectedLocation);
      _vessel.Init(_locations[_selectedLocation].transform.localPosition);
      _fx.transform.localRotation = _locations[_selectedLocation].localDstRoto;
    }
    _earthPrefab.SetActive(true);
    UpdateLevelsStates();
    onShow?.Invoke(_selectedLocation);
  }
  public void Show(int indexLocation, bool show_next)
  {
    SelectLocation(indexLocation);
    _fx.gameObject.SetActive(true);
    _extras.gameObject.SetActive(true);

    _locationsPath.SetActive(!GameState.Progress.Locations.AllStateFinished());

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
    _fx.gameObject.SetActive(false);
    _extras.gameObject.SetActive(false);
  }
  public int GetLevel(int locationIdx) => _locations[locationIdx].levelIdx;

  public void OnInputBeg(TouchInputData tid)
  {
    if(!_earthPrefab.activeInHierarchy)
      return;

    _vdragBeg = tid.InputPosition;
    _vdragPrev = _vdragBeg.Value;
    _vrotateSpeed = Vector2.zero;
    _move2location = false;
  }
  public void OnInputMov(TouchInputData tid)
  {
    if(!_earthPrefab.activeInHierarchy)
      return;

    if(_vdragBeg != null)
    {
      var moveDist = tid.InputPosition - _vdragPrev;
      _vrotateSpeed.x = Mathf.Clamp(moveDist.x * _rotateDragDegrees, -_rotateMax, _rotateMax);
      _vrotateSpeed.y = Mathf.Clamp(moveDist.y * _rotateDragDegrees, -_rotateMax, _rotateMax);
      _vdragPrev = tid.InputPosition;
    }
  }  
  public void OnInputEnd(TouchInputData tid)
  {
    if(!_earthPrefab.activeInHierarchy)
      return;

    if(tid.HoveredCollider && Mathf.Abs(tid.InputPosition.x - _vdragBeg.Value.x) < 0.05f)
    {
      var location = tid.HoveredCollider.GetComponentInParent<Location>();
      if(location && location.transform.position.z < 0)
      {
        if(location.state >= Level.State.Unlocked) // && location.state != Level.State.Finished)
        {
          SelectLocation(location);
          StartRotateToLocation(location);
          MoveVesselToLocation(location.idx);
        }
      }
    }
    _vdragBeg = null;
  }
  void OnBtnPlay()
  {
    Level.mode = Level.Mode.Clearing;
    onLevelStart?.Invoke(_selectedLocation);
  }
  void OnBtnFeed()
  {
    Level.mode = Level.Mode.Feeding;
    onLevelStart?.Invoke(_selectedLocation);
  }
  void OnLocationPolluted(int locationIdx)
  {
    _locations[locationIdx].state = GameState.Progress.Locations.GetLocationState(locationIdx);
  }  
  void UpdateLevelsStates()
  {
    for(int q = 0; q < _locations.Length; ++q)
      _locations[q].state = GameState.Progress.Locations.GetLocationState(q);
  }

  void SelectLocation(Location location) => SelectLocation(location.idx);
  void SelectLocation(int location)
  {
    if(_selectedLocation >=0)
      _locations[_selectedLocation].Select(false);
    _locations[location].Select(true);
    _selectedLocation = location;
    GameState.Progress.locationIdx = location;
    onLevelSelected?.Invoke(location);
  }
  void MoveVesselToLocation(int location)
  {
    _vessel.FlyTo(_locations[location].transform.localPosition);
  }
  int  GetNextLocation(int location)
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
      _vrotateSpeed = Vector2.zero;
      var rotDest = _locations[_selectedLocation].localDstRoto;
      _fx.transform.localRotation = Quaternion.Slerp(_fx.transform.localRotation, rotDest, _rotateToLocationSpeed * Time.deltaTime);
      if(Mathf.Abs(Quaternion.Angle(_fx.transform.localRotation, rotDest)) < 0.01f)
        _move2location = false;
    }
  }
  void RotateFree()
  {
    _vrotateSpeed *= Mathf.Pow(_rotateDamping, TimeEx.deltaTimeFrame);
    float angle = Vector3.SignedAngle(Vector3.ProjectOnPlane(_fx.transform.localRotation * Vector3.up, Vector3.right), Vector3.up, Vector3.right);
    if(angle <= -_rotateXRange)
    {
      if(_vrotateSpeed.y >= 0)
        _vrotateSpeed.y = -0.1f;
    }
    else if(angle >= _rotateXRange)
    {
      if(_vrotateSpeed.y <= 0)
        _vrotateSpeed.y = 0.1f;
    }
    var qpreR = _fx.transform.localRotation;
    _fx.transform.localRotation = Quaternion.AngleAxis(_vrotateSpeed.y, Vector3.right) * _fx.transform.localRotation;
    _fx.transform.localRotation *= Quaternion.AngleAxis(-_vrotateSpeed.x, Vector3.up);

    _earthFx?.RotoSpeed(_vrotateSpeed.x);
    //_earthFx.RotoParams(_fx.transform.localRotation, _vrotateSpeed.magnitude);
  }

  void Update()
  {
    RotateToLocation();
    RotateFree();
  }
}
