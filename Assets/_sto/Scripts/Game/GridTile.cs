using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{
  [SerializeField] GameObject _dirty;
  [SerializeField] GameObject _clear;

  public void set(bool act)
  {
    _dirty.SetActive(act);
    _clear.SetActive(!act);
  }
}
