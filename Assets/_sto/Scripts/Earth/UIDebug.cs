using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDebug : MonoBehaviour
{
    [SerializeField] private Rotation _rotationEarth; 
    [Header("Speed Rotation")]
    [SerializeField] private TextMeshProUGUI _speedRotationText;
    [SerializeField] private Slider _speedRotationSlider;
    [Header("Max angular Velocity")]
    [SerializeField] private TextMeshProUGUI _maxAngularVelocityText;
    [SerializeField] private Slider _maxAngularVelocitySlider;
    [Header("Angular Drag")]
    [SerializeField] private TextMeshProUGUI _angularDragText;
    [SerializeField] private Slider _angularDragSlider;

    public void SetSpeedRotation(float speed)
    {
        _speedRotationSlider.value = speed; 
        _speedRotationText.text = "Speed Rotation:" + speed; 
        _rotationEarth.SetSpeedRotation(speed);
    }
    
    public void SetMaxAngularVelocity(float value)
    {
        _maxAngularVelocitySlider.value = value; 
        _maxAngularVelocityText.text = "Max Angular Velocity:" + value; 
        _rotationEarth.SetMaxAngularVelocity(value);
    }
    
    public void SetAngularDrag(float value)
    {
        _angularDragSlider.value = value; 
        _angularDragText.text = "Angular Drag:" + value.ToString("F1"); 
        _rotationEarth.SetAngularDrag(value);
    }
}
