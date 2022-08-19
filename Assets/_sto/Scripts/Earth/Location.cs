using System;
using UnityEngine;

public class Location : MonoBehaviour
{
  [SerializeField] GameObject[] _stateModels;
  [SerializeField] GameObject   _markerModel;
  [SerializeField] GameObject   _selectionModel;
  [SerializeField] Level.State  _state = Level.State.Locked;
  [SerializeField] Transform    _modelTransf;


  Quaternion _localDstRoto = Quaternion.identity;
  private int _idx = -1;

  public Quaternion localDstRoto => _localDstRoto;
  public int  idx => _idx;

  public void Init(int idx, Transform levelTransf, Level.State level_state)
  { 
    _idx = idx;
    state = level_state;

    transform.localPosition = levelTransf.localPosition;
    transform.localRotation = Quaternion.LookRotation(-levelTransf.localPosition) * Quaternion.AngleAxis(-90, Vector3.right);
    var posxz = transform.localPosition;
    posxz.y = 0;
    _localDstRoto = Quaternion.AngleAxis(Vector3.SignedAngle(posxz, -Vector3.forward, Vector3.up), Vector3.up);

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
    _selectionModel.SetActive(false);
  }

  int  State2MI(Level.State state) => (int)state;
  void SetStateModel(Level.State state)
  {
    int mi = State2MI(state);
    for(int q = 0; q < _stateModels.Length; ++q)
      _stateModels[q].SetActive(q == mi);
  }
}
