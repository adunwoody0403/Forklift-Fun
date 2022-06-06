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

    // Start is called before the first frame update
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

    // Private
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
