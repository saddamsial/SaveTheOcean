using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthFx : MonoBehaviour
{
  [SerializeField] Renderer   _globeRenderer;
  [SerializeField] Transform  _globeRotator;
  [SerializeField] float      _roto2SwayFactor = 10.0f;
  
  MaterialPropertyBlock _mpb = null;

  float _swayDst = 0.0f;
  float _sway = 0;
  Vector3 _swayAxis = Vector3.right;

  void Awake()
  {
    if(_mpb == null)
      _mpb = new MaterialPropertyBlock();
    _globeRenderer.SetPropertyBlock(_mpb);  
  }
  public void RotoSpeed(float roto_speed)
  {
    _swayDst = Mathf.Clamp(TimeEx.deltaTimeFrameInv * roto_speed / _roto2SwayFactor, -1, 1);
    if(Mathf.Approximately(_swayDst, 0))
      _swayDst = 0;
  }
  public void RotoParams(Quaternion q, float roto_speed)
  {
    float a = 0;
    q.ToAngleAxis(out a, out _swayAxis);
    _swayDst = Mathf.Clamp(TimeEx.deltaTimeFrameInv * roto_speed / _roto2SwayFactor, -1, 1);
    if(Mathf.Approximately(_swayDst, 0))
      _swayDst = 0;
  }  
  void Update()
  {
    _sway = Mathf.Lerp(_sway, _swayDst, Time.deltaTime * 5);
    _globeRenderer.GetPropertyBlock(_mpb);
    _mpb.SetFloat("_SwayMultiplier", _sway);
    //_mpb.SetVector("_SwayAxis", _swayAxis);
    _globeRenderer.SetPropertyBlock(_mpb);
  }
}
