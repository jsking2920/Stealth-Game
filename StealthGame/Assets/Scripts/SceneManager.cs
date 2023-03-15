using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager S;

    public string menuSceneName = "Menu";

    private void Awake()
    {
        if (S != null)
        {
            print("2 scene managers!!");
            Destroy(gameObject);
        }

        S = this;
        DontDestroyOnLoad(this);
    }

    public void ToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(menuSceneName);
    }

    public void btn_PlayGame(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void btn_ExitGame()
    {
        Application.Quit();
    }
}
