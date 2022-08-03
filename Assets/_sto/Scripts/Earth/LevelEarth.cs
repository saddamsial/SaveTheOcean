using System;
using UnityEngine;

public class LevelEarth : MonoBehaviour
{
    [SerializeField] private GameObject _liningGameObject;
    [SerializeField] private int _indexLevel;
    [SerializeField] private Transform _modelTransform;
    private float _angleY;

    public Transform ModelTransform => _modelTransform; 
    public GameObject LiningGameObject => _liningGameObject;
    public int IndexLevel => _indexLevel;
    public float AngleY => _angleY;

    private void Start()
    {
        _angleY = transform.localEulerAngles.y; 
        Debug.Log(gameObject.name + " " + _angleY);
    }
}
