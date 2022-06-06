using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSounds : MonoBehaviour
{
    [SerializeField] private Transform vehicleRoot;
    [SerializeField] private AudioSource sound;
    [SerializeField] private List<Transform> ObjectsToIgnore = new List<Transform>();

    const float minimumImpulse = 200.0f;
    const float maximumImpulse = 1000.0f;

    [SerializeField] private float minimumVolume = 0.075f;
    [SerializeField] private float maximumVolume = 1.0f;
    private List<CollisionSoundSource> collisionSoundSources = new List<CollisionSoundSource>();
    
    void Start()
    {
        if (vehicleRoot == null)
        {
            Debug.LogWarning("Could not initialize collision sound handler. Invalid vehicle root reference.");
            enabled = false;
            return;
        }

        CollisionSoundSource[] sources = vehicleRoot.GetComponentsInChildren<CollisionSoundSource>();
        foreach (CollisionSoundSource source in sources)
        {
            source.SetCollisionSoundHandler(this);
        }
    }

    void Update()
    {
        
    }

    public void OnHandleVehicleCollision(Collision collision)
    {
        if (IsColliderPartOfObjectToIgnore(collision.collider)) return;

        float impulse = collision.impulse.magnitude;
        if (impulse > minimumImpulse)
        {
            float collisionStrength = Mathf.InverseLerp(minimumImpulse, maximumImpulse, impulse);
            PlayCollisionSound(collisionStrength);   
        }
    }

    private void PlayCollisionSound(float collisionStrength)
    {
        if (sound == null) return;

        float volume = Mathf.Lerp(minimumVolume, maximumVolume, collisionStrength);
        sound.volume = volume;
        sound.Play();
    }

    private bool IsColliderPartOfObjectToIgnore(Collider collider)
    {
        foreach (Transform t in ObjectsToIgnore)
        {
            Collider[] colliders = t.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                if (col == collider) return true;
            }
        }

        return false;
    }
}
