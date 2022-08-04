using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
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

        if (raycastHit.collider == null) return;

        var level = raycastHit.collider.GetComponentInParent<LevelEarth>();
        if (level)
        {
            Select(level);
            var angle = GetAngleToSelectLevel(level);
            _rotation.RotateToSelectLevel(angle);
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
    }

    private float GetAngleToSelectLevel(LevelEarth levelEarth)
    {
        _vec1 = _camera.transform.position - transform.position;
        _vec1 = Vector3.Normalize(Vector3.ProjectOnPlane(_vec1, Vector3.up));
        _vec2 = levelEarth.ModelTransform.position - transform.position;
        _vec2.Normalize();
        _vec2 = Vector3.Normalize(Vector3.ProjectOnPlane(_vec2, Vector3.up));
        var angle = Vector3.SignedAngle(_vec1, _vec2, Vector3.up);
        return angle;
    }
}