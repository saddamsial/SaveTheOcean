using System;
using UnityEngine;

public class LevelEarth : MonoBehaviour
{
    [SerializeField] private GameObject _liningGameObject;
    [SerializeField] private int _indexLevel;

    private float _angleY; 
    
    public GameObject LiningGameObject => _liningGameObject;
    public int IndexLevel => _indexLevel;
    public float AngleY => _angleY;

    private void Start()
    {
        _angleY = transform.localEulerAngles.y; 
    }
}
