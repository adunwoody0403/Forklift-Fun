using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkliftLiftController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] [Range(-1, 1)] private float throttleInput;
    [SerializeField] [Range(-1, 1)] private float steerInput;
    [SerializeField] [Range(0, 1)] private float liftInput;
    [SerializeField] [Range(0, 1)] private float upInput;
    [SerializeField] [Range(0, 1)] private float downInput;

    [Header("Drive Configuration")]
    [SerializeField] private float maxSpeed = 10.0f;
    [SerializeField] private float maxTorque = 100.0f;
    [SerializeField] private float maxBrakeTorque = 100.0f;
    [SerializeField] private float maxSteerAngle = 30.0f;
    [SerializeField] private AnimationCurve driveTorqueCurve = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 0.0f);
    [SerializeField] private Transform centreOfMass;
    [SerializeField] private Rigidbody rb;

    [Header("Lift Configuration")]
    [SerializeField] private float maxLiftSpeed = 1.0f;
    [SerializeField] private float maxLiftPosition = 3.0f;
    [SerializeField] private float minLiftPosition = -0.5f;
    [SerializeField] private Transform lift;

    [Header("Wheel Configuration")]
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;
    [SerializeField] private Transform frontLeftWheel;
    [SerializeField] private Transform frontRightWheel;
    [SerializeField] private Transform rearLeftWheel;
    [SerializeField] private Transform rearRightWheel;

    [Header("Runtime")]
    [SerializeField] private float speed;
    [SerializeField] private float driveTorque;
    [SerializeField] private float brakeTorque;
    [SerializeField] private float steerAngle;
    [SerializeField] private float liftPosition;
    private Vector3 previousPosition;
    private Vector3 initialLiftPosition;

    private void Start()
    {
        rb.centerOfMass = transform.InverseTransformPoint(centreOfMass.position);
        previousPosition = transform.position;
        initialLiftPosition = lift.localPosition;
    }

    private void Update()
    {
        UpdateInputs();
    }

    void FixedUpdate()
    {
        Drive();
        UpdateLift();
        UpdateWheels();
    }

    private void UpdateInputs()
    {
        throttleInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
        upInput = Input.GetAxis("Fire1");
        downInput = Input.GetAxis("Fire2");
        liftInput = upInput - downInput;
    }

    private void Drive()
    {
        Vector3 position = transform.position;
        speed = Vector3.Dot((position - previousPosition) / Time.deltaTime, transform.forward);

        float speedNormalized = Mathf.Clamp01(Mathf.Abs(speed) / maxSpeed);
        float driveTorqueFactor = driveTorqueCurve.Evaluate(speedNormalized);
        driveTorque = throttleInput * maxTorque * driveTorqueFactor;

        if (Mathf.Abs(driveTorque) < 0.01f || Mathf.Sign(driveTorque) != Mathf.Sign(speed)) brakeTorque = maxBrakeTorque;
        else brakeTorque = 0.0f;

        steerAngle = -maxSteerAngle * steerInput;

        previousPosition = position;
    }

    private void UpdateLift()
    {
        liftPosition += liftInput * maxLiftSpeed * Time.deltaTime;
        if (liftPosition > maxLiftPosition) liftPosition = maxLiftPosition;
        else if (liftPosition < minLiftPosition) liftPosition = minLiftPosition;
        lift.localPosition = initialLiftPosition + (Vector3.up * liftPosition);
    }

    private void UpdateWheels()
    {
        // Update wheel colliders
        frontLeftWheelCollider.motorTorque = driveTorque;
        frontRightWheelCollider.motorTorque = driveTorque;
        rearLeftWheelCollider.motorTorque = driveTorque;
        rearRightWheelCollider.motorTorque = driveTorque;

        frontLeftWheelCollider.brakeTorque = brakeTorque;
        frontRightWheelCollider.brakeTorque = brakeTorque;
        rearLeftWheelCollider.brakeTorque = brakeTorque;
        rearRightWheelCollider.brakeTorque = brakeTorque;

        rearLeftWheelCollider.steerAngle = steerAngle;
        rearRightWheelCollider.steerAngle = steerAngle;

        // Update wheel visuals
        Vector3 wheelPosition;
        Quaternion wheelRotation;

        frontLeftWheelCollider.GetWorldPose(out wheelPosition, out wheelRotation);
        frontLeftWheel.transform.position = wheelPosition;
        frontLeftWheel.transform.rotation = wheelRotation;

        frontRightWheelCollider.GetWorldPose(out wheelPosition, out wheelRotation);
        frontRightWheel.transform.position = wheelPosition;
        frontRightWheel.transform.rotation = wheelRotation;

        rearLeftWheelCollider.GetWorldPose(out wheelPosition, out wheelRotation);
        rearLeftWheel.transform.position = wheelPosition;
        rearLeftWheel.transform.rotation = wheelRotation;

        rearRightWheelCollider.GetWorldPose(out wheelPosition, out wheelRotation);
        rearRightWheel.transform.position = wheelPosition;
        rearRightWheel.transform.rotation = wheelRotation;
    }
}
