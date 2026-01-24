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

    public float WheelPaddingX => _wheelPaddingX;
    public float WheelPaddingZ => _wheelPaddingZ;
    public float SpringRestLength => _springRestLength;
    public float SpringStrength => _springStrength;
    public float SpringDamper => _springDamper;
}
