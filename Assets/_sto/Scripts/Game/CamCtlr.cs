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

  [SerializeField] float _zoomSpeed = 2;
  
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
    _camIngame.Priority = 30;
    _camTransit.Priority = 20;
    _camGlobe.Priority = 10;
  }

  public float zoomSpeed {get => _zoomSpeed; set => _zoomSpeed = value;}
}
