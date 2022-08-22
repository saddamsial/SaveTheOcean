using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vessel : MonoBehaviour
{
  [SerializeField] float _flyAlt = 1.1f;

  Vector3 _vdest;

  void Awake()
  {
    _vdest = vpos;
  }

  public Vector3  vpos
  { 
    get => transform.localPosition; 
    set => transform.localPosition = value;
  }
  public Quaternion vrot
  {
    get => transform.localRotation;
    set => transform.localRotation = value;
  }

  public void FlyTo(Vector3 vdest)
  {
    _vdest = vdest;
  }
  float rots = 1;
  void Fly()
  {
    var vprev = vpos;
    var v = Vector3.Lerp(vpos, _vdest, Time.deltaTime * rots);
    vpos = v.normalized * _flyAlt;
    var rotDst = Quaternion.LookRotation(vpos-vprev, vpos);
    var ang = Quaternion.Angle(vrot, rotDst);
    rots = ang < 10 ? 1-ang/10 : 0.1f;
    vrot = Quaternion.Lerp(vrot, rotDst, Time.deltaTime);
  }
  void Update()
  {
    Fly();
  }
}
