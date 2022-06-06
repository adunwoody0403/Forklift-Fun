using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void OnPressPlay()
    {
        Debug.Log(" --------- Play --------- ");
        Game.InvokePlay();
    }

    public void OnPressQuit()
    {
        Debug.Log(" --------- Quit --------- ");
        Game.InvokeQuit();
    }
}
