using System;
using System.Linq;
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

    [Header("Audio Sources")]
    private AudioSource audioSource;
    private AudioSource engineAudio;

    [Header("Particle Systems")]
    public ParticleSystem particleSystem1; 
    public ParticleSystem particleSystem2;


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

        Transform crashSoundObject = transform.Find("CrashSoundSource");
        Transform engineSoundObject = transform.Find("EngineSoundSource");

        if (crashSoundObject != null)
            audioSource = crashSoundObject.GetComponent<AudioSource>();

        if (engineSoundObject != null)
            engineAudio = engineSoundObject.GetComponent<AudioSource>();
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

        PlayAudio();
        ShowParticles(isAccelerating);
    }

    private void PlayAudio()
    {
        float pitchMaxSpeed = 100f;
        float t = Mathf.Clamp01(CurrentSpeed / pitchMaxSpeed);
        engineAudio.pitch = Mathf.Lerp(0.5f, 3f, t);

        if (!engineAudio.isPlaying)
        {
            engineAudio.Play();
        }
    }

    private void ShowParticles(bool isAccelerating)
    {
        float speedInKmph = CurrentSpeed;
        float minSpeedForParticles = 40f;

        bool isGrounded = wheels.Any(o => o.WheelCollider.isGrounded);

        if (!isGrounded)
        {
            particleSystem1.Stop();
            particleSystem2.Stop();
        }
        else if (isAccelerating)
        {
            particleSystem1.Play();
            particleSystem2.Play();
            ChangeParticleSettings();
        }
        else if (speedInKmph < minSpeedForParticles) 
        {
            particleSystem1.Stop();
            particleSystem2.Stop();
        }
    }

    private void ChangeParticleSettings()
    {
        float speedInKmph = CurrentSpeed;

        float rotationX = Mathf.Lerp(0f, -40f, speedInKmph / 100f);

        particleSystem1.transform.rotation = Quaternion.Euler(rotationX, particleSystem1.transform.eulerAngles.y, particleSystem1.transform.eulerAngles.z);
        particleSystem2.transform.rotation = Quaternion.Euler(rotationX, particleSystem2.transform.eulerAngles.y, particleSystem2.transform.eulerAngles.z);

        var emission1 = particleSystem1.emission;
        emission1.rateOverTime = Mathf.Lerp(0f, 100f, speedInKmph / 100f);

        var emission2 = particleSystem2.emission;
        emission2.rateOverTime = Mathf.Lerp(0f, 100f, speedInKmph / 100f);

        var velocityOverLifetime1 = particleSystem1.velocityOverLifetime;
        velocityOverLifetime1.z = new ParticleSystem.MinMaxCurve(-speedInKmph / 20f);

        var velocityOverLifetime2 = particleSystem2.velocityOverLifetime;
        velocityOverLifetime2.z = new ParticleSystem.MinMaxCurve(-speedInKmph / 20f);
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
