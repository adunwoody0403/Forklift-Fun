using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField] private KeyCode lightToggle = KeyCode.L;
    [SerializeField] private List<Light> lights;

    private bool lightsEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        if (lights.Count <= 0)
        {
            Debug.LogWarning("Could not initialize forklift light controller. Missing light references.");
            enabled = false;
            return;
        }

        lightsEnabled = false;
        DisableLights();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(lightToggle))
        {
            lightsEnabled = !lightsEnabled;
            if (lightsEnabled) EnableLights();
            else DisableLights();
        }
    }

    private void EnableLights()
    {
        foreach (Light light in lights)
        {
            light.enabled = true;
        }
    }

    private void DisableLights()
    {
        foreach (Light light in lights)
        {
            light.enabled = false;
        }
    }
}
