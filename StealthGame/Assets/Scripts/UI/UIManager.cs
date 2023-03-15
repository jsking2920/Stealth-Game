using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI centerScreenMessage;

    [SerializeField] private TextMeshProUGUI timerText;

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
    }

    public void OnGameStart(string message, bool showTimer)
    {
        StartCoroutine(CutToBlackCo(message, showTimer));
    }

    private IEnumerator CutToBlackCo(string message, bool showTimer)
    {
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
