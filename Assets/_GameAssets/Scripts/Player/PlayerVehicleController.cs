using System;
using System.Collections.Generic;

using UnityEngine;


public class PlayerVehicleController : MonoBehaviour
{
    public class SpringData
    {
        public float _currentLength;
        public float _currentVelocity;
    }

    private static readonly WheelType[] _wheelType = new WheelType[]
    {
        WheelType.FrontLeft,
        WheelType.FrontRight,
        WheelType.BackLeft,
        WheelType.BackRight,
    };

    [Header("References")]

    [SerializeField] private VehicleSettingsSO vehicleSettings;
    [SerializeField] private Rigidbody vehicleRigidbody;
    [SerializeField] private BoxCollider vehicleCollider;
    private float _steerInput;
    private float _accelerateInput;

    private Dictionary<WheelType, SpringData> _springDatas = new Dictionary<WheelType, SpringData>();

    private void Awake()
    {

        foreach (var wheel in _wheelType)
        {
            _springDatas.Add(wheel, new SpringData());
        }

    }
    private void Update()
    {
        SetSteerInput(Input.GetAxis("Horizontal"));
        SetAcclerateInput(Input.GetAxis("Vertical"));
    }
    private void FixedUpdate()
    {
        UpdateSuspension();
        UpdateSteering();
        UpdateAcceleration();
    }

    private void SetSteerInput(float steerInput)
    {
        _steerInput = Mathf.Clamp(steerInput, -1f, 1f);
    }
    private void SetAcclerateInput(float accelerateInput)
    {
        _accelerateInput = Mathf.Clamp(accelerateInput, -1f, 1f);
    }

    private void UpdateSuspension()
    {
        foreach (var id in _springDatas.Keys)
        {
            CastSpring(id);
            float currentLength = _springDatas[id]._currentLength;
            float currentVelocity = _springDatas[id]._currentVelocity;

            float force = SpringMathExtensions.CalculateForceDamped(currentLength, currentVelocity, vehicleSettings.SpringRestLength,
             vehicleSettings.SpringStrength, vehicleSettings.SpringDamper);

            vehicleRigidbody.AddForceAtPosition(force * transform.up, GetSpringPosition(id));

        }
    }
    private void UpdateSteering()
    {
        foreach (WheelType wheel in _wheelType)
        {
            if (!IsGrounded(wheel))
            {
                continue;
            }
            Vector3 springposition = GetSpringPosition(wheel);
            Vector3 slideDirection = GetWheelSlideDirection(wheel);
            float slideVelocity = Vector3.Dot(slideDirection, vehicleRigidbody.GetPointVelocity(springposition));

            float desiredChangevelocity = GetWheelGripFactor(wheel) * -slideVelocity;
            float desiredAcceleration = desiredChangevelocity / Time.fixedDeltaTime;
            Vector3 force = desiredAcceleration * slideDirection * vehicleSettings.TireMass;
            vehicleRigidbody.AddForceAtPosition(force, GetTorquePosition(wheel));

        }
    }
    private void UpdateAcceleration()
    {

        if(Mathf.Approximately(_accelerateInput, 0f))
        {
            return;
        }

        float accelerateforce = Vector3.Dot(transform.forward, vehicleRigidbody.linearVelocity);

        bool movingForward = accelerateforce > 0f;
        float speed = Mathf.Abs(accelerateforce);
        if(movingForward && speed > vehicleSettings.MaxForwardSpeed)
        {
            return;
        }
        else if(!movingForward && speed > vehicleSettings.MaxBackwardSpeed)
        {
            return;
        }

        foreach (WheelType wheel in _wheelType)
        {
            if (!IsGrounded(wheel))
            {
                continue;
            }
            Vector3 position = GetTorquePosition(wheel);
            Vector3 wheelForward = GetWheelRollDirection(wheel);
            vehicleRigidbody.AddForceAtPosition(wheelForward * _accelerateInput * vehicleSettings.AcceleratePower, position);

        }
    }

