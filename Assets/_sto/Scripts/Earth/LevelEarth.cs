using System;
using UnityEngine;

public class LevelEarth : MonoBehaviour
{

  [SerializeField] GameObject[] _stateModels;
  [SerializeField] GameObject   _markerModel;
  [SerializeField] GameObject   _selectionModel;
  [SerializeField] Level.State  _state = Level.State.Locked;

  private int _idx = -1;


  public int  idx => _idx;
  public void Init(int idx, Level.State level_state)
  { 
    _idx = idx;
    state = level_state;
    Select(false);
  }
  public Level.State state 
  {
    get => _state;
    set
    {
      _state = value;
      SetStateModel(_state);
    }
  }
  public void Select(bool sel)
  {
    _markerModel.SetActive(sel);
    _selectionModel.SetActive(sel);
  }

  int  State2MI(Level.State state) => (int)state;
  void SetStateModel(Level.State state)
  {
    int mi = State2MI(state);
    for(int q = 0; q < _stateModels.Length; ++q)
      _stateModels[q].SetActive(q == mi);
  }
}
