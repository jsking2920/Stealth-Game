using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] teamTexts; // need to have as many of these as there can be teams (6 currently, technically)
    [SerializeField] private TextMeshProUGUI centerScreenMessage;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private Image blackScreen; // used for fade to black/cuts

    private bool showTimer = false;
    [HideInInspector] public bool useFloatScore = false; // defaults to using int score from teams
    private int teamsJoined = 0;

    public float cutToBlackTime = 4f;

    private void Start()
    {
        blackScreen.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        pausePanel.SetActive(false);
        endGamePanel.SetActive(false);

        centerScreenMessage.gameObject.SetActive(true);
        centerScreenMessage.text = "Press Space To Start";

        foreach (TextMeshProUGUI t in teamTexts)
        {
            t.text = "JOIN";
            t.color = Color.white;
            t.gameObject.SetActive(true);
        }
    }

    // Call this in Start in timed game modes to show the timer
    public void ShowTimerAfterCut()
    {
        showTimer = true;
    }

    public void OnPause()
    {
        pausePanel.SetActive(true);
    }

    public void OnResume()
    {
        pausePanel.SetActive(false);
    }

    // TODO: rethink win screen UI
    public void OnGameEnd(string message = "You Win")
    {
        centerScreenMessage.text = message;
        centerScreenMessage.gameObject.SetActive(true);
        timerText.gameObject.SetActive(false);

        endGamePanel.SetActive(true);
    }

    public void OnGameStart(string message)
    {
        StartCoroutine(CutToBlackCo(message));
    }

    private IEnumerator CutToBlackCo(string message)
    {
        for (int i = teamsJoined; i < teamTexts.Length; i++)
        {
            teamTexts[i].gameObject.SetActive(false);
        }

        centerScreenMessage.gameObject.SetActive(false);
        blackScreen.gameObject.SetActive(true);

        yield return new WaitForSeconds(cutToBlackTime * 0.2f);

        centerScreenMessage.text = message;
        centerScreenMessage.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(cutToBlackTime * 0.8f);

        centerScreenMessage.gameObject.SetActive(false);
        blackScreen.gameObject.SetActive(false);

        if (showTimer)
        {
            timerText.gameObject.SetActive(true);
        }

        GameModeManager.S.playerInteractionEnabled = true;
    }
    public void AddTeamScoreText(Team team)
    {
        TextMeshProUGUI t = teamTexts[teamsJoined];
        t.color = team.teamColor;
        t.text = useFloatScore ? team.floatScore.ToString() : team.intScore.ToString();
        t.gameObject.SetActive(true);

        teamsJoined++;
    }

    public void UpdateTeamScore(int index, string score)
    {
        teamTexts[index].text = score;
    }

    public void SetTimerText(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60.0f);
        float seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    #region Button Callbacks 
    public void btn_TogglePause()
    {
        GameModeManager.S.TogglePause();
    }

    public void btn_QuitToMenu()
    {
        GameModeManager.S.QuitToMenu();
    }

    public void btn_RestartGameMode()
    {
        GameModeManager.S.RestartGame();
    }

    public void btn_QuitGame()
    {
        GameModeManager.S.QuitGame();
    }
    #endregion
}
