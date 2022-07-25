using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib.Utilities;

public class Pipes : MonoBehaviour
{
  [SerializeField] Transform[] _pipes;
  [SerializeField] Transform[] _pollutions;
  //[SerializeField] ObjectSpin _spin;

  Vector3[][] _pipePaths = null;
  float[] _initialPollution = new float[] { 1, 1 };
  float _pollutionRateCur = 1.0f;
  float _pollutionRateDst = 1.0f;
  
  void Awake()
  {
    _pipePaths = new Vector3[_pipes.Length][];
    _initialPollution[0] = _pollutions[0].localScale.x;
    _initialPollution[1] = _pollutions[1].localScale.x;

    for(int p = 0; p < _pipes.Length; ++p)
    {
      _pipePaths[p] = new Vector3[_pipes[p].childCount];
      for(int q = 0; q < _pipePaths[p].Length; ++q)
        _pipePaths[p][q] = _pipes[p].GetChild(q).transform.position;
    }    
  }
  public void PollutionRate(float pr)
  {
    _pollutionRateDst = pr;    
  }
  public Vector3[] GetPath(int pipe_idx) => _pipePaths[pipe_idx];

  void Pollutions()
  {
    for(int q = 0; q < _pollutions.Length; ++q)
    {
      var poll = _pollutions[q];
      _pollutionRateCur = Mathf.MoveTowards(_pollutionRateCur, _pollutionRateDst, Time.deltaTime * 4);
      poll.localScale = new Vector3(poll.localScale.x, poll.localScale.y, _pollutionRateCur * _initialPollution[q]);
      if(_pollutionRateCur <= 0)
        poll.gameObject.SetActive(false);
    }
  }

  void Update()
  {
    Pollutions();
  }
}
