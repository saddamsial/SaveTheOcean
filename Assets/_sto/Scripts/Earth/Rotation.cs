using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] private float _speedRotation = 5f; 
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Rotate();
        }
    }

    private void Rotate()
    {
        var rotationY = _speedRotation * Input.GetAxis("Mouse X") * Time.deltaTime; 
        var rotation = new Vector3(0, - rotationY, 0); 
        transform.Rotate(rotation);
    }
}
