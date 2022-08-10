using System.Collections;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;

    [Header("Speed rotation")] [SerializeField]
    private float _forceRotation = 100f;

    [SerializeField] private float _speedRotationTransform = 16f;
    [SerializeField] private float _speedRotationToSelectLevel = 20f;
    [Space] [SerializeField] private float _maxAngularVelocity = 14f;

    private bool _isRotateToSelectLevel;
    private float _forceRotationY;
 //   private bool _isMouseDown = false;
 //   private IEnumerator _enumeratorStopRotationPhysics;

    private void Awake()
    {
        _rigidbody.centerOfMass = Vector3.zero;
        _rigidbody.maxAngularVelocity = _maxAngularVelocity;
    }

    private void Update()
    {
        if (IsButtonMouseDown())
        {
            ResetForceAndVelocityRotation();
            _rigidbody.isKinematic = true;
            /*  if (_enumeratorStopRotationPhysics != null)
                  StopCoroutine(_enumeratorStopRotationPhysics);
              _enumeratorStopRotationPhysics = StopRotationPhysic();
              StartCoroutine(_enumeratorStopRotationPhysics);*/
        }

        if (IsMouseButton())
        {
            TransformRotation();
        }

        if (IsButtonMouseUp())
        {
            _rigidbody.isKinematic = false;
            RotatePhysics();
        }
    }

    private static bool IsMouseButton() => Input.GetMouseButton(0) || Input.touchCount > 0;

    private bool IsButtonMouseDown() => Input.GetMouseButtonDown(0) ||
                                        (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

    private bool IsButtonMouseUp() => Input.GetMouseButtonUp(0) || (Input.touchCount > 0 &&
                                                                    Input.GetTouch(0).phase is TouchPhase.Canceled
                                                                        or TouchPhase.Ended);


    /*   private void FixedUpdate()
       {
         if((Input.GetMouseButton(0) || Input.touchCount > 0) && !_isRotateToSelectLevel)
         {
           RotatePhysics();
         }
       }*/

    private void TransformRotation()
    {
        var positionMouse = Input.GetAxis("Mouse X");
        transform.RotateAround(transform.position, -transform.up,
            Time.deltaTime * _speedRotationTransform * positionMouse);
    }

    /*   private IEnumerator StopRotationPhysic()
       {
           var timer = 0f;
           while (timer < 0.08f)
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
       }*/

    private void RotatePhysics()
    {
        var mouseX = Input.GetAxis("Mouse X");

    //    HandlerForceAndVelocityRotation(mouseX);
        _forceRotationY += _forceRotation * mouseX * Time.deltaTime;
        _rigidbody.AddTorque(transform.up * -_forceRotationY);
    }

 /*   private void HandlerForceAndVelocityRotation(float mouseX)
    {
        if ((mouseX > 0 && _rigidbody.angularVelocity.y > 0) ||
            (mouseX < 0 && _rigidbody.angularVelocity.y < 0)
           )
            ResetForceAndVelocityRotation();

        if (_rigidbody.angularVelocity.y is < 0.1f and > -0.1f)
            ResetForceRotation();
    }*/

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

        _isRotateToSelectLevel = false;
        _rigidbody.isKinematic = false;
    }

    private void ResetForceAndVelocityRotation()
    {
        ResetForceRotation();
        _rigidbody.angularVelocity = Vector3.zero;
    }

    private void ResetForceRotation() => _forceRotationY = 0;
    public void SetSpeedRotationTransform(float speed) => _speedRotationTransform = speed;
    public void SetForceRotation(float force) => _forceRotation = force;
    public void SetMaxAngularVelocity(float velocity) => _rigidbody.maxAngularVelocity = velocity;
    public void SetAngularDrag(float drag) => _rigidbody.angularDrag = drag;
    
    public float GetSpeedRotation() => _speedRotationTransform;
    public float GetForceRotation() => _forceRotation;
    public float GetMaxAngularVelocity() => _rigidbody.maxAngularVelocity;
    public float GetAngularDrag() => _rigidbody.angularDrag;
}