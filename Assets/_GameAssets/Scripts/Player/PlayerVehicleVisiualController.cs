using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerVehicleVisiualController : MonoBehaviour
{
    [SerializeField] private PlayerVehicleController _playerVehicleController;
    [SerializeField] private Transform _wheelFL, _wheelFR, _wheelBL, _wheelBR;
    [SerializeField] private float _wheelSpinSpeed, _wheelYWhenSpinMax, _wheelYWhenSpinMin;

    private Quaternion _wheelFLRoll, _wheelFRRoll;

    private float _springRestLenght;
    private float _forwardSpeed;
    private float _steerInput;
    private float _steerAngle;

    private Dictionary<WheelType, float> _springsCurrentLenght = new()
    {
      {WheelType.FrontLeft,0f },
      {WheelType.FrontRight,0f },
      {WheelType.BackLeft,0f },
      {WheelType.BackRight,0f }
    };

    private void Update()
    {
        UpdateVisiualStates();
        RotateWheels();
        SetSuspension();
    }
    private void Start()
    {
        _springRestLenght = _playerVehicleController.Settings.SpringRestLength;
        _steerAngle = _playerVehicleController.Settings.SteerAngle;
        _wheelFLRoll = _wheelFL.localRotation;
        _wheelFRRoll = _wheelFR.localRotation;

    }

    private void UpdateVisiualStates()
    {
        _steerInput = Input.GetAxis("Horizontal");
        _forwardSpeed = Vector3.Dot(_playerVehicleController.Velocity, _playerVehicleController.Forward);

        _springsCurrentLenght[WheelType.FrontLeft] = _playerVehicleController.GetSpringCurrentLenght(WheelType.FrontLeft);
        _springsCurrentLenght[WheelType.FrontRight] = _playerVehicleController.GetSpringCurrentLenght(WheelType.FrontRight);
        _springsCurrentLenght[WheelType.BackLeft] = _playerVehicleController.GetSpringCurrentLenght(WheelType.BackLeft);
        _springsCurrentLenght[WheelType.BackRight] = _playerVehicleController.GetSpringCurrentLenght(WheelType.BackRight);


    }
    private void RotateWheels()
    {
        if (_springsCurrentLenght[WheelType.FrontLeft] < _springRestLenght)
        {
            _wheelFLRoll *= Quaternion.AngleAxis(_forwardSpeed * _wheelSpinSpeed * Time.deltaTime, Vector3.right);
        }

        if (_springsCurrentLenght[WheelType.FrontRight] < _springRestLenght)
        {
            _wheelFRRoll *= Quaternion.AngleAxis(_forwardSpeed * _wheelSpinSpeed * Time.deltaTime, Vector3.right);
        }

        if (_springsCurrentLenght[WheelType.BackLeft] < _springRestLenght)
        {
            _wheelBL.localRotation *= Quaternion.AngleAxis(_forwardSpeed * _wheelSpinSpeed * Time.deltaTime, Vector3.right);
        }

        if (_springsCurrentLenght[WheelType.BackRight] < _springRestLenght)
        {
            _wheelBR.localRotation *= Quaternion.AngleAxis(_forwardSpeed * _wheelSpinSpeed * Time.deltaTime, Vector3.right);
        }

        _wheelFL.localRotation = Quaternion.AngleAxis(_steerInput * _steerAngle, Vector3.up) * _wheelFLRoll;
        _wheelFR.localRotation = Quaternion.AngleAxis(_steerInput * _steerAngle, Vector3.up) * _wheelFRRoll;
    }
    private void SetSuspension()
    {
        float _springFLRatio = _springsCurrentLenght[WheelType.FrontLeft] / _springRestLenght;
        float _springFRRatio = _springsCurrentLenght[WheelType.FrontRight] / _springRestLenght;
        float _springBLRatio = _springsCurrentLenght[WheelType.BackLeft] / _springRestLenght;
        float _springBRRatio = _springsCurrentLenght[WheelType.BackRight] / _springRestLenght;

        _wheelFL.localPosition = new Vector3(_wheelFL.localPosition.x, _wheelYWhenSpinMin +
        (_wheelYWhenSpinMax - _wheelYWhenSpinMin) * _springFLRatio, _wheelFL.localPosition.z);
        _wheelFR.localPosition = new Vector3(_wheelFR.localPosition.x, _wheelYWhenSpinMin +
         (_wheelYWhenSpinMax - _wheelYWhenSpinMin) * _springFRRatio, _wheelFR.localPosition.z);
        _wheelBL.localPosition = new Vector3(_wheelBL.localPosition.x, _wheelYWhenSpinMin +
        (_wheelYWhenSpinMax - _wheelYWhenSpinMin) * _springBLRatio, _wheelBL.localPosition.z);
        _wheelBR.localPosition = new Vector3(_wheelBR.localPosition.x, _wheelYWhenSpinMin +
        (_wheelYWhenSpinMax - _wheelYWhenSpinMin) * _springBRRatio, _wheelBR.localPosition.z);
    }
}
