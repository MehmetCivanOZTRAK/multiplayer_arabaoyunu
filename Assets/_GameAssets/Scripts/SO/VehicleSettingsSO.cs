using UnityEngine;

[CreateAssetMenu(fileName = "VehicleSettings", menuName = "ScriptableObjects/VehicleSettings", order = 1)]
public class VehicleSettingsSO : ScriptableObject
{
    [Header("Wheel Settings")]
    [SerializeField] private float _wheelPaddingX;
    [SerializeField] private float _wheelPaddingZ;

    [Header("Suspension Settings")]
    [SerializeField] private float _springRestLength;
    [SerializeField] private float _springStrength;
    [SerializeField] private float _springDamper;

    [Header("Handling Settings")]
    [SerializeField] private float _steerAngle;
    [SerializeField] private float _frontWheelsGripFactor;
    [SerializeField] private float _backWheelsGripFactor;
    [Header("Body Settings")]
    [SerializeField] private float _tireMass;

    
    [Header("Power Settings")]
    [SerializeField] private float _acceleratepower;
    [SerializeField] private float _maxForwardSpeed;
    [SerializeField] private float _maxBackwardSpeed;
    [SerializeField] private float _brakesPower;
    [Header("Air Resistance")]
    [SerializeField] private float _airResistance;

    public float WheelPaddingX => _wheelPaddingX;
    public float WheelPaddingZ => _wheelPaddingZ;
    public float SpringRestLength => _springRestLength;
    public float SpringStrength => _springStrength;
    public float SpringDamper => _springDamper;
    public float SteerAngle => _steerAngle;
    public float FrontWheelsGripFactor => _frontWheelsGripFactor;
    public float BackWheelsGripFactor => _backWheelsGripFactor;
    public float TireMass => _tireMass;
    public float AcceleratePower => _acceleratepower;
    public float MaxForwardSpeed => _maxForwardSpeed;
    public float MaxBackwardSpeed => _maxBackwardSpeed;
    public float BrakesPower => _brakesPower;
    public float AirResistance => _airResistance;

}
