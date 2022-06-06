using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Game;

public class PickupCollectionZone : MonoBehaviour
{
    private BoxCollider zone;
    private List<PickupData> pickups = new List<PickupData>();

    [SerializeField] private int totalPickups;
    [SerializeField] private int currentPickupCount;

    public GameEvent OnBeginCompleteRoundTimer;
    public GameEvent OnCancelCompleteRoundTimer;

    private bool completeTimerRunning = false;
    private bool collectedAllPickups = false;
    private float completeTimer;
    private const float waitTimeSeconds = 3.0f;
    private bool hasWon = false;

    private class PickupData
    {
        public PickupObject pickup = null;
        public List<Collider> colliders = new List<Collider>();
    }


    // Start is called before the first frame update
    void Start()
    {
        zone = GetComponent<BoxCollider>();
        if (zone == null)
        {
            Debug.LogError("PickupCollectionZone failed to initialize. Could not find box collider zone.");
            enabled = false;
            return;
        }

        if (!zone.isTrigger) zone.isTrigger = true;

        //List<PickupObject> allPickups = new List<PickupObject>();
        PickupObject[] pickups = Object.FindObjectsOfType<PickupObject>();
        totalPickups = pickups.Length;
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePickupState();
        UpdateCompleteTimer();
    }
    
    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Something entered zone.");
        PickupObject pickup = other.GetComponentInParent<PickupObject>();
        if (pickup != null) OnPickupEnteredZone(pickup, other);
    }

    public void OnTriggerExit(Collider other)
    {
        //Debug.Log("Something exited zone.");
        PickupObject pickup = other.GetComponentInParent<PickupObject>();
        if (pickup != null) OnPickupExitedZone(pickup, other);
    }

    void OnPickupEnteredZone(PickupObject pickup, Collider collider)
    {
        PickupData data = FindPickupData(pickup);
        if (data == null)
        {
            AddNewPickup(pickup, collider);
            Debug.Log("Pickup entered zone.");
        }
        else
        {
            if (!data.colliders.Contains(collider)) data.colliders.Add(collider);
        }
    }

    void OnPickupExitedZone(PickupObject pickup, Collider collider)
    {
        PickupData data = FindPickupData(pickup);
        if (data == null)
        {
            Debug.LogWarning("Unregistered pickup exited zone.");
        }
        else
        {
            if (data.colliders.Contains(collider))
            {
                //Debug.Log("Pickup collider exited zone.");
                data.colliders.Remove(collider);
            }

            if (data.colliders.Count <= 0)
            {
                pickups.Remove(data);
                Debug.Log("Pickup exited zone.");
            }
        }
    }

    private PickupData FindPickupData(PickupObject pickup)
    {
        foreach (PickupData p in pickups)
        {
            if (p.pickup == pickup) return p;
        }

        return null;
    }

    private void AddNewPickup(PickupObject pickup, Collider collider)
    {
        PickupData data = new PickupData();
        data.pickup = pickup;
        data.colliders.Add(collider);

        pickups.Add(data);
    }

    private void UpdatePickupState()
    {
        if (currentPickupCount != pickups.Count) currentPickupCount = pickups.Count;
        if (currentPickupCount >= totalPickups)
        {
            if (!collectedAllPickups) collectedAllPickups = true;
        }
        else
        {
            if (collectedAllPickups) collectedAllPickups = false;
        }
    }

    private void UpdateCompleteTimer()
    {
        if (hasWon) return;

        if (collectedAllPickups && AreAllPickupsSleeping())
        {
            if (!completeTimerRunning)
            {
                Debug.Log("Complete timer started.");
                completeTimerRunning = true;
                completeTimer = 0.0f;
                OnBeginCompleteRoundTimer?.Invoke();
            }
        }
        else
        {
            if (completeTimerRunning)
            {
                Debug.Log("Complete timer stopped.");
                completeTimerRunning = false;
                OnCancelCompleteRoundTimer?.Invoke();
            }
        }

        if (completeTimerRunning)
        {
            if (completeTimer < waitTimeSeconds)
            {
                completeTimer += Time.deltaTime;
            }
            else
            {
                if (completeTimer != waitTimeSeconds) completeTimer = waitTimeSeconds;
                if (!hasWon)
                {
                    hasWon = true;
                    Game.InvokeGameWin();
                }
            }
        }
    }

    public bool IsCompleteTimerRunning()
    {
        return completeTimerRunning;
    }

    public float GetCompleteTimer()
    {
        return completeTimer;
    }

    public int GetTotalPickupCount()
    {
        return totalPickups;
    }

    public int GetCurrentPickupCount()
    {
        return currentPickupCount;
    }

    private bool AreAllPickupsSleeping()
    {
        foreach (PickupData pickup in pickups)
        {
            Rigidbody pickupBody = pickup.pickup.GetComponentInChildren<Rigidbody>();
            if (pickupBody == null) continue;
            if (!pickupBody.IsSleeping()) return false;
        }

        return true;
    }
}
