using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{
  [SerializeField] GameObject _dirty;
  [SerializeField] GameObject _clear;
  [SerializeField] ParticleSystem _ps;

  Vector2 _vgrid = Vector2.zero;

  public Vector2 vgrid{get => _vgrid; set => _vgrid = value;}

  public void set(bool act)
  {
    _dirty.SetActive(act);
    _clear.SetActive(!act);
    if(act)
      this.Invoke(() => this._ps.Play(), 1);
    else
      _ps.Stop();
  }
}
