using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDebug : MonoBehaviour
{
    [SerializeField] private Rotation _rotationEarth; 
    [Header("Speed Rotation Transform")]
    [SerializeField] private TextMeshProUGUI _speedRotationText;
    [SerializeField] private Slider _speedRotationSlider;
    [Header("Force Rotation")]
    [SerializeField] private TextMeshProUGUI _forceRotationText;
    [SerializeField] private Slider _forceRotationSlider;
    [Header("Max angular Velocity")]
    [SerializeField] private TextMeshProUGUI _maxAngularVelocityText;
    [SerializeField] private Slider _maxAngularVelocitySlider;
    [Header("Angular Drag")]
    [SerializeField] private TextMeshProUGUI _angularDragText;
    [SerializeField] private Slider _angularDragSlider;

    private void Start()
    {
        SetSpeedRotation(_rotationEarth.GetSpeedRotation() / 10);
        SetForceRotation(_rotationEarth.GetForceRotation() / 1000);
        SetMaxAngularVelocity(_rotationEarth.GetMaxAngularVelocity());
        SetAngularDrag(_rotationEarth.GetAngularDrag());
    }

    public void SetSpeedRotation(float speed)
    {
        _speedRotationSlider.value = speed;
        var value = speed * 10; 
        _speedRotationText.text = "Speed Rotation Transform:" + value ; 
        _rotationEarth.SetSpeedRotationTransform(value);
    }
    
    public void SetForceRotation(float speed)
    {
        _forceRotationSlider.value = speed;
        var value = speed * 1000; 
        _forceRotationText.text = "Force Rotation:" + value ; 
        _rotationEarth.SetForceRotation(value);
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
