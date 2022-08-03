using System.Collections;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody; 
    [SerializeField] private float _speedRotation = 5f;
    [SerializeField] private float _speedRotationToSelectLevel = 20f;
    [SerializeField] private float _offsetRotationToSelectLevel = 1f; 

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
        
        var targetAngle = Quaternion.Euler(0, -angle, 0); 
        var _thisTransform = transform; 
        
        Debug.Log("Start Coroutine: " + _thisTransform.rotation.y + "  " + targetAngle.y);
        while ( -(_thisTransform.rotation.y - _offsetRotationToSelectLevel)  <= targetAngle.y ||
               -(_thisTransform.rotation.y + _offsetRotationToSelectLevel) >= targetAngle.y)
        {
            _thisTransform.rotation = Quaternion.Lerp(_thisTransform.rotation, targetAngle,
                Time.deltaTime * _speedRotationToSelectLevel);
            yield return null; 
        }
        Debug.Log("Stop Coroutine: " + _thisTransform.rotation.y + "  " + targetAngle.y);
        
        _rigidbody.isKinematic = false; 
        _isRotateToSelectLevel = false; 
    }
}
