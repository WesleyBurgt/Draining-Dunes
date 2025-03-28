using System;
using UnityEngine;

public class CarControl : MonoBehaviour
{
    [Header("Car Properties")]
    public float motorTorque = 2000f;
    public float brakeTorque = 2000f;
    public float maxSpeed = 20f;
    public float steeringRange = 30f;
    public float steeringRangeAtMaxSpeed = 10f;
    public float centreOfGravityOffset = -1f;
    public float antiRollValue = 1500f;
    public float FuelTankSize = 100f;
    public float Fuel = 100f;
    [Tooltip("0 is pure physics, 1 the car will grip in the direction it's facing.")]
    [SerializeField, Range(0, 1)] private float _steerHelper;

    private WheelControl[] wheels;
    private Rigidbody rigidBody;
    private CarInputActions carControls;
    private float oldRotation;

    [HideInInspector] public float CurrentSpeed { get { return rigidBody.linearVelocity.magnitude * 3.6f; } }
    [HideInInspector] public float damagePercentage = 0f;


    void Awake()
    {
        carControls = new CarInputActions();
    }
    void OnEnable()
    {
        carControls.Enable();
    }

    void OnDisable()
    {
        carControls.Disable();
    }

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        Vector3 centerOfMass = rigidBody.centerOfMass;
        centerOfMass.y += centreOfGravityOffset;
        rigidBody.centerOfMass = centerOfMass;

        wheels = GetComponentsInChildren<WheelControl>();
    }

    private float collisionSpeed;

    void OnCollisionEnter(Collision collision)
    {
        collisionSpeed = CurrentSpeed;
    }

    void OnCollisionExit(Collision collision)
    {
        float collisionReduction = Math.Abs(CurrentSpeed - collisionSpeed);
        damagePercentage += collisionReduction * 10;
        Debug.Log("collisionReduction: " + collisionReduction);
    }

    void FixedUpdate()
    {
        Vector2 inputVector = carControls.Car.Movement.ReadValue<Vector2>();

        float throttleInput = inputVector.y;
        float steeringInput = inputVector.x;

        float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.linearVelocity);
        float speedFactor = Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(forwardSpeed));

        bool canAccelerate = Fuel > 0 && damagePercentage < 100;

        float motorTorqueWithDamage = motorTorque * (100 - damagePercentage) / 100;
        float currentMotorTorque = Mathf.Lerp(motorTorqueWithDamage, 0, speedFactor);
        float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        bool isAccelerating = (Mathf.Sign(throttleInput) == Mathf.Sign(forwardSpeed)) && throttleInput != 0;
                
        SteerHelper();
        ApplyFuelUsage(isAccelerating, currentMotorTorque);

        foreach (var wheel in wheels)
        {
            if (wheel.steerable)
            {
                wheel.WheelCollider.steerAngle = steeringInput * currentSteerRange;
            }

            if (isAccelerating)
            {
                if (canAccelerate && wheel.motorized)
                {
                    wheel.WheelCollider.motorTorque = throttleInput * currentMotorTorque;
                }
                else
                {
                    wheel.WheelCollider.motorTorque = 0f;
                }
                wheel.WheelCollider.brakeTorque = 0f;
            }
            else
            {
                wheel.WheelCollider.motorTorque = 0f;
                wheel.WheelCollider.brakeTorque = Mathf.Abs(throttleInput) * brakeTorque;
            }
        }

        AntiRoll();
    }

    private void SteerHelper()
    {
        foreach (WheelControl wheel in wheels)
        {
            wheel.WheelCollider.GetGroundHit(out WheelHit wheelHit);
            if (wheelHit.normal == Vector3.zero)
                return;
        }

        if (Mathf.Abs(oldRotation - transform.eulerAngles.y) < 10f)
        {
            float turnAdjust = (transform.eulerAngles.y - oldRotation) * _steerHelper;
            Quaternion velRotation = Quaternion.AngleAxis(turnAdjust, Vector3.up);
            rigidBody.linearVelocity = velRotation * rigidBody.linearVelocity;
        }

        oldRotation = transform.eulerAngles.y;
    }

    private void AntiRoll()
    {
        float travelL = 1.0f;
        float travelR = 1.0f;
        foreach(WheelControl wheel in wheels)
        {
            WheelCollider wheelCollider = wheel.WheelCollider;
            bool grounded = wheelCollider.GetGroundHit(out WheelHit wheelHit);

            if (grounded)
            {
                float travel = (-wheelCollider.transform.InverseTransformPoint(wheelHit.point).y - wheelCollider.radius) / wheelCollider.suspensionDistance;
                if (wheel.leftSide)
                {
                    travelL += travel;
                }
                else
                {
                    travelR += travel;
                }
            }
        }

        float antiRollForce = (travelL - travelR) * antiRollValue;

        foreach(WheelControl wheel in wheels)
        {
            WheelCollider wheelCollider = wheel.WheelCollider;
            bool grounded = wheelCollider.GetGroundHit(out WheelHit wheelHit);

            if (grounded)
            {
                rigidBody.AddForceAtPosition(wheelCollider.transform.up * -antiRollForce, wheelCollider.transform.position);
            }
        }
    }

    void ApplyFuelUsage(bool isAccelerating, float currentMotorTorque)
    {
        if (Fuel > 0)
        {
            float baseFuelLoss = 0.01f;
            float damageFuelLossMultiplier = 0.0003f;
            float torqueFuelLossMultiplier = 0.00001f;

            float damageFuelLoss = damagePercentage * damageFuelLossMultiplier;
            float torqueFuelLoss = (motorTorque - currentMotorTorque) * torqueFuelLossMultiplier;

            float FuelUsage = baseFuelLoss + damageFuelLoss;
            if (isAccelerating)
            {
                FuelUsage +=  torqueFuelLoss;
            }
            Fuel -= FuelUsage;
        }
    }

    public void ResetFuel()
    {
        Fuel = FuelTankSize;
    }

    public void ResetDamage()
    {
        damagePercentage = 0;
    }
}
