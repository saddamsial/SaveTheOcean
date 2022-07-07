using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib;

public class GridElem : MonoBehaviour
{
  [SerializeField] ActivatableObject _actObj;
  
  Vector2Int _grid;

  public Vector2Int vgrid {get => _grid; set{_grid = value;}}

  public void Show()
  {
    _actObj.ActivateObject();
  }
  public void Hide()
  {
    _actObj.DeactivateObject();
  }
}
