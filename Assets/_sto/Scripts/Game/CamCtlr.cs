using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib.CameraSystem;

public class CamCtlr : MonoBehaviour
{
  Earth _earth;
  VirtualCameraRig _virtualCamRig;
  CameraRig[]      _camRigs;
  
  [SerializeField] float _zoomMin  = 0.3f;
  [SerializeField] float _zoomMax = 1;
  [SerializeField] float _zoomSpeed = 2;

  float     _zoomTo = 1.0f;
  Vector3   _vzoom = Vector3.one;

  void Awake()
  {
    _virtualCamRig = FindObjectOfType<VirtualCameraRig>(true);
    _camRigs = _virtualCamRig.GetComponentsInChildren<CameraRig>(true);
    _earth = FindObjectOfType<Earth>(true);
  }

  public void SwitchTo(int camRigIdx)
  {
    _camRigs[camRigIdx].GetComponent<CameraAutoTracker>().RecalculateCamera();
    _virtualCamRig.SwitchRig(_camRigs[camRigIdx]);
    //_camRigs[1-camRigIdx].GetComponent<CameraAutoTracker>().enabled = false;
    //_camRigs[camRigIdx].GetComponent<CameraAutoTracker>().enabled = true;
  }
  public void SetTo(int camRigIdx)
  {
    _camRigs[camRigIdx].GetComponent<CameraAutoTracker>().RecalculateCamera();
    _virtualCamRig.SetRig(_camRigs[camRigIdx]);
    //_camRigs[1-camRigIdx].GetComponent<CameraAutoTracker>().enabled = false;
    //_camRigs[camRigIdx].GetComponent<CameraAutoTracker>().enabled = true;
  }
  public void  ZoomIn() => _zoomTo = _zoomMin;
  public void  ZoomOut() => _zoomTo = _zoomMax;
  public float zoomSpeed {get => _zoomSpeed; set => _zoomSpeed = value;}
  public float zoom {get => _vzoom.x ; set => _vzoom.x = _zoomTo = value;}

  void Update()
  {
    float s = Mathf.Lerp(_vzoom.x, _zoomTo, Time.deltaTime * _zoomSpeed);
    _vzoom.Set(s,s,s);
    _earth.zoom.localScale = _vzoom;
  }
}
