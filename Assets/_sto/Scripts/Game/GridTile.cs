using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{
  [SerializeField] SpriteRenderer _sr;
  [SerializeField] Color _colorAct;
  public void set(bool act)
  {
    _sr.material.color = (act)? _colorAct : Color.clear;
  }
}
