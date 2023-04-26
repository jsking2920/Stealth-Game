using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager S;

    public string menuSceneName = "Menu";
    public string gameSceneName = "Main";

    private GameObject managerToInstantiate = null;

    private void Awake()
    {
        if (S == null)
        {
            S = this;
            DontDestroyOnLoad(this);
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
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
        UnityEngine.SceneManagement.SceneManager.LoadScene(menuSceneName, LoadSceneMode.Single);
    }

    public void btn_PlayGame(GameObject gameModeManagerPrefab)
    {
        managerToInstantiate = gameModeManagerPrefab;
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }

    public void btn_RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    public void btn_ExitGame()
    {
        Application.Quit();
    }
}
