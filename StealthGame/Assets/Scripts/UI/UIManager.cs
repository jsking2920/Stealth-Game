using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI[] teamTexts; // need to have as many of these as there can be teams
    public Image[] teamImages;
    private int scoreFlashCount = 12;

    [SerializeField] private TextMeshProUGUI centerScreenMessage;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private TextMeshProUGUI readyUpWarningText;
    
    [SerializeField] private GameObject endGamePanelButtons;
    [SerializeField] private TextMeshProUGUI endGameMessage;
    public float timeBeforeEndScreen = 6f;
    
    [SerializeField] private Image blackScreen; // used for fade to black/cuts
    [SerializeField] private GameObject lobbyPanel;

    [SerializeField] private TextMeshProUGUI lobbyGamemodeName;
    private TextMeshProUGUI[] lobbyRules;
    [SerializeField] private Color plusScoreColor;
    [SerializeField] private Color minusScoreColor;

    private bool showTimer = false;
    [HideInInspector] public bool useFloatScore = false; // defaults to using int score from teams
    private int teamsJoined = 0;
    private int killersJoined = 0;

    public float cutToBlackTime = 4f;

    private void Start()
    {   
        blackScreen.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        pausePanel.SetActive(false);
        endGameMessage.enabled = false;
        endGamePanelButtons.SetActive(false);

        centerScreenMessage.gameObject.SetActive(false);
        centerScreenMessage.text = "Press Space To Start";
        
        lobbyPanel.gameObject.SetActive(true);
        lobbyRules = lobbyPanel.GetComponentInChildren<VerticalLayoutGroup>().gameObject.GetComponentsInChildren<TextMeshProUGUI>();

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
        resumeButton.Select();
    }

    public void OnResume()
    {
        pausePanel.SetActive(false);
    }

    // TODO: rethink win screen UI
    public void OnGameEnd(string message = "You Win")
    {
        // centerScreenMessage.text = message;
        // centerScreenMessage.gameObject.SetActive(true);
        timerText.gameObject.SetActive(false);
        
        StartCoroutine(ShowEndPanelCo(timeBeforeEndScreen, message));
    }

    IEnumerator ShowEndPanelCo(float seconds, string message)
    {
        yield return new WaitForSeconds(seconds / 3);
        endGameMessage.text = message;
        endGameMessage.enabled = true;
        yield return new WaitForSeconds(seconds);
        endGamePanelButtons.SetActive(true);
        endGamePanelButtons.transform.GetChild(0).GetComponent<Button>().Select();
        Time.timeScale = 0f;
    }

    public void OnGameStart(string message)
    {
        StartCoroutine(CutToBlackCo(message));
    }

    // rules and colors need to be a same length — change to a dict / 2D array in the future
    public void OnLobbyEnter(string name, List<string> rules, List<bool> colors)
    {
        lobbyGamemodeName.text = name;
        for (int i = 0; i < rules.Count; i++)
        {
            lobbyRules[i].text = rules[i];
            if (colors[i]) lobbyRules[i].color = plusScoreColor;
            else lobbyRules[i].color = minusScoreColor;
        }
    }

    private IEnumerator CutToBlackCo(string message)
    {
        for (int i = teamsJoined; i < teamTexts.Length; i++)
        {
            teamTexts[i].gameObject.SetActive(false);
        }

        lobbyPanel.gameObject.SetActive(false);
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

    public IEnumerator FlashScoreCo(int teamIndex)
    {   
        Image circle = teamImages[teamIndex];
        float interval = GameModeManager.S.playerRespawnTime / (scoreFlashCount * 2);

        for (int i = 0; i < scoreFlashCount - 1; i++)
        {
            circle.enabled = false;
            yield return new WaitForSeconds(interval);
            circle.enabled = true;
            yield return new WaitForSeconds(interval);
        }
    }
    
    public void AddTeamScoreText(Team team)
    {
        TextMeshProUGUI t = teamTexts[teamsJoined];
        Image i = teamImages[teamsJoined];
        t.color = team.teamColor;
        i.color = team.teamColor;
        t.text = useFloatScore ? team.floatScore.ToString() : team.intScore.ToString();
        t.gameObject.SetActive(true);
        i.gameObject.SetActive(true);

        teamImages[teamsJoined].gameObject.SetActive(true);

        teamsJoined++;
    }

    public void AddPlayerLivesText(Player player)
    {
        TextMeshProUGUI t = teamTexts[killersJoined];
        Image i = teamImages[killersJoined];
        t.color = Color.white;
        i.color = Color.white;
        t.text = (player.lives).ToString();
        t.gameObject.SetActive(true);
        i.gameObject.SetActive(true);

        killersJoined++;
        teamsJoined++;
    }

    public void NotReadyWarning(string message)
    {
        StartCoroutine(NotReadyWarningCo(message));
    }

    private IEnumerator NotReadyWarningCo(string message)
    {
        readyUpWarningText.gameObject.SetActive(true);
        readyUpWarningText.text = message;

        yield return new WaitForSeconds(3.0f);

        readyUpWarningText.gameObject.SetActive(false);
    }

    public void UpdateTeamScore(int index, string score)
    {
        teamTexts[index].text = score;
    }
    
    public void UpdateLivesText(int index, string lives)
    {
        teamTexts[index].text = lives;
    }

    public void SetTimerText(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60.0f);
        float seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    public void AnimateScore(int teamIndex)
    {
        StartCoroutine(Co_AnimateScore(teamTexts[teamIndex], teamImages[teamIndex]));
    }

    private IEnumerator Co_AnimateScore(TextMeshProUGUI t, Image img)
    {
        Vector3 originalScale = t.transform.localScale;
        Vector3 targetScale = new Vector3(1.25f, 1.25f, 1.0f);

        float halfDuration = 0.08f;
        float timer = 0.0f;

        while (timer < halfDuration)
        {
            img.transform.localScale = Vector3.Lerp(originalScale, targetScale, Mathf.InverseLerp(0.0f, halfDuration, timer));
            timer += Time.deltaTime;
            yield return null;
        }
        
        timer = halfDuration;

        while (timer > 0.0f)
        {
            img.transform.localScale = Vector3.Lerp(originalScale, targetScale, Mathf.InverseLerp(0.0f, halfDuration, timer));
            timer -= Time.deltaTime;
            yield return null;
        }

        while (timer < halfDuration)
        {
            img.transform.localScale = Vector3.Lerp(originalScale, targetScale, Mathf.InverseLerp(0.0f, halfDuration, timer));
            timer += Time.deltaTime;
            yield return null;
        }

        timer = halfDuration;

        while (timer > 0.0f)
        {
            img.transform.localScale = Vector3.Lerp(originalScale, targetScale, Mathf.InverseLerp(0.0f, halfDuration, timer));
            timer -= Time.deltaTime;
            yield return null;
        }

        img.transform.localScale = originalScale;
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
