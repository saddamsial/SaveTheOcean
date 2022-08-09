using System;
using UnityEngine;

public enum StateLevel {Lock, Unlock, Passed}

public class LevelEarth : MonoBehaviour
{
    [SerializeField] private GameObject _liningGameObject;
   /* [SerializeField] */ private int _indexLevel;
    [SerializeField] private Transform _modelTransform;
    [SerializeField] private StateLevel _stateLevel = StateLevel.Passed;
    [SerializeField] private ModelsLevelEarth _modelsLevelEarth; 
    public Transform ModelTransform => _modelTransform; 
    public GameObject LiningGameObject => _liningGameObject;
    public int IndexLevel => _indexLevel;
    public StateLevel StateLevel => _stateLevel;

    public void SetStateLevel(int index, StateLevel state)
    {
        foreach (var models in _modelsLevelEarth.GameObjectsModelsLevel)
            models.gameObject.SetActive(false);

        _modelsLevelEarth.GameObjectsModelsLevel[(int)state].SetActive(true);
        _indexLevel = index; 
        _stateLevel = state; 
    }
}
