using System;
using UnityEngine;

public class CarControl : MonoBehaviour
{
    [Header("Car Properties")]
    public float motorTorque = 2000f;
    public float brakeTorque = 2000f;
    public float handbrakeTorque = 5000f;
    public float maxSpeed = 20f;
    public float steeringRange = 30f;
    public float steeringRangeAtMaxSpeed = 10f;
    public Vector3 centerOfGravityOffset = new Vector3(0, -1, 0);
    public float antiRollValue = 1500f;
    [Tooltip("0 is pure physics, 1 the car will grip in the direction it's facing.")]
    [SerializeField, Range(0, 1)] private float _steerHelper;
    [HideInInspector] public float CurrentSpeed { get { return rigidBody.linearVelocity.magnitude * 3.6f; } }

    public int money = 0;
    [Range(0, 100)] public float damagePercentage = 0f;

    [Header("Fuel Properties")]
    public float FuelTankSize = 100f;
    public float Fuel = 100f;
    [SerializeField] private float baseFuelLoss = 0.01f;
    [SerializeField] private float damageFuelLossMultiplier = 0.0003f;
    [SerializeField] private float torqueFuelLossMultiplier = 0.00001f;

    private WheelControl[] wheels;
    private Rigidbody rigidBody;
    private CarInputActions carControls;

    private float oldRotation;
    private float collisionSpeed;


    private AudioSource audioSource;
    private AudioSource engineAudio;


    void AddDamagePercentage(float addToDamagePercentage)
    {
        float newDamagePercentage = damagePercentage + addToDamagePercentage;
        float minValue = 0f;
        float maxValue = 100f;
        float clampedDamagePercentage = Mathf.Clamp(newDamagePercentage, minValue, maxValue);

        damagePercentage = clampedDamagePercentage;
    }

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
        OffsetCenterOfGravity();
        wheels = GetComponentsInChildren<WheelControl>();

        // Haal de audioSources uit child GameObjects
        Transform crashSoundObject = transform.Find("CrashSoundSource");
        Transform engineSoundObject = transform.Find("EngineSoundSource");

        if (crashSoundObject != null)
            audioSource = crashSoundObject.GetComponent<AudioSource>();
        else
            Debug.LogWarning("CrashSoundSource object niet gevonden!");

        if (engineSoundObject != null)
        {
            engineAudio = engineSoundObject.GetComponent<AudioSource>();
            engineAudio.loop = true;
        }
        else
            Debug.LogWarning("EngineSoundSource object niet gevonden!");
    }


    private void OffsetCenterOfGravity()
    {
        Vector3 centerOfMass = rigidBody.centerOfMass;
        centerOfMass.x += centerOfGravityOffset.x;
        centerOfMass.y += centerOfGravityOffset.y;
        centerOfMass.z += centerOfGravityOffset.z;
        rigidBody.centerOfMass = centerOfMass;
    }

    void OnCollisionEnter(Collision collision)
    {
        collisionSpeed = CurrentSpeed;

        if (audioSource != null)
        {
            audioSource.Play();
        }
    }



    void OnCollisionExit(Collision collision)
    {
        float collisionReduction = Math.Abs(CurrentSpeed - collisionSpeed);
        AddDamagePercentage(collisionReduction * 10);
    }

    void FixedUpdate()
    {
        bool isUsingHandbrake = carControls.Car.Handbrake.ReadValue<float>() > 0.5f;

        Vector2 inputVector = carControls.Car.Movement.ReadValue<Vector2>();
        float throttleInput = inputVector.y;
        float steeringInput = inputVector.x;

        float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.linearVelocity);
        float speedFactor = Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(forwardSpeed));

        float motorTorqueWithDamage = motorTorque * (100 - damagePercentage) / 100;
        float currentMotorTorque = Mathf.Lerp(motorTorqueWithDamage, 0, speedFactor);
        float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        bool canAccelerate = Fuel > 0 && damagePercentage < 100;

        bool negligibleForwardSpeed = forwardSpeed > -0.1f && forwardSpeed < 0.1f;
        bool signsAreSame = Mathf.Sign(throttleInput) == Mathf.Sign(forwardSpeed);
        bool isAccelerating = (signsAreSame || negligibleForwardSpeed) && throttleInput != 0;

        ApplyDrive(isAccelerating, canAccelerate, isUsingHandbrake, throttleInput, currentMotorTorque);
        ApplyFuelUsage(isAccelerating, currentMotorTorque);

        ApplySteering(steeringInput, currentSteerRange);
        SteeringAssist();
        AntiRoll();


        float pitchMaxSpeed = 80f; // bij deze snelheid moet pitch = 2f zijn
        float t = Mathf.Clamp01(CurrentSpeed / pitchMaxSpeed);
        engineAudio.pitch = Mathf.Lerp(0.5f, 2f, t);


        
        if ( !engineAudio.isPlaying)
        {
            engineAudio.Play();
        }
    
    }

    private void ApplyDrive(bool isAccelerating, bool canAccelerate, bool isUsingHandbrake, float throttleInput, float currentMotorTorque)
    {
        foreach (var wheel in wheels)
        {
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
                if (!isUsingHandbrake)
                {
                    wheel.WheelCollider.brakeTorque = Mathf.Abs(throttleInput) * brakeTorque;
                }
            }

            if (isUsingHandbrake)
            {
                wheel.WheelCollider.brakeTorque = handbrakeTorque;
            }
        }
    }

    private void ApplySteering(float steeringInput, float currentSteerRange)
    {
        foreach (var wheel in wheels)
        {
            if (wheel.steerable)
            {
                wheel.WheelCollider.steerAngle = steeringInput * currentSteerRange;
            }
        }
    }

    private void SteeringAssist()
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
        foreach (WheelControl wheel in wheels)
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

        foreach (WheelControl wheel in wheels)
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
            float damageFuelLoss = damagePercentage * damageFuelLossMultiplier;
            float torqueFuelLoss = (motorTorque - currentMotorTorque) * torqueFuelLossMultiplier;

            float FuelUsage = baseFuelLoss + damageFuelLoss;
            if (isAccelerating)
            {
                FuelUsage += torqueFuelLoss;
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
