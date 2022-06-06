using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Game
{
    public delegate void GameEvent();
    public static event GameEvent OnGameStart;
    public static event GameEvent OnPlay;
    public static event GameEvent OnQuit;
    public static event GameEvent OnWin;
    public static event GameEvent OnQuitToMenu;

    public static GameObject MainVehicle { get { return Object.FindObjectOfType<MainVehicle>()?.gameObject; }}

    public static void InvokeOnGameStart()
    {
        Debug.Log("===== OnGameStart =====");
        OnGameStart?.Invoke();
    }

    public static void InvokePlay()
    {
        Debug.Log("===== OnPlay =====");
        OnPlay?.Invoke();
    }

    public static void InvokeQuit()
    {
        Debug.Log("===== OnQuit =====");
        OnQuit?.Invoke();
    }

    public static void InvokeGameWin()
    {
        Debug.Log("===== Win =====");
        OnWin?.Invoke();
    }

    public static void InvokeQuitToMenu()
    {
        Debug.Log("===== Quit To Menu =====");
        OnQuitToMenu?.Invoke();
    }
}
