using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSoundSource : MonoBehaviour
{
    private CollisionSounds collisionSoundController;

    // Start is called before the first frame update
    void Start()
    {
        if (collisionSoundController == null)
        {
            Debug.LogWarning("Could not initialize collision sound source (" + gameObject.name + "). Invalid collision sound controller reference.");
            enabled = false;
            return;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision sound source (" + gameObject.name + "): Collision. Impulse = " + collision.impulse.magnitude);
        if (collisionSoundController != null) collisionSoundController.OnHandleVehicleCollision(collision);
    }

    public void SetCollisionSoundHandler(CollisionSounds collisionSoundController)
    {
        this.collisionSoundController = collisionSoundController;
        Debug.Log("Collision sound source (" + gameObject.name + ") registered.");
    }
}
