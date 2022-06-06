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
