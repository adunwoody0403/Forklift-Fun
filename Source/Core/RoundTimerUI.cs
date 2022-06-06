using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static RoundController;

public class RoundTimerUI : MonoBehaviour
{
    [SerializeField] private Text preRoundText;
    [SerializeField] private Text minuteText;
    [SerializeField] private Text secondText;
    [SerializeField] private Text roundTimerColonText;
    [SerializeField] private Text currentPickupText;
    [SerializeField] private Text pickupSlashText;
    [SerializeField] private Text totalPickupText;

    [SerializeField] private GameObject preRoundTextObj;
    [SerializeField] private GameObject roundTextObj;
    [SerializeField] private GameObject winTextObject;

    [SerializeField] private Color roundInProgressTextColour = Color.white;
    [SerializeField] private Color waitForCompleteTextColour = Color.red;
    [SerializeField] private Color winTextColour = Color.green;

    private RoundController roundController;
    private PickupCollectionZone zone;

    // Start is called before the first frame update
    void Start()
    {
        roundController = Object.FindObjectOfType<RoundController>();
        if (roundController == null)
        {
            Debug.LogError("RoundTimerUI: Could not start. Failed to find round controller.");
            enabled = false;
            return;
        }

        zone = Object.FindObjectOfType<PickupCollectionZone>();
        if (zone == null)
        {
            Debug.LogError("RoundTimerUI: Could not start. Failed to find PickupCollectionZone");
            enabled = false;
            return;
        }

        InitializeTotalPickupText();
    }

    // Update is called once per frame
    void Update()
    {
        if (roundController == null) return;

        UpdatePreRoundTextState();
        UpdateRoundTextState();
        UpdateWinTextState();

        UpdatePreRoundText();
        UpdateMinuteText();
        UpdateSecondText();
        UpdatePickupText();
    }

    private void UpdatePreRoundTextState()
    {
        RoundState roundState = roundController.GetRoundState();
        if (roundState == RoundState.PreRound)
        {
            if (!preRoundTextObj.activeSelf) preRoundTextObj.SetActive(true);
        }
        else
        {
            if (preRoundTextObj.activeSelf) preRoundTextObj.SetActive(false);
        }
    }

    private void UpdateRoundTextState()
    {
        RoundState roundState = roundController.GetRoundState();
        if (roundState == RoundState.PreRound)
        {
            if (roundTextObj.activeSelf) roundTextObj.SetActive(false);
        }
        else
        {
            if (!roundTextObj.activeSelf) roundTextObj.SetActive(true);

            if (roundState == RoundState.InProgress)
            {
                UpdateRoundTextColour(roundInProgressTextColour);
            }
            else if (roundState == RoundState.WaitingForCompleteTimer)
            {
                UpdateRoundTextColour(waitForCompleteTextColour);
            }
            else if (roundState == RoundState.Win)
            {
                UpdateRoundTextColour(winTextColour);
            }
        }
    }

    private void UpdateWinTextState()
    {
        if (roundController == null) return;

        RoundState roundState = roundController.GetRoundState();
        if (roundState == RoundState.Win)
        {
            if (!winTextObject.activeSelf) winTextObject.SetActive(true);
        }
        else
        {
            if (winTextObject.activeSelf) winTextObject.SetActive(false);
        }
    }

    private void UpdatePreRoundText()
    {
        if (preRoundText == null || !preRoundText.enabled) return;

        int seconds = roundController.GetPreRoundCountdownSeconds();
        preRoundText.text = string.Format("{0,-3:00}", seconds);
    }

    private void UpdateMinuteText()
    {
        if (minuteText == null || !minuteText.enabled) return;

        int minutes = roundController.GetRoundTimeMinutes();
        minuteText.text = string.Format("{0,3:00}", minutes);
    }

    private void UpdateSecondText()
    {
        if (secondText == null || !secondText.enabled) return;

        int seconds = roundController.GetRoundTimeSeconds();
        secondText.text = string.Format("{0,-3:00}", seconds);
    }

    private void UpdatePickupText()
    {
        if (currentPickupText == null) return;
        int currentPickups = zone.GetCurrentPickupCount();
        currentPickupText.text = string.Format("{0,2}", currentPickups);
    }

    private void InitializeTotalPickupText()
    {
        if (totalPickupText == null) return;
        if (zone == null) return;
        int totalPickups = zone.GetTotalPickupCount();
        totalPickupText.text = string.Format("{0,-2}", totalPickups);
    }

    private void UpdateRoundTextColour(Color color)
    {
        if (secondText != null)
        {
            if (secondText.color != color) secondText.color = color;
        }

        if (minuteText != null)
        {
            if (minuteText.color != color) minuteText.color = color;
        }

        if (roundTimerColonText != null)
        {
            if (roundTimerColonText.color != color) roundTimerColonText.color = color;
        }

        if (currentPickupText != null)
        {
            if (currentPickupText.color != color) currentPickupText.color = color;
        }

        if (pickupSlashText != null)
        {
            if (pickupSlashText.color != color) pickupSlashText.color = color;
        }

        if (totalPickupText != null)
        {
            if (totalPickupText.color != color) totalPickupText.color = color;
        }
    }
}
