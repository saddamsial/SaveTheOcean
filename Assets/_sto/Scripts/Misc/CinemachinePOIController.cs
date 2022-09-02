using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib.CameraSystem;
using Cinemachine;

[RequireComponent(typeof(CinemachineTargetGroup))]
[DefaultExecutionOrder(-10)]
public class CinemachinePOIController : MonoBehaviour
{
    CinemachineTargetGroup targetGroup = null;

    private void Awake() {
        targetGroup = GetComponent<CinemachineTargetGroup>();
    }
    private void OnEnable() {
        CameraPOI.onActivated += RegisterPOI;
        CameraPOI.onDeactivated += UnregisterPOI;
    }
    private void OnDisable() {
        CameraPOI.onActivated -= RegisterPOI;
        CameraPOI.onDeactivated -= UnregisterPOI;        
    }

    void RegisterPOI(CameraPOI sender){
        targetGroup.AddMember(sender.transform, 1f,0f);
    }
    void UnregisterPOI(CameraPOI sender){
        targetGroup.RemoveMember(sender.transform);
    }

}
