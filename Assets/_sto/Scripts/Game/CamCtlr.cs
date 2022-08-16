using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib.CameraSystem;

public class CamCtlr : MonoBehaviour
{
  VirtualCameraRig _virtualCamRig;
  CameraRig[]      _camRigs;

  void Awake()
  {
    _virtualCamRig = FindObjectOfType<VirtualCameraRig>(true);
    _camRigs = _virtualCamRig.GetComponentsInChildren<CameraRig>(true);
  }

  public void SwitchTo(int camRigIdx)
  {
    _virtualCamRig.SwitchRig(_camRigs[camRigIdx]);
  }
  public void SetTo(int camRigIdx)
  {
    _virtualCamRig.SetRig(_camRigs[camRigIdx]);
  }
}
