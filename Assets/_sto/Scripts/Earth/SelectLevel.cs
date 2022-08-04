using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectLevel : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Rotation _rotation;
    private LevelEarth _currentLevelEarth;
    private Vector3 _vec1;
    private Vector3 _vec2;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            RaycastStart();
    }

    private void RaycastStart()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out var raycastHit); 
        
        if(raycastHit.collider == null) return;

        var level = raycastHit.collider.GetComponentInParent<LevelEarth>();
        if (level)
        {
            Select(level);
            Rotate(level); 
        }
           
    }

    private void Select(LevelEarth levelEarth)
    {
        if (_currentLevelEarth == null)
        {
            _currentLevelEarth = levelEarth; 
            _currentLevelEarth.LiningGameObject.SetActive(true);
        }
        else if (levelEarth != _currentLevelEarth)
        {
            _currentLevelEarth.LiningGameObject.SetActive(false);
            _currentLevelEarth = levelEarth; 
            _currentLevelEarth.LiningGameObject.SetActive(true);
        }
        
     //   Debug.Log("Select the Level: " + _currentLevelEarth.IndexLevel);
    //    _rotation.RotateToSelectLevel(_currentLevelEarth.AngleY); 
    }

    private void Rotate(LevelEarth levelEarth)
    {
        Debug.Log("position cylindr: " + levelEarth.ModelTransform.position);
        _vec1 = _camera.transform.position - transform.position;
        _vec1 = Vector3.Normalize(Vector3.ProjectOnPlane(_vec1, Vector3.up));
        Debug.Log(" _vec1: " + _vec1);
        _vec2 = levelEarth.ModelTransform.position - transform.position;
        _vec2.Normalize();
        _vec2 = Vector3.Normalize(Vector3.ProjectOnPlane(_vec2, Vector3.up));
        Debug.Log("_vec2: " + _vec2);
        var angle = Vector3.SignedAngle(_vec1, _vec2, Vector3.up); 
        Debug.Log("angle: " + angle);
        transform.RotateAround(transform.position,Vector3.down, angle);
    }
}
