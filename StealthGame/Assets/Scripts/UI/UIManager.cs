using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI centerScreenMessage;
    [SerializeField] private TextMeshProUGUI timerText;

    [HideInInspector] public bool useFloatScore = false; // defaults to using int score from teams
    private int teamsJoined = 0;
    [SerializeField] private TextMeshProUGUI[] teamTexts; // need to have as many of these as there can be teams (6 currently, technically)

    [SerializeField] private Image blackScreen;
    public float cutToBlackTime = 2.0f;

    private void Start()
    {
        blackScreen.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
    }

    public void SetPreGameUI()
    {
        centerScreenMessage.gameObject.SetActive(true);
        centerScreenMessage.text = "Press Space To Start";

        foreach(TextMeshProUGUI t in teamTexts)
        {
            t.text = "JOIN";
            t.gameObject.SetActive(true);
        }
    }

    public void AddTeamScoreText(Team team)
    {
        TextMeshProUGUI t = teamTexts[teamsJoined];
        t.color = team.color;
        t.text = useFloatScore ? team.floatScore.ToString() : team.intScore.ToString();
        t.gameObject.SetActive(true);
        teamsJoined++;
    }

    public void UpdateTeamScore(int index, string score)
    {
        teamTexts[index].text = score;
    }

    public void OnGameStart(string message, bool showTimer)
    {
        StartCoroutine(CutToBlackCo(message, showTimer));
    }

    private IEnumerator CutToBlackCo(string message, bool showTimer)
    {
        for (int i = teamsJoined; i < teamTexts.Length; i++)
        {
            teamTexts[i].gameObject.SetActive(false);
        }

        centerScreenMessage.gameObject.SetActive(false);
        blackScreen.gameObject.SetActive(true);

        yield return new WaitForSeconds(cutToBlackTime * 0.2f);

        centerScreenMessage.gameObject.SetActive(true);
        centerScreenMessage.text = message;

        yield return new WaitForSeconds(cutToBlackTime * 0.8f);

        centerScreenMessage.gameObject.SetActive(false);
        blackScreen.gameObject.SetActive(false);

        if (showTimer)
        {
            timerText.gameObject.SetActive(true);
        }
    }

    public void OnGameEnd(string message = "You Win")
    {
        centerScreenMessage.text = message;
        centerScreenMessage.gameObject.SetActive(true);
        timerText.gameObject.SetActive(false);
    }

    public void SetTimerText(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60.0f);
        float seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
