using System;
using UnityEngine;

public enum StateLevel {Lock, Unlock, Passed}

public class LevelEarth : MonoBehaviour
{
    [SerializeField] private GameObject _liningGameObject;
    [SerializeField] private int _indexLevel;
    [SerializeField] private Transform _modelTransform;
    [SerializeField] private StateLevel _stateLevel = StateLevel.Lock;

    private Renderer _rendererModel; 
    public Transform ModelTransform => _modelTransform; 
    public GameObject LiningGameObject => _liningGameObject;
    public int IndexLevel => _indexLevel;
    public StateLevel StateLevel => _stateLevel;

    private void Awake()
    {
        _rendererModel = _modelTransform.GetComponent<Renderer>();
    }

    public void SetStateLevel(StateLevel state)
    {
        switch (state)
        {
            case StateLevel.Lock:
                _rendererModel.material.color = Color.black;
                break;
            case StateLevel.Unlock:
                _rendererModel.material.color = Color.green;
                break;
            case StateLevel.Passed:
                _rendererModel.material.color = Color.blue;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _stateLevel = state; 
    }
}
