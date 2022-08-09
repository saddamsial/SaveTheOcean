using System.Collections;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _speedRotation = 5f;
    [SerializeField] private float _speedRotationToSelectLevel = 20f;
    [SerializeField] private float _maxAngularVelocity = 14f;

    private bool _isRotateToSelectLevel;
    private float _forceRotationY;
    private bool _isMouseDown = false;
    private IEnumerator _enumeratorStopRotation;

    private void Start()
    {
        _rigidbody.centerOfMass = Vector3.zero;
        _rigidbody.maxAngularVelocity = _maxAngularVelocity;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_enumeratorStopRotation != null)
                StopCoroutine(_enumeratorStopRotation);
            _enumeratorStopRotation = StopRotationPhysic();
            StartCoroutine(_enumeratorStopRotation);
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && !_isRotateToSelectLevel)
        {
            Rotate();
        }
    }

    private IEnumerator StopRotationPhysic()
    {
        var timer = 0f;

        while (timer < 0.1f)
        {
            timer += Time.deltaTime;
            var mouseX = Input.GetAxis("Mouse X");
            if (mouseX is > 0.1f or < -0.1f)
            {
                yield break;
            }

            yield return null;
        }

        ResetForceAndVelocityRotation();
    }

    private void Rotate()
    {
        var mouseX = Input.GetAxis("Mouse X");

        HandlerForceAndVelocityRotation(mouseX);

        _forceRotationY += _speedRotation * mouseX * Time.deltaTime;
        _rigidbody.AddTorque(transform.up * -_forceRotationY);
    }

    private void HandlerForceAndVelocityRotation(float mouseX)
    {
        if ((mouseX > 0 && _rigidbody.angularVelocity.y > 0) ||
            (mouseX < 0 && _rigidbody.angularVelocity.y < 0)
           )
            ResetForceAndVelocityRotation();

        if (_rigidbody.angularVelocity.y is < 0.1f and > -0.1f)
            ResetForceRotation();
    }

    public void RotateToSelectLevel(float angle)
    {
        StopAllCoroutines();
        StartCoroutine(RotateToSelectLevelCoroutine(angle));
    }


    private IEnumerator RotateToSelectLevelCoroutine(float angle)
    {
        ResetForceAndVelocityRotation();
        _isRotateToSelectLevel = true;
        _rigidbody.isKinematic = true;

        var mult = angle > 0 ? 1 : -1;
        angle = Mathf.Abs(angle);
        while (angle > 0)
        {
            yield return null;
            angle -= Time.deltaTime * _speedRotationToSelectLevel;
            transform.RotateAround(transform.position, -transform.up,
                Time.deltaTime * mult * _speedRotationToSelectLevel);
        }

        _rigidbody.isKinematic = false;
        _isRotateToSelectLevel = false;
    }

    private void ResetForceAndVelocityRotation()
    {
        ResetForceRotation();
        _rigidbody.angularVelocity = Vector3.zero;
    }

    private void ResetForceRotation() => _forceRotationY = 0;

    public void SetSpeedRotation(float speed) => _speedRotation = speed;

    public void SetMaxAngularVelocity(float velocity) => _rigidbody.maxAngularVelocity = velocity;

    public void SetAngularDrag(float drag) => _rigidbody.angularDrag = drag;
}