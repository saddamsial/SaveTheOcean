using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody; 
    [SerializeField] private float _speedRotation = 5f; 
    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            Rotate();
        }
    }

    private void Rotate()
    {
        var rotationY = _speedRotation * Input.GetAxis("Mouse X") * Time.deltaTime;
        _rigidbody.AddTorque(Vector3.up * -rotationY);
    }
}
