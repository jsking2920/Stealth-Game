using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public string menuSceneName = "Menu";
    public string gameSceneName = "Main";

    private GameObject managerToInstantiate = null;

    void Start()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == gameSceneName && managerToInstantiate != null)
        {
            Instantiate(managerToInstantiate);
        }
    }

    public void ToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(menuSceneName);
    }

    public void btn_PlayGame(GameObject gameModeManagerPrefab)
    {
        managerToInstantiate = gameModeManagerPrefab;
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }

    public void btn_ExitGame()
    {
        Application.Quit();
    }
}
