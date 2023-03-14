using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager S;

    public string gameplaySceneName = "Main";

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

    public void btn_PlayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameplaySceneName);
    }
}
