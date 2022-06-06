using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkliftLiftSound : MonoBehaviour
{
    [SerializeField] private AudioSource sound;
    [SerializeField] private Transform lift;

    [SerializeField] private float MaxSpeed = 1.0f;

    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.2f;
    [SerializeField] private float minVolume = 0.1f;
    [SerializeField] private float maxVolume = 1.2f;

    private Vector3 currentPosition;
    private Vector3 previousPosition;
    [SerializeField] private float speed;
    [SerializeField] private float normalizedSpeed;

    void Start()
    {
        if (lift == null)
        {
            Debug.LogWarning("Could not initialize forklift lift sound. Invalid vehicle lift transform reference.");
            enabled = false;
            return;
        }

        if (sound == null)
        {
            Debug.LogWarning("Could not initialize forklift drive sound. Invalid sound reference.");
            enabled = false;
            return;
        }

        currentPosition = lift.localPosition;
        sound.Play();
    }

    void Update()
    {
        previousPosition = currentPosition;
        currentPosition = lift.localPosition;
        Vector3 vehicleUp = lift.up;
        speed = Mathf.Lerp(speed, Vector3.Dot((currentPosition - previousPosition) / Time.deltaTime, vehicleUp), 0.3f);
        normalizedSpeed = Mathf.Clamp01(Mathf.Abs(speed / MaxSpeed));

        float pitch = Mathf.Lerp(minPitch, maxPitch, normalizedSpeed);
        float volume = Mathf.Lerp(minVolume, maxVolume, normalizedSpeed);
        sound.volume = volume;
        sound.pitch = pitch;
    }
}
