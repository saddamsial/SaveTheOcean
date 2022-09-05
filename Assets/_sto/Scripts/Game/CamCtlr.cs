using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamCtlr : MonoBehaviour
{
  [SerializeField] CinemachineVirtualCamera _camGlobe;
  [SerializeField] CinemachineVirtualCamera _camIngame;
  [SerializeField] CinemachineVirtualCamera _camTransit;

  [SerializeField] float _zoomMin  = 1.0f;
  [SerializeField] float _zoomMax = 3.0f;
  [SerializeField] float _zoomSpeed = 2;
  
  float           _defFov = 30;
  float           _zoom = 1.0f;
  float           _zoomTo = 1.0f;

  void Awake()
  {
    //_camIngame.enabled = true;
    //_camGlobe.enabled = true;
    _defFov = _camGlobe.m_Lens.FieldOfView;
  }
  public void SwitchToGlobe()
  {
    _camIngame.Priority = 10;
    _camTransit.Priority = 20;
    _camGlobe.Priority = 30;
  }
  public void SwitchToTransit()
  {
    _camIngame.Priority = 10;
    _camGlobe.Priority = 20;
    _camTransit.Priority = 30;    
  }
  public void SwitchToIngame()
  {
    //_camIngame.enabled = true;
    _camIngame.Priority = 30;
    _camTransit.Priority = 20;
    _camGlobe.Priority = 10;
  }

  public void  ZoomIn() => _zoomTo = _zoomMin;
  public void  ZoomOut() => _zoomTo = _zoomMax;
  public float zoomSpeed {get => _zoomSpeed; set => _zoomSpeed = value;}

  void Update()
  {
    //_zoom = Mathf.Lerp(_zoom, _zoomTo, Time.deltaTime * _zoomSpeed);
    //_camGlobe.m_Lens.FieldOfView = _defFov * _zoom;
  }
}