    private void CastSpring(WheelType wheelType)
    {
        Vector3 position = GetSpringPosition(wheelType);

        float previousLength = _springDatas[wheelType]._currentLength;
        float currentLength;



        if (Physics.Raycast(position, -transform.up, out var hit, vehicleSettings.SpringRestLength))
        {
            currentLength = hit.distance;
        }
        else
        {
            currentLength = vehicleSettings.SpringRestLength;
        }
        float velocity = (currentLength - previousLength) / Time.fixedDeltaTime;
        _springDatas[wheelType]._currentLength = currentLength;
        _springDatas[wheelType]._currentVelocity = velocity;


    }

    private Vector3 GetSpringPosition(WheelType wheelType)
    {
        return transform.localToWorldMatrix.MultiplyPoint3x4(GetSpringRelativePosition(wheelType));
    }
    private Vector3 GetSpringRelativePosition(WheelType wheelType)
    {
        Vector3 boxsize = vehicleCollider.size;
        float boxcenter = boxsize.y * -0.5f;
        float paddingX = vehicleSettings.WheelPaddingX;
        float paddingZ = vehicleSettings.WheelPaddingZ;

        return wheelType switch
        {
            WheelType.FrontLeft => new Vector3(boxsize.x * (paddingX - 0.5f), boxcenter, boxsize.z * (0.5f - paddingZ)),
            WheelType.FrontRight => new Vector3(boxsize.x * (0.5f - paddingX), boxcenter, boxsize.z * (0.5f - paddingZ)),
            WheelType.BackLeft => new Vector3(boxsize.x * (paddingX - 0.5f), boxcenter, boxsize.z * (paddingZ - 0.5f)),
            WheelType.BackRight => new Vector3(boxsize.x * (0.5f - paddingX), boxcenter, boxsize.z * (paddingZ - 0.5f)),
            _ => default,
        };
    }
    private Vector3 GetTorquePosition(WheelType wheelType)
    {
        return transform.localToWorldMatrix.MultiplyPoint3x4(GetTorqueRelativePosition(wheelType));
    }
    private Vector3 GetTorqueRelativePosition(WheelType wheelType)
    {
        Vector3 boxsize = vehicleCollider.size;
        
        float paddingX = vehicleSettings.WheelPaddingX;
        float paddingZ = vehicleSettings.WheelPaddingZ;

        return wheelType switch
        {
            WheelType.FrontLeft => new Vector3(boxsize.x * (paddingX - 0.5f), 0f, boxsize.z * (0.5f - paddingZ)),
            WheelType.FrontRight => new Vector3(boxsize.x * (0.5f - paddingX), 0f, boxsize.z * (0.5f - paddingZ)),
            WheelType.BackLeft => new Vector3(boxsize.x * (paddingX - 0.5f), 0f, boxsize.z * (paddingZ - 0.5f)),
            WheelType.BackRight => new Vector3(boxsize.x * (0.5f - paddingX), 0f, boxsize.z * (paddingZ - 0.5f)),
            _ => default,
        };
    }
    private Vector3 GetWheelSlideDirection(WheelType wheelType)
    {
        Vector3 forward = GetWheelRollDirection(wheelType);
        return Vector3.Cross(forward, transform.up);
    }
    private Vector3 GetWheelRollDirection(WheelType wheelType)
    {
        bool _frontWheel = wheelType == WheelType.FrontLeft || wheelType == WheelType.FrontRight;
        if (_frontWheel)
        {
            var _steerQuaternion = Quaternion.AngleAxis(_steerInput * vehicleSettings.SteerAngle, transform.up);
            return _steerQuaternion * transform.forward;
        }
        else
        {
            return transform.forward;
        }
    }
    private float GetWheelGripFactor(WheelType wheelType)
    {
        bool _frontWheel = wheelType == WheelType.FrontLeft || wheelType == WheelType.FrontRight;
        return _frontWheel ? vehicleSettings.FrontWheelsGripFactor : vehicleSettings.BackWheelsGripFactor;
    }
    private bool IsGrounded(WheelType wheelType)
    {
        return _springDatas[wheelType]._currentLength < vehicleSettings.SpringRestLength;
    }



}
public static class SpringMathExtensions
{
    public static float CalculateForceDamped(float currentLength, float lenghtVelocity, float restLength,
     float strenght, float damper)
    {
        float lenghtOffset = restLength - currentLength;
        return (lenghtOffset * strenght) - (lenghtVelocity * damper);
    }
}
