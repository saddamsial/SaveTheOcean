using System;
using System.Collections;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _speedRotation = 5f;
    [SerializeField] private float _speedRotationToSelectLevel = 20f;

    private bool _isRotateToSelectLevel;

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && !_isRotateToSelectLevel)
        {
            Rotate();
        }
    }

    private void Rotate()
    {
        var rotationY = _speedRotation * Input.GetAxis("Mouse X") * Time.deltaTime;
        _rigidbody.AddTorque(Vector3.up * -rotationY);
    }

    public void RotateToSelectLevel(float angle)
    {
        StopAllCoroutines();
        StartCoroutine(RotateToSelectLevelCoroutine(angle));
    }


    private IEnumerator RotateToSelectLevelCoroutine(float angle)
    {
        _isRotateToSelectLevel = true;
        _rigidbody.isKinematic = true;

        var mult = angle > 0 ? 1 : -1;
        angle = Mathf.Abs(angle);
        while (angle > 0)
        {
            yield return null;
            angle -= Time.deltaTime * _speedRotationToSelectLevel;
            transform.RotateAround(transform.position, Vector3.down,
                Time.deltaTime * mult * _speedRotationToSelectLevel);
        }

        _rigidbody.isKinematic = false;
        _isRotateToSelectLevel = false;
    }
}