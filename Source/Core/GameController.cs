using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private static bool isInitialized = false;

    // Scene names
    public const string MainMenuSceneName = "MenuScene";
    public const string MainLevelSceneName = "Warehouse";
    public const string RoundUIScene = "RoundUI";

    private void Awake()
    {
        if (!isInitialized) InitializeGame();
    }

    void Start()
    {
        if (!isInitialized)
        {
            InitializeGame();
            if (!isInitialized)
            {
                Debug.LogError("Could not start game. Failed to initialize.");
                return;
            }
        }

        Game.InvokeOnGameStart();
    }

    void Update()
    {
        
    }

    private void InitializeGame()
    {
        Game.OnGameStart += OnGameStart;
        Game.OnPlay += OnPlay;
        Game.OnQuit += OnQuit;
        Game.OnWin += OnWin;
        Game.OnQuitToMenu += OnQuitToMenu;
        isInitialized = true;
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    private void UnloadScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }

    private void OnGameStart()
    {
        Debug.Log("Game controller: Loading menu scene");
        LoadScene(MainMenuSceneName);
    }

    private void OnPlay()
    {
        Debug.Log("Game controller: Playing game");
        UnloadScene(MainMenuSceneName);
        LoadScene(MainLevelSceneName);
        LoadScene(RoundUIScene);
    }

    private void OnQuit()
    {
        Debug.Log("Game controller: Quitting.");
        Application.Quit(0);
    }

    private void OnWin()
    {
        Debug.Log("Game controller: Win");
    }

    private void OnQuitToMenu()
    {
        Debug.Log("Game controller: Quit to menu.");
        UnloadScene(MainLevelSceneName);
        UnloadScene(RoundUIScene);
        LoadScene(MainMenuSceneName);
        StartCoroutine(EnableCursor());
        
    }

    private IEnumerator EnableCursor()
    {
        yield return new WaitForSeconds(0.0f);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
