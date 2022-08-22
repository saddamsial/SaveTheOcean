using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vessel : MonoBehaviour
{
  [SerializeField] float _flyAlt = 1.1f;

  Vector3     _vdst;
 
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

  public void Init(Vector3 locationPos)
  {
    vpos = locationPos.normalized * _flyAlt;
    vrot = Quaternion.LookRotation(vpos) * Quaternion.AngleAxis(90, Vector3.right);
    _vdst = vpos;
  }
  public void FlyTo(Vector3 vdest)
  {
    _vdst = vdest.normalized * _flyAlt;
  }

  void Fly()
  {
    if(Vector3.Distance(vpos, _vdst) > 0.01f)
    {
      var vprev = vpos;
      vpos = Vector3.Slerp(vpos, _vdst, Time.deltaTime);
      vrot = Quaternion.Lerp(vrot, Quaternion.LookRotation(vpos - vprev, vpos), Time.deltaTime * 6);
    }
  }
  void Update()
  {
    Fly();
  }
}
