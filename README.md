# Forklift Fun
Forklift Fun is a small game I made in 2017 using Unity. In this game, you play as a forklift operator in a warehouse, with the main objective being to deliver water tanks to a designated loading zone. Though small, the game is a complete package with gameplay mechanics, objectives, driving physics and audio. To play this game, you can download a build [here](https://drive.google.com/drive/folders/1t1hmGVzZbIq_E2GQWHAC4pCT9Omlcwio?usp=sharing).

![Game](/Images/Game.PNG)

## Code Samples
I have provided code samples for this game in the Source folder. Here are some specific examples:

### Forklift
The game centres around the player operating a forklift to transport cargo. The physics is handled using PhysX wheel colliders. Here is the code for operating the forklift.
```C#
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

```

### Gameplay
Here is a sample of the logic used to track the main objective of the game, delivering 6 water tanks to the loading zone.
```C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Game;

public class RoundController : MonoBehaviour
{
    //Pickup zone
    private PickupCollectionZone zone;

    // Round state
    public enum RoundState { PreRound, InProgress, WaitingForCompleteTimer, Win }
    private RoundState roundState;

    // Round timer
    private const float PreRoundDuration = 5.0f;
    [SerializeField] private float preRoundCountdown = 0.0f;
    [SerializeField] private float roundTimer = 0.0f;

    [SerializeField] private int roundTimeSeconds = 0;
    [SerializeField] private int roundTimeMinutes = 0;

    // Round events
    public GameEvent OnPreRoundStart;
    public GameEvent OnRoundStart;

    // Inputs to disable
    [SerializeField] private List<MonoBehaviour> inputComponentsToDisable = new List<MonoBehaviour>();

    void Start()
    {
        zone = Object.FindObjectOfType<PickupCollectionZone>();
        if (zone == null)
        {
            Debug.LogError("Round controller. Could not start game. Failed to find pickupcollectionzone");
            enabled = false;
            return;
        }

        zone.OnBeginCompleteRoundTimer += OnRoundCompleteTimerBegin;
        zone.OnCancelCompleteRoundTimer += OnRoundCompleteTimerCancel;
        Game.OnWin += EndRound;
        BeginPreRound();
    }

    void Update()
    {
        UpdateRoundTimers();
        if (Input.GetKey(KeyCode.Escape))
        {
            Quit();
        }
    }

    public RoundState GetRoundState()
    {
        return roundState;
    }

    public float GetRoundTime()
    {
        return roundTimer;
    }

    public int GetRoundTimeMinutes()
    {
        return roundTimeMinutes;
    }

    public int GetRoundTimeSeconds()
    {
        return roundTimeSeconds;
    }

    public int GetPreRoundCountdownSeconds()
    {
        return (int)(Mathf.Floor(preRoundCountdown));
    }

    private void UpdateRoundTimers()
    {
        if (roundState == RoundState.PreRound)
        {
            preRoundCountdown -= Time.deltaTime;
            if (preRoundCountdown <= 0)
            {
                BeginRound();
            }
        }
        else if (roundState == RoundState.InProgress)
        {
            roundTimer += Time.deltaTime;

            float detailedTime = roundTimer;
            roundTimeMinutes = (int) Mathf.Floor(detailedTime / 60.0f);
            detailedTime %= 60.0f;
            roundTimeSeconds = (int) Mathf.Floor(detailedTime);        
        }
    }

    private void BeginPreRound()
    {
        roundState = RoundState.PreRound;
        preRoundCountdown = PreRoundDuration;
        OnPreRoundStart?.Invoke();

        foreach(MonoBehaviour b in inputComponentsToDisable)
        {
            b.enabled = false;
        }
    }

    private void BeginRound()
    {
        roundState = RoundState.InProgress;
        roundTimer = 0.0f;
        OnRoundStart?.Invoke();

        foreach (MonoBehaviour b in inputComponentsToDisable)
        {
            b.enabled = true;
        }
    }

    private void EnterWaitForWinState()
    {
        roundState = RoundState.WaitingForCompleteTimer;
    }

    private void ContinueRound()
    {
        roundState = RoundState.InProgress;
    }

    private void EndRound()
    {
        roundState = RoundState.Win;
    }

    private void OnRoundCompleteTimerBegin()
    {
        EnterWaitForWinState();
    }

    private void OnRoundCompleteTimerCancel()
    {
        ContinueRound();
    }

    private void Quit()
    {
        Game.InvokeQuitToMenu();
    }
}

```

### Audio
Audio has been added to the game for enhanced immersion. Here is a sample of how the audio is handled for the forklift driving.
```C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkliftDriveSound : MonoBehaviour
{
    [SerializeField] private AudioSource sound;
    [SerializeField] private Rigidbody vehicleBody;

    [SerializeField] private float MaxSpeed = 10.0f;

    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.2f;
    [SerializeField] private float minVolume = 0.1f;
    [SerializeField] private float maxVolume = 1.2f;

    [SerializeField] private float speed;
    [SerializeField] private float normalizedSpeed;

    void Start()
    {
        if (vehicleBody == null)
        {
            Debug.LogWarning("Could not initialize forklift drive sound. Invalid vehicle rigidbody reference.");
            enabled = false;
            return;
        }

        if (sound == null)
        {
            Debug.LogWarning("Could not initialize forklift drive sound. Invalid sound reference.");
            enabled = false;
            return;
        }

        sound.Play();
    }
    
    void Update()
    { 
        Vector3 currentVelocity = vehicleBody.velocity;
        speed = Vector3.Dot(currentVelocity, vehicleBody.gameObject.transform.forward);
        normalizedSpeed = Mathf.Clamp01(Mathf.Abs(speed) / MaxSpeed);

        float pitch = Mathf.Lerp(minPitch, maxPitch, normalizedSpeed);
        float volume = Mathf.Lerp(minVolume, maxVolume, normalizedSpeed);
        sound.volume = volume;
        sound.pitch = pitch;
    }
}

```
