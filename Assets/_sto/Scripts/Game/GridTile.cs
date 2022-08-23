using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{
  [SerializeField] GameObject _dirty;
  [SerializeField] GameObject _clear;
  [SerializeField] ParticleSystem _ps;

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
